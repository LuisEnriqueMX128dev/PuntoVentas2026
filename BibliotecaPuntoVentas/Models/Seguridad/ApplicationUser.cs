using BibliotecaPuntoVentas.Models.Negocio;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaPuntoVentas.Models.Seguridad
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }

        public bool Estatus { get; set; } = true;

        public DateTime AltaSistema { get; set; } = DateTime.Now;
        public DateTime? ModificacionSistema { get; set; }

        // Relaciones
        public ICollection<Venta>? Ventas { get; set; }
        public ICollection<MovimientoInventario>? MovimientosInventario { get; set; }
        public ICollection<Caja>? CajasAperturadas { get; set; }
        public ICollection<CorteCaja>? CortesCaja { get; set; }
    }
}
