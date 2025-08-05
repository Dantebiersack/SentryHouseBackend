using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SentryHouseBackend.Models
{
    public class UsuarioApp
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("correo")]
        public string Correo { get; set; }

        [BsonElement("contrasena")]
        public string Contrasena { get; set; }

        [BsonElement("fechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [BsonElement("cotizacionId")]
        public int CotizacionId { get; set; }
    }

}
