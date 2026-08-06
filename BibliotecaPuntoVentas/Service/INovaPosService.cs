using BibliotecaPuntoVentas.ViewModels.Clientes;
using BibliotecaPuntoVentas.ViewModels.Configuracion;
using BibliotecaPuntoVentas.ViewModels.Dashboard;
using BibliotecaPuntoVentas.ViewModels.Inventario;
using BibliotecaPuntoVentas.ViewModels.Productos;
using BibliotecaPuntoVentas.ViewModels.Reportes;
using BibliotecaPuntoVentas.ViewModels.Ventas;

namespace BibliotecaPuntoVentas.Service
{
    public interface INovaPosService
    {
        Task<DashboardViewModel> ObtenerDashboardAsync();

        Task<List<CategoriaProductoViewModel>> ObtenerCategoriasAsync();
        Task<CategoriaProductoViewModel?> ObtenerCategoriaPorIdAsync(Guid categoriaId);
        Task<Guid> CrearCategoriaAsync(CategoriaProductoFormularioViewModel model);
        Task<bool> EditarCategoriaAsync(CategoriaProductoFormularioViewModel model);
        Task<bool> CambiarEstatusCategoriaAsync(Guid categoriaId);

        Task<List<ProductoListadoViewModel>> ObtenerProductosAsync(
            string? busqueda = null,
            Guid? categoriaId = null,
            bool? estatus = null);

        Task<ProductoFormularioViewModel?> ObtenerProductoPorIdAsync(Guid productoId);
        Task<Guid> CrearProductoAsync(ProductoFormularioViewModel model, string? usuarioId = null);
        Task<bool> EditarProductoAsync(ProductoFormularioViewModel model);
        Task<bool> CambiarEstatusProductoAsync(Guid productoId);
        Task<bool> ExisteCodigoProductoAsync(string codigo, Guid? productoId = null);

        Task<InventarioIndexViewModel> ObtenerInventarioAsync(string? busqueda = null);
        Task<List<MovimientoInventarioViewModel>> ObtenerMovimientosInventarioAsync(Guid? productoId = null);
        Task<bool> RegistrarEntradaInventarioAsync(
            MovimientoInventarioFormularioViewModel model,
            string? usuarioId = null);
        Task<bool> RegistrarAjusteInventarioAsync(
            MovimientoInventarioFormularioViewModel model,
            string? usuarioId = null);

        Task<List<ClienteListadoViewModel>> ObtenerClientesAsync(string? busqueda = null);
        Task<ClienteFormularioViewModel?> ObtenerClientePorIdAsync(Guid clienteId);
        Task<Guid> CrearClienteAsync(ClienteFormularioViewModel model);
        Task<bool> EditarClienteAsync(ClienteFormularioViewModel model);
        Task<bool> CambiarEstatusClienteAsync(Guid clienteId);

        Task<PuntoVentaViewModel> ObtenerPuntoVentaAsync(
            string? busqueda = null,
            Guid? categoriaId = null);

        Task<ProductoPuntoVentaViewModel?> ObtenerProductoPorCodigoAsync(string codigo);
        Task<ResultadoVentaViewModel> RegistrarVentaAsync(
            RegistrarVentaViewModel model,
            string? usuarioId = null);

        Task<List<VentaListadoViewModel>> ObtenerVentasAsync(
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null);

        Task<VentaDetalleViewModel?> ObtenerDetalleVentaAsync(Guid ventaId);
        Task<bool> CancelarVentaAsync(
            Guid ventaId,
            string motivo,
            string? usuarioId = null);

        Task<CajaViewModel?> ObtenerCajaAbiertaAsync(string? usuarioId = null);
        Task<Guid> AbrirCajaAsync(
            AperturaCajaViewModel model,
            string? usuarioId = null);
        Task<bool> CerrarCajaAsync(
            CierreCajaViewModel model,
            string? usuarioId = null);

        Task<ReporteVentasViewModel> ObtenerReporteVentasAsync(
            DateTime fechaInicio,
            DateTime fechaFin);
        Task<ReporteInventarioViewModel> ObtenerReporteInventarioAsync();

        Task<ConfiguracionNegocioViewModel> ObtenerConfiguracionAsync();
        Task<bool> GuardarConfiguracionAsync(ConfiguracionNegocioViewModel model);
    }
}
