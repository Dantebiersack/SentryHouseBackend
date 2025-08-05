using Microsoft.AspNetCore.Mvc;
using SentryHouseBackend.Data;
using SentryHouseBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace SentryHouseBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MateriaPrimaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MateriaPrimaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MateriaPrima>>> GetMateriasPrimas()
        {
            return await _context.MateriasPrimas.Include(mp => mp.Proveedor).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<MateriaPrima>> CrearMateriaPrima(MateriaPrima mp)
        {
            _context.MateriasPrimas.Add(mp);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMateriasPrimas), new { id = mp.Id }, mp);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarMateriaPrima(int id, [FromBody] MateriaPrima mpActualizada)
        {
            if (id != mpActualizada.Id)
                return BadRequest("El ID no coincide.");

            var mpExistente = await _context.MateriasPrimas.FindAsync(id);
            if (mpExistente == null)
                return NotFound();

            mpExistente.NombreProducto = mpActualizada.NombreProducto;
            mpExistente.Cantidad = mpActualizada.Cantidad;
            mpExistente.CostoTotal = mpActualizada.CostoTotal;
            mpExistente.ProveedorId = mpActualizada.ProveedorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarMateriaPrima(int id)
        {
            var mp = await _context.MateriasPrimas.FindAsync(id);
            if (mp == null)
                return NotFound();

            _context.MateriasPrimas.Remove(mp);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
