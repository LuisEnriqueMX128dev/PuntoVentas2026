using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Configuracion
{
    public class ConfiguracionNegocioViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "El nombre comercial es obligatorio.")]
        [StringLength(
            150,
            ErrorMessage = "El nombre comercial no puede superar los 150 caracteres.")]
        public string NombreComercial { get; set; } = null!;

        [StringLength(
            200,
            ErrorMessage = "La razón social no puede superar los 200 caracteres.")]
        public string? RazonSocial { get; set; }

        [StringLength(
            20,
            ErrorMessage = "El RFC no puede superar los 20 caracteres.")]
        public string? Rfc { get; set; }

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(
            20,
            ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string? NumeroTelefonico { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(
            150,
            ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        public string? CorreoElectronico { get; set; }

        [StringLength(
            300,
            ErrorMessage = "La dirección no puede superar los 300 caracteres.")]
        public string? Direccion { get; set; }

        public string? UrlLogo { get; set; }

        [Required(ErrorMessage = "La moneda es obligatoria.")]
        [StringLength(10)]
        public string Moneda { get; set; } = "MXN";

        [Range(
            0,
            100,
            ErrorMessage = "El porcentaje de impuesto debe estar entre 0 y 100.")]
        public decimal PorcentajeImpuesto { get; set; } = 16.00m;

        public bool ImprimirTicketAutomaticamente { get; set; }

        public bool ControlarInventario { get; set; } = true;

        public bool MostrarAlertasStock { get; set; } = true;

        public bool SolicitarClienteVenta { get; set; }

        public bool Estatus { get; set; } = true;
    }
}
