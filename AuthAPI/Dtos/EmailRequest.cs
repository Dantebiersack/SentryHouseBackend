namespace SentryHouseBackend.Dtos
{
    public class EmailRequest
    {
        public string ? Destinatario { get; set; }
        public string Asunto { get; set; }
        public string Cuerpo { get; set; }
    }
}
