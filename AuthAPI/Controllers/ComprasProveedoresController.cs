using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentryHouseBackend.Data;
using SentryHouseBackend.Dtos;
using SentryHouseBackend.Models;
using System.Transactions;

namespace SentryHouseBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComprasProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/comprasproveedores?status=&proveedorId=&from=&to=&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraLiteDto>>> GetCompras(
            [FromQuery] EstadoCompra? status,
            [FromQuery] int? proveedorId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 200) pageSize = 20;

            var q = _context.ComprasProveedores
                .AsNoTracking()
                .Include(c => c.Proveedor)
                .OrderByDescending(c => c.Fecha)
                .AsQueryable();

            if (status.HasValue) q = q.Where(c => c.Estado == status.Value);
            if (proveedorId.HasValue) q = q.Where(c => c.ProveedorId == proveedorId.Value);
            if (from.HasValue) q = q.Where(c => c.Fecha >= from.Value.Date);
            if (to.HasValue) q = q.Where(c => c.Fecha < to.Value.Date.AddDays(1));

            var data = await q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(c => new CompraLiteDto
                {
                    Id = c.Id,
                    Fecha = c.Fecha,
                    Estado = c.Estado,
                    ProveedorId = c.ProveedorId,
                    ProveedorNombre = c.Proveedor != null ? c.Proveedor.Nombre : string.Empty,
                    Subtotal = c.Subtotal,
                    Iva = c.Iva,
                    Total = c.Total
                })
                .ToListAsync();

            return Ok(data);
        }

        // GET api/comprasproveedores/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CompraDetailDto>> GetCompra(int id)
        {
            var compra = await _context.ComprasProveedores
                .AsNoTracking()
                .Include(c => c.Proveedor)
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.MateriaPrima)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null) return NotFound();

            var dto = new CompraDetailDto
            {
                Id = compra.Id,
                Fecha = compra.Fecha,
                Estado = compra.Estado,
                ProveedorId = compra.ProveedorId,
                ProveedorNombre = compra.Proveedor?.Nombre ?? string.Empty,
                Subtotal = compra.Subtotal,
                Iva = compra.Iva,
                Total = compra.Total,
                NumeroDocumento = compra.NumeroDocumento,
                Notas = compra.Notas,
                Detalles = compra.Detalles.Select(d => new CompraDetalleDto
                {
                    Id = d.Id,
                    MateriaPrimaId = d.MateriaPrimaId,
                    MateriaPrimaNombre = d.MateriaPrima?.NombreProducto ?? string.Empty,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    IvaPorcentaje = d.IvaPorcentaje,
                    Subtotal = d.Subtotal,
                    Iva = d.Iva,
                    TotalLinea = d.TotalLinea
                }).ToList()
            };

            return Ok(dto);
        }

        // POST api/comprasproveedores
        [HttpPost]
        public async Task<ActionResult<CompraDetailDto>> CrearCompra([FromBody] CompraCreateDto dto)
        {
            // Validaciones básicas
            if (!await _context.Proveedores.AnyAsync(p => p.Id == dto.ProveedorId))
                return BadRequest("Proveedor no existe.");

            if (dto.Detalles == null || dto.Detalles.Count == 0)
                return BadRequest("Debe agregar al menos un detalle.");

            var materiaIds = dto.Detalles.Select(x => x.MateriaPrimaId).Distinct().ToList();
            var existentes = await _context.MateriasPrimas
                .Where(m => materiaIds.Contains(m.Id))
                .Select(m => m.Id).ToListAsync();

            if (existentes.Count != materiaIds.Count)
                return BadRequest("Una o más materias primas no existen.");

            var compra = new CompraProveedor
            {
                ProveedorId = dto.ProveedorId,
                Fecha = dto.Fecha ?? DateTime.UtcNow,
                NumeroDocumento = dto.NumeroDocumento,
                Notas = dto.Notas,
                Estado = EstadoCompra.Borrador
            };

            foreach (var li in dto.Detalles)
            {
                var ivaPct = li.IvaPorcentaje ?? 0.16m;
                var subtotal = li.Cantidad * li.PrecioUnitario;
                var iva = Math.Round(subtotal * ivaPct, 2, MidpointRounding.AwayFromZero);
                var totalLinea = subtotal + iva;

                compra.Detalles.Add(new CompraDetalle
                {
                    MateriaPrimaId = li.MateriaPrimaId,
                    Cantidad = li.Cantidad,
                    PrecioUnitario = li.PrecioUnitario,
                    IvaPorcentaje = ivaPct,
                    Subtotal = subtotal,
                    Iva = iva,
                    TotalLinea = totalLinea
                });

                compra.Subtotal += subtotal;
                compra.Iva += iva;
                compra.Total += totalLinea;
            }

            _context.ComprasProveedores.Add(compra);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompra), new { id = compra.Id }, new { id = compra.Id });
        }

        // PUT api/comprasproveedores/5  (solo encabezado; detalles se manejan abajo)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCompra(int id, [FromBody] CompraUpdateDto dto)
        {
            var compra = await _context.ComprasProveedores.FindAsync(id);
            if (compra == null) return NotFound();

            if (compra.Estado is EstadoCompra.Recibida or EstadoCompra.Cancelada)
                return Conflict("No se puede editar una compra en estado Recibida o Cancelada.");

            if (dto.Fecha.HasValue) compra.Fecha = dto.Fecha.Value;
            if (!string.IsNullOrWhiteSpace(dto.NumeroDocumento)) compra.NumeroDocumento = dto.NumeroDocumento;
            compra.Notas = dto.Notas ?? compra.Notas;
            compra.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST api/comprasproveedores/5/detalles
        [HttpPost("{id:int}/detalles")]
        public async Task<IActionResult> AgregarDetalle(int id, [FromBody] CompraDetalleCreateDto li)
        {
            var compra = await _context.ComprasProveedores
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null) return NotFound();
            if (compra.Estado is EstadoCompra.Recibida or EstadoCompra.Cancelada)
                return Conflict("No se pueden agregar detalles en este estado.");

            if (!await _context.MateriasPrimas.AnyAsync(m => m.Id == li.MateriaPrimaId))
                return BadRequest("Materia prima no existe.");

            var ivaPct = li.IvaPorcentaje ?? 0.16m;
            var subtotal = li.Cantidad * li.PrecioUnitario;
            var iva = Math.Round(subtotal * ivaPct, 2, MidpointRounding.AwayFromZero);
            var totalLinea = subtotal + iva;

            var det = new CompraDetalle
            {
                CompraProveedorId = id,
                MateriaPrimaId = li.MateriaPrimaId,
                Cantidad = li.Cantidad,
                PrecioUnitario = li.PrecioUnitario,
                IvaPorcentaje = ivaPct,
                Subtotal = subtotal,
                Iva = iva,
                TotalLinea = totalLinea
            };

            compra.Detalles.Add(det);
            compra.Subtotal += subtotal;
            compra.Iva += iva;
            compra.Total += totalLinea;
            compra.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT api/comprasproveedores/5/detalles/99
        [HttpPut("{id:int}/detalles/{detalleId:int}")]
        public async Task<IActionResult> EditarDetalle(int id, int detalleId, [FromBody] CompraDetalleCreateDto li)
        {
            var compra = await _context.ComprasProveedores
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null) return NotFound();
            if (compra.Estado is EstadoCompra.Recibida or EstadoCompra.Cancelada)
                return Conflict("No se pueden editar detalles en este estado.");

            var det = compra.Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (det == null) return NotFound();

            // revertimos totales, recalculamos y sumamos de nuevo
            compra.Subtotal -= det.Subtotal;
            compra.Iva -= det.Iva;
            compra.Total -= det.TotalLinea;

            var ivaPct = li.IvaPorcentaje ?? 0.16m;
            var subtotal = li.Cantidad * li.PrecioUnitario;
            var iva = Math.Round(subtotal * ivaPct, 2, MidpointRounding.AwayFromZero);
            var totalLinea = subtotal + iva;

            det.MateriaPrimaId = li.MateriaPrimaId;
            det.Cantidad = li.Cantidad;
            det.PrecioUnitario = li.PrecioUnitario;
            det.IvaPorcentaje = ivaPct;
            det.Subtotal = subtotal;
            det.Iva = iva;
            det.TotalLinea = totalLinea;

            compra.Subtotal += subtotal;
            compra.Iva += iva;
            compra.Total += totalLinea;
            compra.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/comprasproveedores/5/detalles/99
        [HttpDelete("{id:int}/detalles/{detalleId:int}")]
        public async Task<IActionResult> EliminarDetalle(int id, int detalleId)
        {
            var compra = await _context.ComprasProveedores
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null) return NotFound();
            if (compra.Estado is EstadoCompra.Recibida or EstadoCompra.Cancelada)
                return Conflict("No se pueden eliminar detalles en este estado.");

            var det = compra.Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (det == null) return NotFound();

            compra.Subtotal -= det.Subtotal;
            compra.Iva -= det.Iva;
            compra.Total -= det.TotalLinea;

            _context.ComprasDetalles.Remove(det);
            compra.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT api/comprasproveedores/5/estado
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambioEstadoDto body)
        {
            var compra = await _context.ComprasProveedores
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null) return NotFound();

            if (compra.Estado == EstadoCompra.Recibida && body.Estado != EstadoCompra.Recibida)
                return Conflict("Una compra recibida no puede cambiar a otro estado.");

            if (compra.Estado == EstadoCompra.Cancelada)
                return Conflict("Una compra cancelada no puede cambiar de estado.");

            // Reglas simples
            if (body.Estado == EstadoCompra.Cancelada && compra.Estado == EstadoCompra.Recibida)
                return Conflict("No se puede cancelar una compra ya recibida.");

            compra.Estado = body.Estado;
            compra.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST api/comprasproveedores/5/recibir
        [HttpPost("{id:int}/recibir")]
        public async Task<IActionResult> RecibirCompra(int id)
        {
            var compra = await _context.ComprasProveedores
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null) return NotFound();
            if (compra.Estado is EstadoCompra.Recibida or EstadoCompra.Cancelada)
                return Conflict("Compra no válida para recepción.");

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // Actualiza inventario (suma cantidades y costo total sin IVA)
                var detallePorMateria = compra.Detalles
                    .GroupBy(d => d.MateriaPrimaId)
                    .Select(g => new
                    {
                        MateriaPrimaId = g.Key,
                        Cantidad = g.Sum(x => x.Cantidad),
                        CostoSinIva = g.Sum(x => x.Subtotal) // solo subtotal a inventario
                    })
                    .ToList();

                var mpIds = detallePorMateria.Select(x => x.MateriaPrimaId).ToList();
                var mats = await _context.MateriasPrimas
                    .Where(m => mpIds.Contains(m.Id))
                    .ToListAsync();

                foreach (var x in detallePorMateria)
                {
                    var mp = mats.First(m => m.Id == x.MateriaPrimaId);
                    mp.Cantidad += x.Cantidad;
                    mp.CostoTotal += x.CostoSinIva; // valorado con costo sin IVA
                }

                compra.Estado = EstadoCompra.Recibida;
                compra.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return NoContent();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // DELETE api/comprasproveedores/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarCompra(int id)
        {
            var compra = await _context.ComprasProveedores.FindAsync(id);
            if (compra == null) return NotFound();

            if (compra.Estado != EstadoCompra.Borrador && compra.Estado != EstadoCompra.Cancelada)
                return Conflict("Solo se pueden eliminar compras en Borrador o Cancelada.");

            _context.ComprasProveedores.Remove(compra);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
