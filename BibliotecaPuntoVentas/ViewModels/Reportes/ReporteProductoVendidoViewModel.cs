namespace BibliotecaPuntoVentas.ViewModels.Reportes
{
    public class ReporteProductoVendidoViewModel
    {
        public Guid ProductoId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public int CantidadVendida { get; set; }

        public decimal TotalVendido { get; set; }

        public decimal CostoEstimado { get; set; }

        public decimal GananciaEstimada =>
            TotalVendido - CostoEstimado;
    }
}
