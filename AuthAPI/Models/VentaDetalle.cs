using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SentryHouseBackend.Models
{
    public class VentaDetalle
    {
        public int Id { get; set; }

        [Required]
        public int VentaId { get; set; }

        [JsonIgnore]
        public Venta? Venta { get; set; }

        [Required]
        public int ServicioId { get; set; }
        public Servicio? Servicio { get; set; }

        [Required]
        public int Cantidad { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(5,4)")]
        public decimal IvaPorcentaje { get; set; } = 0.16m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Iva { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalLinea { get; set; }
    }
}
