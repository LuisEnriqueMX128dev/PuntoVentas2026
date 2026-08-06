using BibliotecaPuntoVentas.Helpers;
using BibliotecaPuntoVentas.Service;
using BibliotecaPuntoVentas.ViewModels.Inventario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaPuntoVentas.Controllers
{
    public class InventarioController : Controller
    {
        private readonly INovaPosService _novaPosService;

        public InventarioController(INovaPosService novaPosService)
        {
            _novaPosService = novaPosService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? busqueda)
        {
            var model = await _novaPosService.ObtenerInventarioAsync(busqueda);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarMovimiento(
            MovimientoInventarioFormularioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["MensajeError"] = "Verifica los datos del movimiento.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (model.TipoMovimiento == SistemaConstantes.MovimientoEntrada)
                {
                    await _novaPosService.RegistrarEntradaInventarioAsync(model);
                }
                else
                {
                    await _novaPosService.RegistrarAjusteInventarioAsync(model);
                }

                TempData["MensajeExito"] = "El inventario se actualizó correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["MensajeError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
