namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class ConfiguracionNegocio
    {
        public Guid Id { get; set; }

        public string NombreComercial { get; set; } = null!;
        public string? RazonSocial { get; set; }
        public string? Rfc { get; set; }

        public string? NumeroTelefonico { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? Direccion { get; set; }

        public string? UrlLogo { get; set; }

        public string Moneda { get; set; } = "MXN";
        public decimal PorcentajeImpuesto { get; set; } = 16.00m;

        public bool ImprimirTicketAutomaticamente { get; set; }
        public bool ControlarInventario { get; set; } = true;
        public bool MostrarAlertasStock { get; set; } = true;

        public bool SolicitarClienteVenta { get; set; }

        public bool Estatus { get; set; } = true;

        public DateTime AltaSistema { get; set; } = DateTime.Now;
        public DateTime? ModificacionSistema { get; set; }
    }
}
