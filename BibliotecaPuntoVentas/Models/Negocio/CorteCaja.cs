using BibliotecaPuntoVentas.Models.Seguridad;

namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class CorteCaja
    {
        public Guid Id { get; set; }

        public Guid CajaId { get; set; }
        public string UsuarioId { get; set; } = null!;

        public decimal MontoInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTarjeta { get; set; }
        public decimal TotalTransferencia { get; set; }

        public decimal EfectivoEsperado { get; set; }
        public decimal EfectivoContado { get; set; }
        public decimal Diferencia { get; set; }

        public string? Observaciones { get; set; }

        public DateTime FechaCorte { get; set; } = DateTime.Now;
        public DateTime AltaSistema { get; set; } = DateTime.Now;

        // Relaciones
        public Caja? Caja { get; set; }
        public ApplicationUser? Usuario { get; set; }
    }
}
