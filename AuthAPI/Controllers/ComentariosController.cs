using Microsoft.AspNetCore.Mvc;
using SentryHouseBackend.Models;
using System.Collections.Generic;

namespace SentryHouseBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentariosController : ControllerBase
    {
        private static List<Comentarios> comentarios = new List<Comentarios>();

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(comentarios);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Comentarios model)
        {
            if (ModelState.IsValid)
            {
                comentarios.Add(model);
                return Ok(model);
            }
            return BadRequest(ModelState);
        }
    }
}
