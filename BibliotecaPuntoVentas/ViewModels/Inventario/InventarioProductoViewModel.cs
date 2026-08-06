namespace BibliotecaPuntoVentas.ViewModels.Inventario
{
    public class InventarioProductoViewModel
    {
        public Guid ProductoId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public decimal PrecioCompra { get; set; }

        public decimal PrecioVenta { get; set; }

        public int Existencia { get; set; }

        public int StockMinimo { get; set; }

        public bool Estatus { get; set; }

        public decimal ValorInventario =>
            PrecioCompra * Existencia;

        public bool TieneStockBajo =>
            Existencia > 0 &&
            Existencia <= StockMinimo;

        public bool EstaAgotado =>
            Existencia <= 0;

        public string EstadoInventario
        {
            get
            {
                if (EstaAgotado)
                {
                    return "Agotado";
                }

                if (TieneStockBajo)
                {
                    return "Stock bajo";
                }

                return "Disponible";
            }
        }
    }
}
