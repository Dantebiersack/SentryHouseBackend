using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SentryHouseBackend.Dtos;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SentryHouseBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("enviarCorreo")]
        public async Task<IActionResult> EnviarCorreo([FromBody] EmailRequest request)
        {
            try
            {
                var fromEmail = _configuration["correo:FromEmail"];
                var fromPassword = _configuration["correo:FromPassword"];
                var smtpHost = _configuration["correo:SmtpHost"];
                var smtpPort = int.Parse(_configuration["correo:SmtpPort"]);

                var fromAddress = new MailAddress(fromEmail, "SentryHouse");
                var toAddress = new MailAddress(request.Destinatario);

                using var smtp = new SmtpClient
                {
                    Host = smtpHost,
                    Port = smtpPort,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = request.Asunto,
                    Body = request.Cuerpo
                };

                await smtp.SendMailAsync(message);

                return Ok(new { mensaje = "Correo enviado correctamente" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
