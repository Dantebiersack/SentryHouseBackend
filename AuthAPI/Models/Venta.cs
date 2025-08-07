using Microsoft.AspNetCore.Identity;

namespace SentryHouseBackend.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int CotizacionId { get; set; }
        public Cotizacion Cotizacion { get; set; }

        public string UsuarioId { get; set; } // este es el Id de AspNetUsers
        public AppUser Usuario { get; set; }  // esta es la navegación a AspNetUsers

        public DateTime FechaVenta { get; set; } = DateTime.Now;
        public string ArchivoDocumento { get; set; }
    }
}
