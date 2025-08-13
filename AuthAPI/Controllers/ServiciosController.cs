using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentryHouseBackend.Data;
using SentryHouseBackend.Dtos;
using SentryHouseBackend.Models;
using System.Linq;

namespace SentryHouseBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiciosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Servicio>>> GetServicios()
        {
            return await _context.Servicios.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Servicio>> CrearServicio(Servicio servicio)
        {
            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetServicios), new { id = servicio.Id }, servicio);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
                return NotFound();

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET api/servicios/{id}/detalle
        [HttpGet("{id:int}/detalle")]
        public async Task<ActionResult<ServicioDetailDto>> GetServicioDetalle(int id)
        {
            var srv = await _context.Servicios
                .Include(s => s.Materiales)
                    .ThenInclude(m => m.MateriaPrima)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (srv == null) return NotFound();

            return Ok(new ServicioDetailDto
            {
                Id = srv.Id,
                Nombre = srv.Nombre,
                Descripcion = srv.Descripcion,
                ArchivoDocumento = srv.ArchivoDocumento,
                PrecioBase = srv.PrecioBase,
                // Proyecta a la nueva clase ServicioDetalleMaterialDto
                Materiales = srv.Materiales.Select(m => new ServicioDetalleMaterialDto
                {
                    MateriaPrimaId = m.MateriaPrimaId,
                    MateriaPrimaNombre = m.MateriaPrima?.NombreProducto ?? "", // Usa el nombre de la MateriaPrima
                    CantidadRequerida = m.CantidadRequerida,
                    Unidad = m.Unidad
                }).ToList()
            });
        }

        // PUT api/servicios/{id}/materiales  (reemplaza la BOM completa)
        [HttpPut("{id:int}/materiales")]
        public async Task<IActionResult> SetMateriales(int id, [FromBody] List<ServicioMaterialDto> bom)
        {
            var srv = await _context.Servicios
                .Include(s => s.Materiales)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (srv == null) return NotFound();

            // valida Materias
            var ids = bom.Select(b => b.MateriaPrimaId).Distinct().ToList();
            var existentes = await _context.MateriasPrimas.Where(m => ids.Contains(m.Id))
                .Select(m => m.Id).ToListAsync();

            if (existentes.Count != ids.Count) return BadRequest("Materia prima inexistente en la BOM.");

            // Eliminar los materiales existentes
            _context.ServiciosMateriales.RemoveRange(srv.Materiales);

            // Agregar los nuevos materiales
            var nuevosMateriales = bom.Select(b => new ServicioMaterial
            {
                ServicioId = id,
                MateriaPrimaId = b.MateriaPrimaId,
                CantidadRequerida = b.CantidadRequerida,
                Unidad = b.Unidad
            }).ToList();

            _context.ServiciosMateriales.AddRange(nuevosMateriales);

            // Guardar los cambios (eliminación e inserción) en una sola transacción
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }

}
