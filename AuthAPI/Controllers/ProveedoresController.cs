using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentryHouseBackend.Data;
using SentryHouseBackend.Models;

namespace SentryHouseBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            return await _context.Proveedores.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Proveedor>> CrearProveedor(Proveedor proveedor)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProveedores), new { id = proveedor.Id }, proveedor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProveedor(int id, [FromBody] Proveedor proveedorActualizado)
        {
            if (id != proveedorActualizado.Id)
                return BadRequest("El ID no coincide.");

            var proveedorExistente = await _context.Proveedores.FindAsync(id);
            if (proveedorExistente == null)
                return NotFound();

            proveedorExistente.Nombre = proveedorActualizado.Nombre;
            proveedorExistente.Direccion = proveedorActualizado.Direccion;
            proveedorExistente.Telefono = proveedorActualizado.Telefono;
            proveedorExistente.Correo = proveedorActualizado.Correo;
            proveedorExistente.NombreYCargo = proveedorActualizado.NombreYCargo;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
