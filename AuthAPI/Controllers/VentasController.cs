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
                .Where(v => v.UsuarioId == usuarioId)
                .Include(v => v.Cotizacion)
                    .ThenInclude(c => c.CotizacionServicios)
                        .ThenInclude(cs => cs.Servicio)
                .SelectMany(v => v.Cotizacion.CotizacionServicios.Select(cs => new
                {
                    ServicioNombre = cs.Servicio.Nombre,
                    ServicioDescripcion = cs.Servicio.Descripcion,
                    FechaVenta = v.FechaVenta,
                    archivoDocumento = cs.Servicio.ArchivoDocumento
                }))
                .ToListAsync();

            return Ok(ventas);
        }
    }
}
