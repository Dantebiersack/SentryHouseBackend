using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentryHouseBackend.Data;
using SentryHouseBackend.Dtos;
using SentryHouseBackend.Models;
using System.Security.Claims;

namespace SentryHouseBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CotizacionesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CotizacionDto>>> GetCotizaciones()
        {
            var cotizaciones = await _context.Cotizaciones
                .Include(c => c.CotizacionServicios)
                    .ThenInclude(cs => cs.Servicio)
                .Select(c => new CotizacionDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Email = c.Email,
                    FechaSolicitud = c.FechaSolicitud,
                    EstaFinalizada = c.EstaFinalizada,
                    UsuarioId = c.UsuarioId, // 🔹 solo guardamos el ID
                    Servicios = c.CotizacionServicios
                        .Select(cs => cs.Servicio.Nombre)
                        .ToList()
                }).ToListAsync();

            return Ok(cotizaciones);
        }

        [HttpGet("misCotizaciones")]
        public async Task<ActionResult<IEnumerable<CotizacionDto>>> GetCotizacionesUser()
        {
            // Obtener el ID del usuario logueado
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

            var cotizaciones = await _context.Cotizaciones
                .Where(c => c.UsuarioId == userId) // 🔹 filtramos por usuario
                .Include(c => c.CotizacionServicios)
                    .ThenInclude(cs => cs.Servicio)
                .Select(c => new CotizacionDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Email = c.Email,
                    FechaSolicitud = c.FechaSolicitud,
                    EstaFinalizada = c.EstaFinalizada,
                    UsuarioId = c.UsuarioId,
                    Servicios = c.CotizacionServicios
                        .Select(cs => cs.Servicio.Nombre)
                        .ToList()
                })
                .ToListAsync();

            return Ok(cotizaciones);
        }



        [HttpPost]
        public async Task<ActionResult> CrearCotizacion(CrearCotizacionDto dto)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

            var cotizacion = new Cotizacion
            {
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                FechaSolicitud = dto.FechaSolicitud,
                EstaFinalizada = dto.EstaFinalizada,
                UsuarioId = dto.UsuarioId, 
                CotizacionServicios = dto.ServiciosIds.Select(sid => new CotizacionServicio
                {
                    ServicioId = sid
                }).ToList()
            };

            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCotizaciones), new { id = cotizacion.Id }, null);
        }

    }
}
