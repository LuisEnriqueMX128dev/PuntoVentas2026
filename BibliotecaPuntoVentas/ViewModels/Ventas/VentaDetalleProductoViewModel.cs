namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class VentaDetalleProductoViewModel
    {
        public Guid ProductoId { get; set; }

        public string Codigo { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Descuento { get; set; }

        public decimal Subtotal { get; set; }
    }
}
