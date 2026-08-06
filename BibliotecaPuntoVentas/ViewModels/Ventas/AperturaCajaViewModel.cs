using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class AperturaCajaViewModel
    {
        [Range(
            0,
            999999999,
            ErrorMessage = "El monto inicial no es válido.")]
        public decimal MontoInicial { get; set; }

        [StringLength(
            500,
            ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}
