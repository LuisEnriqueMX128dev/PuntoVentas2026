namespace BibliotecaPuntoVentas.ViewModels.Shared
{
    public class ResumenTarjetaViewModel
    {
        public string Titulo { get; set; } = null!;
        public string Valor { get; set; } = null!;
        public string? Subtitulo { get; set; }
        public string TipoIcono { get; set; } = "ventas";
        public string TipoColor { get; set; } = "primary";
    }
}
