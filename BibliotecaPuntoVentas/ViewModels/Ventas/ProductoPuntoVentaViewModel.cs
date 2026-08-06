namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class ProductoPuntoVentaViewModel
    {
        public Guid Id { get; set; }

        public Guid CategoriaProductoId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public decimal PrecioVenta { get; set; }

        public int Existencia { get; set; }

        public string? UrlImagen { get; set; }

        public bool Disponible =>
            Existencia > 0;
    }
}
