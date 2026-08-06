using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibliotecaPuntoVentas.ViewModels.Productos
{
    public class ProductoIndexViewModel
    {
        public string? Busqueda { get; set; }

        public Guid? CategoriaId { get; set; }

        public bool? Estatus { get; set; }

        public List<ProductoListadoViewModel> Productos { get; set; } = [];

        public List<SelectListItem> Categorias { get; set; } = [];
    }
}
