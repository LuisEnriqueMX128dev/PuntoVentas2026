using BibliotecaPuntoVentas.Models;
using BibliotecaPuntoVentas.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BibliotecaPuntoVentas.Controllers
{
    public class HomeController : Controller
    {
        private readonly INovaPosService _novaPosService;

        public HomeController(INovaPosService novaPosService)
        {
            _novaPosService = novaPosService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fechaInicio,DateTime? fechaFin)
        {

            var model = await _novaPosService.ObtenerDashboardAsync(fechaInicio,fechaFin);
            return View(model);
        }

    }
}
