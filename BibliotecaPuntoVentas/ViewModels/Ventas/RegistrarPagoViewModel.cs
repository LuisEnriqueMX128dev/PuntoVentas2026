using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class RegistrarPagoViewModel
    {
        [Required(ErrorMessage = "Debes seleccionar un método de pago.")]
        public Guid MetodoPagoId { get; set; }

        [Range(
            0.01,
            999999999,
            ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Monto { get; set; }

        [Range(
            0,
            999999999,
            ErrorMessage = "El monto recibido no es válido.")]
        public decimal MontoRecibido { get; set; }

        public string? Referencia { get; set; }
    }
}
