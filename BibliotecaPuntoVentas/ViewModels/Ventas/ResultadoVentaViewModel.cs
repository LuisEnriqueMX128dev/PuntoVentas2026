namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class ResultadoVentaViewModel
    {
        public bool Exitoso { get; set; }

        public Guid? VentaId { get; set; }

        public string? Folio { get; set; }

        public decimal Total { get; set; }

        public decimal MontoRecibido { get; set; }

        public decimal Cambio { get; set; }

        public string Mensaje { get; set; } = null!;

        public List<string> Errores { get; set; }
            = [];
    }
}
