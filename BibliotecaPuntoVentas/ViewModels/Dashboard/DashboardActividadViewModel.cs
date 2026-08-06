namespace BibliotecaPuntoVentas.ViewModels.Dashboard
{
    public class DashboardActividadViewModel
    {
        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }

        public string TipoActividad { get; set; } = null!;

        public DateTime Fecha { get; set; }

        public string? Referencia { get; set; }
    }
}
