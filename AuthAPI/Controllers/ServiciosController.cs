using Microsoft.AspNetCore.Mvc;
using SentryHouseBackend.Data;
using SentryHouseBackend.Models;
using Microsoft.EntityFrameworkCore;

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

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarServicio(int id, [FromBody] Servicio servicioActualizado)
        {
            if (id != servicioActualizado.Id)
                return BadRequest("El ID no coincide.");

            var servicioExistente = await _context.Servicios.FindAsync(id);
            if (servicioExistente == null)
                return NotFound();

            servicioExistente.Nombre = servicioActualizado.Nombre;
            servicioExistente.Descripcion = servicioActualizado.Descripcion;

            await _context.SaveChangesAsync();
            return NoContent();
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

    }

}
