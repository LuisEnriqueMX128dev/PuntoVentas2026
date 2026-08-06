using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class RegistrarVentaDetalleViewModel
    {
        [Required(ErrorMessage = "El producto es obligatorio.")]
        public Guid ProductoId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        [Range(
            0,
            999999999,
            ErrorMessage = "El descuento no es válido.")]
        public decimal Descuento { get; set; }
    }
}
