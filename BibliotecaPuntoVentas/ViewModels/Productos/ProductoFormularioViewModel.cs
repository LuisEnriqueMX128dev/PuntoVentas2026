using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Productos
{
    public class ProductoFormularioViewModel
    {
        public Guid? Id { get; set; }

        [Required(
            ErrorMessage = "Debes seleccionar una categoría.")]
        public Guid CategoriaProductoId { get; set; }

        [Required(
            ErrorMessage = "El código es obligatorio.")]
        [StringLength(
            50,
            ErrorMessage = "El código no puede superar los 50 caracteres.")]
        public string Codigo { get; set; } = null!;

        [Required(
            ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(
            150,
            ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(
            500,
            ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(
            0,
            999999999,
            ErrorMessage = "El precio de compra no es válido.")]
        public decimal PrecioCompra { get; set; }

        [Range(
            0.01,
            999999999,
            ErrorMessage = "El precio de venta debe ser mayor a cero.")]
        public decimal PrecioVenta { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "La existencia no puede ser negativa.")]
        public int Existencia { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int StockMinimo { get; set; }

        public IFormFile? Foto { get; set; }

        public string? UrlImagenActual { get; set; }

        public bool EliminarImagen { get; set; }

        public bool Estatus { get; set; } = true;

        public List<SelectListItem> Categorias { get; set; } = [];
    }
}
