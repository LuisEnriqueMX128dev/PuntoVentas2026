namespace BibliotecaPuntoVentas.ViewModels.Inventario
{
    public class MovimientoInventarioViewModel
    {
        public Guid Id { get; set; }

        public Guid ProductoId { get; set; }

        public string CodigoProducto { get; set; } = null!;

        public string NombreProducto { get; set; } = null!;

        public string TipoMovimiento { get; set; } = null!;

        public int Cantidad { get; set; }

        public int ExistenciaAnterior { get; set; }

        public int ExistenciaNueva { get; set; }

        public string? Referencia { get; set; }

        public string? Observaciones { get; set; }

        public string? NombreUsuario { get; set; }

        public DateTime FechaMovimiento { get; set; }
    }
}
