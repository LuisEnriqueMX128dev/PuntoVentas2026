using BibliotecaPuntoVentas.Service;
using BibliotecaPuntoVentas.ViewModels.Ventas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaPuntoVentas.Controllers
{
    public class VentasController : Controller
    {
        private readonly INovaPosService _novaPosService;

        public VentasController(INovaPosService novaPosService)
        {
            _novaPosService = novaPosService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? busqueda,
            Guid? categoriaId)
        {
            var model = await _novaPosService.ObtenerPuntoVentaAsync(
                busqueda,
                categoriaId);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarProductoPorCodigo(string codigo)
        {
            var producto = await _novaPosService.ObtenerProductoPorCodigoAsync(codigo);

            if (producto is null)
            {
                return NotFound(new
                {
                    exitoso = false,
                    mensaje = "No se encontró un producto con ese código. Puedes buscarlo manualmente."
                });
            }

            if (producto.Existencia <= 0)
            {
                return BadRequest(new
                {
                    exitoso = false,
                    mensaje = "El producto está agotado."
                });
            }

            return Ok(new
            {
                exitoso = true,
                producto
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(
            [FromBody] RegistrarVentaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .ToList();

                return BadRequest(new
                {
                    exitoso = false,
                    mensaje = "La información de la venta no es válida.",
                    errores
                });
            }

            var resultado = await _novaPosService.RegistrarVentaAsync(model);

            if (!resultado.Exitoso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AbrirCaja(AperturaCajaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["MensajeError"] = "El monto inicial no es válido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _novaPosService.AbrirCajaAsync(model);
                TempData["MensajeExito"] = "La caja se abrió correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["MensajeError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
