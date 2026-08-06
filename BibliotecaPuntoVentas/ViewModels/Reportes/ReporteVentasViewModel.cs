namespace BibliotecaPuntoVentas.ViewModels.Reportes
{
    public class ReporteVentasViewModel
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public decimal TotalVentas { get; set; }

        public decimal TotalDescuentos { get; set; }

        public decimal TotalImpuestos { get; set; }

        public decimal GananciaEstimada { get; set; }

        public int CantidadVentas { get; set; }

        public int CantidadProductosVendidos { get; set; }

        public decimal TicketPromedio =>
            CantidadVentas > 0
                ? TotalVentas / CantidadVentas
                : 0;

        public List<ReporteVentaDiariaViewModel> VentasDiarias { get; set; }
            = [];

        public List<ReporteProductoVendidoViewModel> ProductosMasVendidos { get; set; }
            = [];

        public List<ReporteMetodoPagoViewModel> MetodosPago { get; set; }
            = [];
    }
}
