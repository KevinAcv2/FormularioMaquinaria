using Maquinarias.Data;
using Maquinarias.Models;
using Microsoft.AspNetCore.Mvc;

namespace Maquinarias.Controllers
{
    public class ReporteController : Controller
    {
        private readonly AppDbContext _context;

        public ReporteController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ReporteMaquinaria reporte)
        {
            if (ModelState.IsValid)
            {
                _context.ReportesMaquinaria.Add(reporte);

                await _context.SaveChangesAsync();

                ViewBag.Mensaje = "Reporte enviado correctamente";
            }

            return View();
        }
    }
}
