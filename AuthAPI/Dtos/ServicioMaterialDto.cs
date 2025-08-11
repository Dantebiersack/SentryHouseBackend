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

        // BOM
        public List<ServicioMaterialDto> Materiales { get; set; } = new();
    }

    public class ServicioDetailDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ArchivoDocumento { get; set; }
        public decimal? PrecioBase { get; set; }
        public List<(int materiaPrimaId, string materiaPrimaNombre, decimal cantidad, string? unidad)> Materiales { get; set; } = new();
    }
}
