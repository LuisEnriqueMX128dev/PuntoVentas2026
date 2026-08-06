using BibliotecaPuntoVentas.Models.Seguridad;

namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class Venta
    {
        public Guid Id { get; set; }

        public Guid? ClienteId { get; set; }
        public Guid CajaId { get; set; }

        public string UsuarioId { get; set; } = null!;

        public string Folio { get; set; } = null!;

        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }

        public bool Cancelada { get; set; }
        public string? MotivoCancelacion { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;
        public DateTime AltaSistema { get; set; } = DateTime.Now;

        // Relaciones
        public Cliente? Cliente { get; set; }
        public Caja? Caja { get; set; }
        public ApplicationUser? Usuario { get; set; }

        public ICollection<DetalleVenta>? DetallesVenta { get; set; }
        public ICollection<Pago>? Pagos { get; set; }
    }
}
