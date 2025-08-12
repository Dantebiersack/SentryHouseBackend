using SentryHouseBackend.Models;

namespace SentryHouseBackend.Dtos
{
    public class VentaLineaCreateDto
    {
        public int ServicioId { get; set; }
        public int Cantidad { get; set; } = 1;
        public decimal? PrecioUnitario { get; set; } // si null, usar PrecioBase del servicio
        public decimal? IvaPorcentaje { get; set; }  // default 0.16
    }

    public class VentaCreateDto
    {
        public string UsuarioId { get; set; }
        public int? CotizacionId { get; set; } // opcional
        public List<VentaLineaCreateDto> Lineas { get; set; } = new();
    }

    public class VentaLiteDto
    {
        public int Id { get; set; }
        public DateTime FechaVenta { get; set; }
        public EstadoVenta Estado { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }

    public class VentaDetalleDto
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal IvaPorcentaje { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal TotalLinea { get; set; }
    }

    public class VentaDetailDto : VentaLiteDto
    {
        public int? CotizacionId { get; set; }
        public string UsuarioId { get; set; }
        public List<VentaDetalleDto> Detalles { get; set; } = new();
    }

    public class VentaEstadoDto { public EstadoVenta Estado { get; set; } }
}
