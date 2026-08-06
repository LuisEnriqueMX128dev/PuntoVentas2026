namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class VentaListadoViewModel
    {
        public Guid Id { get; set; }

        public string Folio { get; set; } = null!;

        public string Cliente { get; set; } = "Público general";

        public string Usuario { get; set; } = null!;

        public string Caja { get; set; } = null!;

        public int CantidadProductos { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Descuento { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public bool Cancelada { get; set; }

        public DateTime FechaVenta { get; set; }

        public string Estado =>
            Cancelada
                ? "Cancelada"
                : "Pagada";
    }
}
