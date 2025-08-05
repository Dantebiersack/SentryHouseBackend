using System.Text.Json.Serialization;

namespace SentryHouseBackend.Models
{
    public class MateriaPrima
    {
        public int Id { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoTotal { get; set; }

        public int ProveedorId { get; set; }
        //[JsonIgnore]
        public Proveedor? Proveedor { get; set; }
    }

}
