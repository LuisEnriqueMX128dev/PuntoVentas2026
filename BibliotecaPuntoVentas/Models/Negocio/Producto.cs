namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class Producto
    {
        public Guid Id { get; set; }

        public Guid CategoriaProductoId { get; set; }

        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }

        public int Existencia { get; set; }
        public int StockMinimo { get; set; }

        public string? UrlImagen { get; set; }

        public bool Estatus { get; set; } = true;

        public DateTime AltaSistema { get; set; } = DateTime.Now;
        public DateTime? ModificacionSistema { get; set; }

        // Relaciones
        public CategoriaProducto? CategoriaProducto { get; set; }

        public ICollection<DetalleVenta>? DetallesVenta { get; set; }
        public ICollection<MovimientoInventario>? MovimientosInventario { get; set; }
    }
}
