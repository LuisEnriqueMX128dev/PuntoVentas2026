namespace BibliotecaPuntoVentas.ViewModels.Reportes
{
    public class ReporteMetodoPagoViewModel
    {
        public Guid MetodoPagoId { get; set; }

        public string MetodoPago { get; set; } = null!;

        public int CantidadPagos { get; set; }

        public decimal Total { get; set; }

        public decimal Porcentaje { get; set; }
    }
}
