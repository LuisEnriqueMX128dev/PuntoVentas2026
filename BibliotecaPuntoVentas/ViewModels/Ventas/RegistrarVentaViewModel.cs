using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class RegistrarVentaViewModel
    {
        public Guid? ClienteId { get; set; }

        [Required(ErrorMessage = "No existe una caja abierta.")]
        public Guid CajaId { get; set; }

        [Range(
            0,
            999999999,
            ErrorMessage = "El descuento no es válido.")]
        public decimal Descuento { get; set; }

        public string? Observaciones { get; set; }

        [MinLength(
            1,
            ErrorMessage = "Debes agregar por lo menos un producto.")]
        public List<RegistrarVentaDetalleViewModel> Detalles { get; set; }
            = [];

        [MinLength(
            1,
            ErrorMessage = "Debes registrar al menos un pago.")]
        public List<RegistrarPagoViewModel> Pagos { get; set; }
            = [];
    }
}
