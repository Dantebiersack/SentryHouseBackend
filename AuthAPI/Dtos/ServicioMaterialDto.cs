namespace SentryHouseBackend.Dtos
{
    public class ServicioMaterialDto
    {
        public int MateriaPrimaId { get; set; }
        public decimal CantidadRequerida { get; set; }
        public string? Unidad { get; set; }
    }

    public class ServicioCreateUpdateDto
    {
        public int? Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ArchivoDocumento { get; set; }
        public decimal? PrecioBase { get; set; }

        public List<ServicioMaterialDto> Materiales { get; set; } = new();
    }

    // Este es el nuevo DTO que necesitas crear
    public class ServicioDetalleMaterialDto
    {
        public int MateriaPrimaId { get; set; }
        public string MateriaPrimaNombre { get; set; }
        public decimal CantidadRequerida { get; set; }
        public string? Unidad { get; set; }
    }

    public class ServicioDetailDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ArchivoDocumento { get; set; }
        public decimal? PrecioBase { get; set; }

        // La lista ahora usa el nuevo DTO para ser más clara
        public List<ServicioDetalleMaterialDto> Materiales { get; set; } = new();
    }
}