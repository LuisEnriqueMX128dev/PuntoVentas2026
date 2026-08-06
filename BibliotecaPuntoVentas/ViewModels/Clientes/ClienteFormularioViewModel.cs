using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Clientes
{
    public class ClienteFormularioViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(
            100,
            ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(
            100,
            ErrorMessage = "El apellido paterno no puede superar los 100 caracteres.")]
        public string? ApellidoPaterno { get; set; }

        [StringLength(
            100,
            ErrorMessage = "El apellido materno no puede superar los 100 caracteres.")]
        public string? ApellidoMaterno { get; set; }

        [Phone(ErrorMessage = "El número telefónico no es válido.")]
        [StringLength(
            20,
            ErrorMessage = "El número telefónico no puede superar los 20 caracteres.")]
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

        public bool Estatus { get; set; } = true;
    }
}
