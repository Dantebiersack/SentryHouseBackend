using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace SentryHouseBackend.Models
{
    public class Venta
    {
        public int Id { get; set; }

        public int? CotizacionId { get; set; }
        public Cotizacion? Cotizacion { get; set; }

        public string UsuarioId { get; set; }
        public AppUser Usuario { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;

        public EstadoVenta Estado { get; set; } = EstadoVenta.Borrador;

        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }

        [JsonIgnore]
        public ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}
