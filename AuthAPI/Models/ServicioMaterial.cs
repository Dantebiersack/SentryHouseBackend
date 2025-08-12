using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SentryHouseBackend.Models
{
    public class ServicioMaterial
    {
        public int Id { get; set; }

        [Required]
        public int ServicioId { get; set; }

        [JsonIgnore] // evita ciclo Servicio -> Materiales -> Servicio
        public Servicio? Servicio { get; set; }

        [Required]
        public int MateriaPrimaId { get; set; }
        // navegación one-way; no agregamos colección en MateriaPrima para evitar ciclos
        public MateriaPrima? MateriaPrima { get; set; }

        [Required]
        public decimal CantidadRequerida { get; set; } // por 1 unidad de servicio
        public string? Unidad { get; set; }            // opcional (pz, m, l, etc.)
    }
}
