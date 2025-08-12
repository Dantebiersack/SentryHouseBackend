using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SentryHouseBackend.Models
{
    public class CompraDetalle
    {
        public int Id { get; set; }

        [Required]
        public int CompraProveedorId { get; set; }

        [JsonIgnore] // evita ciclo Compra → Detalles → Compra
        public CompraProveedor? Compra { get; set; }

        [Required]
        public int MateriaPrimaId { get; set; }
        public MateriaPrima? MateriaPrima { get; set; } // one-way; sin colección en MateriaPrima → sin ciclo

        [Required]
        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        // IVA por línea (porcentaje almacenado para trazabilidad)
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
