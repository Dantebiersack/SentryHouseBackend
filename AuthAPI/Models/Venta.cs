namespace SentryHouseBackend.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int CotizacionId { get; set; }
        public Cotizacion Cotizacion { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;
        public string ArchivoDocumento { get; set; } // Ruta o nombre de archivo generado (PDF, etc.)
    }

}
