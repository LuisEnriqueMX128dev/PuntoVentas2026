using BibliotecaPuntoVentas.Models.Seguridad;

namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class MovimientoInventario
    {
        public Guid Id { get; set; }

        public Guid ProductoId { get; set; }
        public string UsuarioId { get; set; } = null!;

        public string TipoMovimiento { get; set; } = null!;

        public int Cantidad { get; set; }
        public int ExistenciaAnterior { get; set; }
        public int ExistenciaNueva { get; set; }

        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }

        public DateTime FechaMovimiento { get; set; } = DateTime.Now;
        public DateTime AltaSistema { get; set; } = DateTime.Now;

        // Relaciones
        public Producto? Producto { get; set; }
        public ApplicationUser? Usuario { get; set; }
    }
}
