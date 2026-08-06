namespace BibliotecaPuntoVentas.ViewModels.Reportes
{
    public class ReporteVentaDiariaViewModel
    {
        public DateTime Fecha { get; set; }

        public int CantidadVentas { get; set; }

        public decimal Total { get; set; }

        public decimal Descuentos { get; set; }

        public decimal Impuestos { get; set; }
    }
}
