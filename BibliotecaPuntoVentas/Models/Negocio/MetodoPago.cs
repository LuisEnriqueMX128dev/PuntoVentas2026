namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class MetodoPago
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        public bool Estatus { get; set; } = true;

        public DateTime AltaSistema { get; set; } = DateTime.Now;

        // Relaciones
        public ICollection<Pago>? Pagos { get; set; }
    }
}
