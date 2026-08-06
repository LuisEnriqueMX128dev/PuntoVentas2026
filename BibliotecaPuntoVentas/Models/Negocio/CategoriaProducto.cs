namespace BibliotecaPuntoVentas.Models.Negocio
{
    public class CategoriaProducto
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        public bool Estatus { get; set; } = true;

        public DateTime AltaSistema { get; set; } = DateTime.Now;
        public DateTime? ModificacionSistema { get; set; }

        // Relaciones
        public ICollection<Producto>? Productos { get; set; }
    }
}
