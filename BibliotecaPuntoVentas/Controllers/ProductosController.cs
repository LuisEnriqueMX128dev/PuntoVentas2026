using BibliotecaPuntoVentas.Service;
using BibliotecaPuntoVentas.ViewModels.Productos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibliotecaPuntoVentas.Controllers
{
    public class ProductosController : Controller
    {
        private readonly INovaPosService
            _novaPosService;

        public ProductosController(
            INovaPosService novaPosService)
        {
            _novaPosService =
                novaPosService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? busqueda,
            Guid? categoriaId,
            bool? estatus)
        {
            var categorias =
                await _novaPosService
                    .ObtenerCategoriasAsync();

            var model =
                new ProductoIndexViewModel
                {
                    Busqueda = busqueda,
                    CategoriaId = categoriaId,
                    Estatus = estatus,

                    Productos =
                        await _novaPosService
                            .ObtenerProductosAsync(
                                busqueda,
                                categoriaId,
                                estatus),

                    Categorias = categorias
                        .Where(c => c.Estatus)
                        .Select(c =>
                            new SelectListItem
                            {
                                Value =
                                    c.Id.ToString(),

                                Text =
                                    c.Nombre,

                                Selected =
                                    categoriaId ==
                                    c.Id
                            })
                        .ToList()
                };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var model =
                new ProductoFormularioViewModel
                {
                    Estatus = true,
                    StockMinimo = 5
                };

            await CargarCategoriasAsync(
                model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            ProductoFormularioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync(
                    model);

                return View(model);
            }

            try
            {
                await _novaPosService
                    .CrearProductoAsync(model);

                TempData["MensajeExito"] =
                    "El producto se registró correctamente.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No fue posible registrar el producto.");
            }

            await CargarCategoriasAsync(
                model);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(
            Guid id)
        {
            var model =
                await _novaPosService
                    .ObtenerProductoPorIdAsync(id);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            ProductoFormularioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync(
                    model);

                return View(model);
            }

            try
            {
                var resultado =
                    await _novaPosService
                        .EditarProductoAsync(
                            model);

                if (!resultado)
                {
                    return NotFound();
                }

                TempData["MensajeExito"] =
                    "El producto se actualizó correctamente.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No fue posible actualizar el producto.");
            }

            await CargarCategoriasAsync(
                model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            CambiarEstatus(Guid id)
        {
            var resultado =
                await _novaPosService
                    .CambiarEstatusProductoAsync(id);

            if (!resultado)
            {
                return NotFound();
            }

            TempData["MensajeExito"] =
                "El estado del producto se actualizó.";

            return RedirectToAction(
                nameof(Index));
        }

        private async Task CargarCategoriasAsync(
            ProductoFormularioViewModel model)
        {
            var categorias =
                await _novaPosService
                    .ObtenerCategoriasAsync();

            model.Categorias =
                categorias
                    .Where(c => c.Estatus)
                    .Select(c =>
                        new SelectListItem
                        {
                            Value =
                                c.Id.ToString(),

                            Text =
                                c.Nombre,

                            Selected =
                                c.Id ==
                                model.CategoriaProductoId
                        })
                    .ToList();
        }

    }
}
