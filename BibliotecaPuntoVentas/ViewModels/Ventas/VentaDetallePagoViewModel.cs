namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class VentaDetallePagoViewModel
    {
        public Guid PagoId { get; set; }

        public string MetodoPago { get; set; } = null!;

        public decimal Monto { get; set; }

        public decimal MontoRecibido { get; set; }

        public decimal Cambio { get; set; }

        public string? Referencia { get; set; }

        public DateTime FechaPago { get; set; }
    }
}
