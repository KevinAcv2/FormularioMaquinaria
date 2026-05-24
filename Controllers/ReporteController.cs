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

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ReporteMaquinaria reporte)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Asignar fecha en UTC para PostgreSQL
                    reporte.Fecha = DateTime.UtcNow;

                    _context.ReportesMaquinaria.Add(reporte);

                    await _context.SaveChangesAsync();

                    ViewBag.Mensaje = "Reporte enviado correctamente";

                    // Limpiar formulario
                    ModelState.Clear();

                    return View(new ReporteMaquinaria());
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            return View(reporte);
        }
    }
}