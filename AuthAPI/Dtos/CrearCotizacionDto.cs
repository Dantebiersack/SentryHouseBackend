namespace SentryHouseBackend.Dtos
{
    public class CrearCotizacionDto
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public bool EstaFinalizada { get; set; }

        public string? UsuarioId { get; set; }

        public List<int> ServiciosIds { get; set; }
    }
}
