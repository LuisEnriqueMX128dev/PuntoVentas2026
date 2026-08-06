namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class DetalleVenta
    {
        public Guid Id { get; set; }

        public Guid VentaId { get; set; }
        public Guid ProductoId { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
        public decimal Subtotal { get; set; }

        public DateTime AltaSistema { get; set; } = DateTime.Now;

        // Relaciones
        public Venta? Venta { get; set; }
        public Producto? Producto { get; set; }
    }
}
