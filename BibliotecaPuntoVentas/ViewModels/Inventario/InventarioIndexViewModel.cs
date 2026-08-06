namespace BibliotecaPuntoVentas.ViewModels.Inventario
{
    public class InventarioIndexViewModel
    {
        public string? Busqueda { get; set; }

        public decimal ValorTotalInventario { get; set; }

        public int TotalUnidades { get; set; }

        public int TotalProductos { get; set; }

        public int ProductosStockBajo { get; set; }

        public int ProductosAgotados { get; set; }

        public List<InventarioProductoViewModel> Productos { get; set; }
            = [];

        public List<MovimientoInventarioViewModel> UltimosMovimientos { get; set; }
            = [];
    }
}
