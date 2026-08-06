using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class CierreCajaViewModel
    {
        [Required(ErrorMessage = "La caja es obligatoria.")]
        public Guid CajaId { get; set; }

        public string? Folio { get; set; }

        public decimal MontoInicial { get; set; }

        public decimal TotalVentas { get; set; }

        public decimal TotalEfectivo { get; set; }

        public decimal TotalTarjeta { get; set; }

        public decimal TotalTransferencia { get; set; }

        public decimal EfectivoEsperado { get; set; }

        [Range(
            0,
            999999999,
            ErrorMessage = "El efectivo contado no es válido.")]
        public decimal EfectivoContado { get; set; }

        public decimal Diferencia =>
            EfectivoContado - EfectivoEsperado;

        [StringLength(
            500,
            ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}
