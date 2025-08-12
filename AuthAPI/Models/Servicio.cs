using System.Text.Json.Serialization;

namespace SentryHouseBackend.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ArchivoDocumento { get; set; }

        public decimal? PrecioBase { get; set; }

        [JsonIgnore] 
        public ICollection<ServicioMaterial> Materiales { get; set; } = new List<ServicioMaterial>();
    }
}
