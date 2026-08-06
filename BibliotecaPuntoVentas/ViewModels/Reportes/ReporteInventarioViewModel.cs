namespace BibliotecaPuntoVentas.ViewModels.Reportes
{
    public class ReporteInventarioViewModel
    {
        public DateTime FechaGeneracion { get; set; }

        public int TotalProductos { get; set; }

        public int TotalUnidades { get; set; }

        public int ProductosDisponibles { get; set; }

        public int ProductosStockBajo { get; set; }

        public int ProductosAgotados { get; set; }

        public decimal ValorCompraInventario { get; set; }

        public decimal ValorVentaInventario { get; set; }

        public decimal GananciaPotencial =>
            ValorVentaInventario -
            ValorCompraInventario;

        public List<ReporteInventarioProductoViewModel> Productos { get; set; }
            = [];
    }
}
