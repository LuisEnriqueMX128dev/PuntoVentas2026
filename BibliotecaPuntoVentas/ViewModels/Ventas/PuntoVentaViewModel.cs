using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibliotecaPuntoVentas.ViewModels.Ventas
{
    public class PuntoVentaViewModel
    {
        public string? Busqueda { get; set; }

        public Guid? CategoriaId { get; set; }

        public Guid? ClienteId { get; set; }

        public Guid? CajaId { get; set; }

        public CajaViewModel? CajaAbierta { get; set; }
        public decimal PorcentajeImpuesto { get; set; } = 11.5m;
        public bool TieneCajaAbierta { get; set; }

        public List<ProductoPuntoVentaViewModel> Productos { get; set; }
            = [];

        public List<SelectListItem> Categorias { get; set; }
            = [];

        public List<SelectListItem> Clientes { get; set; }
            = [];

        public List<SelectListItem> MetodosPago { get; set; }
            = [];
    }
}
