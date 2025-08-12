using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentryHouseBackend.Data;
using System.Linq;
using System.Threading.Tasks;
using SentryHouseBackend.Dtos;
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
    }


}
