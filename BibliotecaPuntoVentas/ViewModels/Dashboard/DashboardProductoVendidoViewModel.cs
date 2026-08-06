namespace BibliotecaPuntoVentas.ViewModels.Dashboard
{
    public class DashboardProductoVendidoViewModel
    {
        public Guid ProductoId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public int CantidadVendida { get; set; }

        public decimal TotalVendido { get; set; }

        public int Existencia { get; set; }

        public int StockMinimo { get; set; }

        public bool TieneStockBajo =>
            Existencia > 0 &&
            Existencia <= StockMinimo;

        public bool EstaAgotado =>
            Existencia <= 0;
    }
}
