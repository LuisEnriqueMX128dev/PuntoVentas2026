namespace BibliotecaPuntoVentas.ViewModels.Reportes
{
    public class ReporteInventarioProductoViewModel
    {
        public Guid ProductoId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public int Existencia { get; set; }

        public int StockMinimo { get; set; }

        public decimal PrecioCompra { get; set; }

        public decimal PrecioVenta { get; set; }

        public decimal ValorCompra =>
            PrecioCompra * Existencia;

        public decimal ValorVenta =>
            PrecioVenta * Existencia;

        public string Estado
        {
            get
            {
                if (Existencia <= 0)
                {
                    return "Agotado";
                }

                if (Existencia <= StockMinimo)
                {
                    return "Stock bajo";
                }

                return "Disponible";
            }
        }
    }
}
