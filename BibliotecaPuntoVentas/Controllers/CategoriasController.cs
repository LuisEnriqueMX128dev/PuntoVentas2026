using BibliotecaPuntoVentas.Service;
using BibliotecaPuntoVentas.ViewModels.Productos;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaPuntoVentas.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly INovaPosService _novaPosService;

        public CategoriasController(INovaPosService novaPosService)
        {
            _novaPosService = novaPosService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _novaPosService.ObtenerCategoriasAsync();

            return View(model);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var model = new CategoriaProductoFormularioViewModel
            {
                Estatus = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CategoriaProductoFormularioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _novaPosService.CrearCategoriaAsync(model);

                TempData["MensajeExito"] = "La categoría se registró correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "No fue posible registrar la categoría.");

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var model = await _novaPosService.ObtenerCategoriaPorIdAsync(id);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(CategoriaProductoFormularioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var resultado = await _novaPosService.EditarCategoriaAsync(model);

                if (!resultado)
                {
                    return NotFound();
                }

                TempData["MensajeExito"] = "La categoría se actualizó correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "No fue posible actualizar la categoría.");

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstatus(Guid id)
        {
            var resultado = await _novaPosService.CambiarEstatusCategoriaAsync(id);

            if (!resultado)
            {
                return NotFound();
            }

            TempData["MensajeExito"] = "El estado de la categoría se actualizó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            try
            {
                var resultado = await _novaPosService.EliminarCategoriaAsync(id);

                if (!resultado)
                {
                    return NotFound();
                }

                TempData["MensajeExito"] = "La categoría se eliminó correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["MensajeError"] = ex.Message;
            }
            catch
            {
                TempData["MensajeError"] = "No fue posible eliminar la categoría.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
