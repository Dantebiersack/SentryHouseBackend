using System.Text.Json.Serialization;

namespace SentryHouseBackend.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string? NombreYCargo { get; set; }

        [JsonIgnore]
        public ICollection<MateriaPrima>? MateriasPrimas { get; set; }
    }

}
