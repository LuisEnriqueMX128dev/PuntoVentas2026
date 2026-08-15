namespace BibliotecaPuntoVentas.ViewModels.Productos
{
    public class CategoriaProductoViewModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Estatus { get; set; }
        public int TotalProductos { get; set; }

        public bool SePuedeEliminar => TotalProductos == 0;
    }
}
