using System.ComponentModel.DataAnnotations;
using SentryHouseBackend.Models;

namespace SentryHouseBackend.Dtos
{
    public class CompraCreateDto
    {
        [Required]
        public int ProveedorId { get; set; }
        public DateTime? Fecha { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? Notas { get; set; }
        public List<CompraDetalleCreateDto> Detalles { get; set; } = new();
    }

    public class CompraDetalleCreateDto
    {
        [Required] public int MateriaPrimaId { get; set; }
        [Required] public int Cantidad { get; set; }
        [Required] public decimal PrecioUnitario { get; set; }
        public decimal? IvaPorcentaje { get; set; } // default 0.16 si null
    }

    public class CompraUpdateDto
    {
        public DateTime? Fecha { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? Notas { get; set; }
    }

    public class CambioEstadoDto
    {
        [Required] public EstadoCompra Estado { get; set; }
    }

    public class CompraLiteDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public EstadoCompra Estado { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }

    public class CompraDetailDto : CompraLiteDto
    {
        public string? NumeroDocumento { get; set; }
        public string? Notas { get; set; }
        public List<CompraDetalleDto> Detalles { get; set; } = new();
    }

    public class CompraDetalleDto
    {
        public int Id { get; set; }
        public int MateriaPrimaId { get; set; }
        public string MateriaPrimaNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal IvaPorcentaje { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal TotalLinea { get; set; }
    }
}
