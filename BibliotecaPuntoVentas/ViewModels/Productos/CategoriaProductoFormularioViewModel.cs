using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Productos
{
    public class CategoriaProductoFormularioViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }

        public bool Estatus { get; set; } = true;
    }
}
