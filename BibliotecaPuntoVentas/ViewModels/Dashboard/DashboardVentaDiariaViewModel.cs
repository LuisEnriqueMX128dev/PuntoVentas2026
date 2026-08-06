namespace BibliotecaPuntoVentas.ViewModels.Dashboard
{
    public class DashboardVentaDiariaViewModel
    {
        public DateTime Fecha { get; set; }

        public string Dia { get; set; } = null!;

        public decimal Total { get; set; }

        public int CantidadVentas { get; set; }
    }
}
