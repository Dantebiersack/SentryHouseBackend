using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentryHouseBackend.Models
{
    public class CompraProveedor
    {
        public int Id { get; set; }

        [Required]
        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; } // one-way; no colección en Proveedor → sin ciclo

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        public EstadoCompra Estado { get; set; } = EstadoCompra.Borrador;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Iva { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public string? NumeroDocumento { get; set; } // factura, XML, etc.
        public string? Notas { get; set; }

        public ICollection<CompraDetalle> Detalles { get; set; } = new List<CompraDetalle>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
