namespace SentryHouseBackend.Dtos
{
    public class CotizacionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public bool EstaFinalizada { get; set; }
        public string? UsuarioId { get; set; } 
        public List<string> Servicios { get; set; }
    }

}
