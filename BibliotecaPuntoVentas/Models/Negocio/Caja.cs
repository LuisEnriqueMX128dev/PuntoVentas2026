using BibliotecaPuntoVentas.Models.Seguridad;

namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class Caja
    {
        public Guid Id { get; set; }

        public string UsuarioAperturaId { get; set; } = null!;

        public string Folio { get; set; } = null!;

        public decimal MontoInicial { get; set; }

        public DateTime FechaApertura { get; set; } = DateTime.Now;
        public DateTime? FechaCierre { get; set; }

        public bool Abierta { get; set; } = true;
        public bool Estatus { get; set; } = true;

        public DateTime AltaSistema { get; set; } = DateTime.Now;

        // Relaciones
        public ApplicationUser? UsuarioApertura { get; set; }

        public ICollection<Venta>? Ventas { get; set; }
        public ICollection<CorteCaja>? CortesCaja { get; set; }
    }
}
