namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class VentaDetalleViewModel
    {
        public Guid Id { get; set; }

        public string Folio { get; set; } = null!;

        public string Cliente { get; set; } = "Público general";

        public string? TelefonoCliente { get; set; }

        public string? CorreoCliente { get; set; }

        public string Usuario { get; set; } = null!;

        public string Caja { get; set; } = null!;

        public decimal Subtotal { get; set; }

        public decimal Descuento { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Total { get; set; }

        public bool Cancelada { get; set; }

        public string? MotivoCancelacion { get; set; }

        public DateTime FechaVenta { get; set; }

        public List<VentaDetalleProductoViewModel> Productos { get; set; }
            = [];

        public List<VentaDetallePagoViewModel> Pagos { get; set; }
            = [];
    }
}
