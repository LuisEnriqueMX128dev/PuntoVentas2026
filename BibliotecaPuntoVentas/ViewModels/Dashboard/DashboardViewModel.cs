namespace BibliotecaPuntoVentas.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public decimal VentasHoy { get; set; }
        public decimal VentasAyer { get; set; }

        public int CantidadVentasHoy { get; set; }
        public int TotalProductos { get; set; }
        public int TotalClientes { get; set; }

        public int ProductosStockBajo { get; set; }
        public int ProductosAgotados { get; set; }

        public decimal PorcentajeCambioVentas { get; set; }

        public List<DashboardVentaDiariaViewModel> VentasUltimosDias { get; set; }
            = [];

        public List<DashboardProductoVendidoViewModel> ProductosMasVendidos { get; set; }
            = [];

        public List<DashboardActividadViewModel> ActividadesRecientes { get; set; }
            = [];
    }
}
