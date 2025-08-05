namespace SentryHouseBackend.Models
{
    public class CotizacionServicio
    {
        public int CotizacionId { get; set; }
        public Cotizacion? Cotizacion { get; set; }

        public int ServicioId { get; set; }
        public Servicio? Servicio { get; set; }
    }

}
