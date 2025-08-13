using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentryHouseBackend.Data;
using System.Linq;
using System.Threading.Tasks;
using SentryHouseBackend.Dtos;
using System.Globalization;
using SentryHouseBackend.Models;

namespace SentryHouseBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerVentasPorUsuario(string usuarioId)
        {
            var ventas = await _context.Ventas
                .AsNoTracking()
                .Where(v => v.UsuarioId == usuarioId)
                .OrderByDescending(v => v.FechaVenta)
                .Select(v => new
                {
                    id = v.Id,
                    fechaVenta = v.FechaVenta,
                     estado = v.Estado,
                    subtotal = v.Subtotal,
                    iva = v.Iva,
                    total = v.Total
                })
                .ToListAsync();

            return Ok(ventas);
        }


        [HttpPost]
        public async Task<ActionResult> CrearVenta([FromBody] VentaCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UsuarioId))
                return BadRequest("UsuarioId requerido.");

            if (dto.Lineas == null || dto.Lineas.Count == 0)
                return BadRequest("Debe incluir al menos un servicio en la venta.");

            var servicioIds = dto.Lineas.Select(l => l.ServicioId).Distinct().ToList();
            var servicios = await _context.Servicios
                .Where(s => servicioIds.Contains(s.Id))
                .ToListAsync();

            if (servicios.Count != servicioIds.Count)
                return BadRequest("Servicio inexistente en la venta.");

            var venta = new Venta
            {
                UsuarioId = dto.UsuarioId,
                CotizacionId = dto.CotizacionId,
                Estado = EstadoVenta.Borrador,
                FechaVenta = DateTime.UtcNow
            };

            foreach (var li in dto.Lineas)
            {
                var srv = servicios.First(s => s.Id == li.ServicioId);
                var precio = li.PrecioUnitario ?? srv.PrecioBase ?? 0m;
                var ivaPct = li.IvaPorcentaje ?? 0.16m;
                var sub = precio * li.Cantidad;
                var iva = Math.Round(sub * ivaPct, 2, MidpointRounding.AwayFromZero);
                var total = sub + iva;

                venta.Detalles.Add(new VentaDetalle
                {
                    ServicioId = li.ServicioId,
                    Cantidad = li.Cantidad,
                    PrecioUnitario = precio,
                    IvaPorcentaje = ivaPct,
                    Subtotal = sub,
                    Iva = iva,
                    TotalLinea = total
                });

                venta.Subtotal += sub;
                venta.Iva += iva;
                venta.Total += total;
            }

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVenta), new { id = venta.Id }, new { id = venta.Id });
        }

        // GET api/ventas/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VentaDetailDto>> GetVenta(int id)
        {
            var v = await _context.Ventas
                .AsNoTracking()
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Servicio)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (v == null) return NotFound();

            return Ok(new VentaDetailDto
            {
                Id = v.Id,
                FechaVenta = v.FechaVenta,
                Estado = v.Estado,
                Subtotal = v.Subtotal,
                Iva = v.Iva,
                Total = v.Total,
                CotizacionId = v.CotizacionId,
                UsuarioId = v.UsuarioId,
                Detalles = v.Detalles.Select(d => new VentaDetalleDto
                {
                    Id = d.Id,
                    ServicioId = d.ServicioId,
                    ServicioNombre = d.Servicio?.Nombre ?? "",
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    IvaPorcentaje = d.IvaPorcentaje,
                    Subtotal = d.Subtotal,
                    Iva = d.Iva,
                    TotalLinea = d.TotalLinea
                }).ToList()
            });
        }

        // PUT api/ventas/{id}/estado
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] VentaEstadoDto body)
        {
            var v = await _context.Ventas
                .Include(x => x.Detalles)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (v == null) return NotFound();

            // Reglas simples (ajústalas a tu flujo real)
            if (v.Estado == EstadoVenta.Cancelada)
                return Conflict("La venta cancelada no puede cambiar de estado.");

            if (v.Estado == EstadoVenta.Entregada && body.Estado != EstadoVenta.Entregada)
                return Conflict("La venta entregada no puede regresar a otro estado.");

            v.Estado = body.Estado;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("dashboard/summary")]
        // [Authorize(Roles = "Admin")] // descomenta si ya manejas roles
        public async Task<IActionResult> GetDashboardSummary([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            // Rango por defecto: últimos 30 días
            var hoyUtc = DateTime.UtcNow.Date;
            var inicio = (from?.Date) ?? hoyUtc.AddDays(-29);
            var fin = (to?.Date) ?? hoyUtc;

            // Normaliza fin para incluir todo el día
            var finInclusive = fin.AddDays(1);

            // KPIs básicos
            var ventasQuery = _context.Ventas.AsNoTracking().Where(v => v.FechaVenta >= inicio && v.FechaVenta < finInclusive);

            var subtotal = await ventasQuery.SumAsync(v => (decimal?)v.Subtotal) ?? 0m;
            var iva = await ventasQuery.SumAsync(v => (decimal?)v.Iva) ?? 0m;
            var total = await ventasQuery.SumAsync(v => (decimal?)v.Total) ?? 0m;
            var cantidadVentas = await ventasQuery.CountAsync();

            // Ventas hoy / semana / mes (en UTC; ajusta si usas zona MX local en BD)
            var inicioSemana = hoyUtc.AddDays(-(int)hoyUtc.DayOfWeek); // domingo como inicio
            var inicioMes = new DateTime(hoyUtc.Year, hoyUtc.Month, 1);

            var ventasHoy = await _context.Ventas.AsNoTracking()
                .Where(v => v.FechaVenta >= hoyUtc && v.FechaVenta < hoyUtc.AddDays(1))
                .SumAsync(v => (decimal?)v.Total) ?? 0m;

            var ventasSemana = await _context.Ventas.AsNoTracking()
                .Where(v => v.FechaVenta >= inicioSemana && v.FechaVenta < hoyUtc.AddDays(1))
                .SumAsync(v => (decimal?)v.Total) ?? 0m;

            var ventasMes = await _context.Ventas.AsNoTracking()
                .Where(v => v.FechaVenta >= inicioMes && v.FechaVenta < hoyUtc.AddDays(1))
                .SumAsync(v => (decimal?)v.Total) ?? 0m;

            // Serie diaria (últimos N días del rango)
            var serieDiaria = await _context.Ventas.AsNoTracking()
                .Where(v => v.FechaVenta >= inicio && v.FechaVenta < finInclusive)
                .GroupBy(v => v.FechaVenta.Date)
                .Select(g => new { fecha = g.Key, total = g.Sum(x => x.Total) })
                .OrderBy(x => x.fecha)
                .ToListAsync();

            // Top servicios (por importe) en el rango
            var topServicios = await _context.VentasDetalles.AsNoTracking()
                .Where(d => d.Venta.FechaVenta >= inicio && d.Venta.FechaVenta < finInclusive)
                .GroupBy(d => new { d.ServicioId, d.Servicio!.Nombre })
                .Select(g => new {
                    servicioId = g.Key.ServicioId,
                    servicio = g.Key.Nombre,
                    cantidad = g.Sum(x => x.Cantidad),
                    importe = g.Sum(x => x.TotalLinea)
                })
                .OrderByDescending(x => x.importe)
                .Take(5)
                .ToListAsync();

            var porEstado = await ventasQuery
                .GroupBy(v => v.Estado)
                .Select(g => new { estado = g.Key, cantidad = g.Count(), total = g.Sum(x => x.Total) })
                .ToListAsync();

            return Ok(new
            {
                rango = new { from = inicio, to = fin },
                kpis = new
                {
                    subtotal,
                    iva,
                    total,
                    cantidadVentas,
                    ventasHoy,
                    ventasSemana,
                    ventasMes
                },
                series = new
                {
                    porDia = serieDiaria.Select(x => new { fecha = x.fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), total = x.total })
                },
                topServicios,
                porEstado
            });
        }


    }


}
