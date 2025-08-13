using System.Text.Json.Serialization;

namespace SentryHouseBackend.Models
{
    public class Cotizacion
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public bool EstaFinalizada { get; set; } = false;

        // 🔹 Relación con Usuario que solicitó la cotización
        public string? UsuarioId { get; set; } // FK hacia AppUser
        public AppUser? Usuario { get; set; }  // Navegación

        // Relación con Servicios
        public ICollection<CotizacionServicio> CotizacionServicios { get; set; }

        public Venta? VentaGenerada { get; set; }
    }
}