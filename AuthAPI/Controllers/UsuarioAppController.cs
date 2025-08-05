using Microsoft.AspNetCore.Mvc;
using SentryHouseBackend.Models;
using SentryHouseBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace SentryHouseBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioAppController : ControllerBase
    {
        private readonly UsuarioAppService _usuarioAppService;

        public UsuarioAppController(UsuarioAppService usuarioAppService)
        {
            _usuarioAppService = usuarioAppService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuarioApp([FromBody] UsuarioApp usuario)
        {
            var creado = await _usuarioAppService.CrearUsuarioAppAsync(usuario);

            if (!creado)
                return BadRequest(new { mensaje = "Ya existe un usuario con ese correo." });

            return Ok(new { mensaje = "Usuario creado en MongoDB." });
        }
    }
}
