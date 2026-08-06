using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaPuntoVentas.ViewModels.Inventario
{
    public class MovimientoInventarioFormularioViewModel
    {
        [Required(ErrorMessage = "Debes seleccionar un producto.")]
        public Guid ProductoId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un tipo de movimiento.")]
        public string TipoMovimiento { get; set; } = null!;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        [StringLength(
            100,
            ErrorMessage = "La referencia no puede superar los 100 caracteres.")]
        public string? Referencia { get; set; }

        [StringLength(
            500,
            ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }

        public List<SelectListItem> Productos { get; set; }
            = [];

        public List<SelectListItem> TiposMovimiento { get; set; }
            = [];
    }
}
