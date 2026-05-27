using Maquinarias.Data;
using Maquinarias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinarias.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<IActionResult> Index(
            string buscar,
            string tipoMaquina)
        {
            var reportes = _context.ReportesMaquinaria.AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
            {
                reportes = reportes.Where(x =>
                    x.NombreOperador.Contains(buscar) ||
                    x.NombreMaquina.Contains(buscar));
            }

            if (!string.IsNullOrEmpty(tipoMaquina))
            {
                reportes = reportes.Where(x =>
                    x.TipoMaquina == tipoMaquina);
            }

            return View(await reportes
                .OrderByDescending(x => x.Fecha)
                .ToListAsync());
        }

        // EDITAR - CARGAR FORMULARIO
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var reporte =
                await _context.ReportesMaquinaria.FindAsync(id);

            if (reporte == null)
            {
                return NotFound();
            }

            return View(reporte);
        }

        // EDITAR - GUARDAR CAMBIOS
        [HttpPost]
        public async Task<IActionResult> Editar(
            ReporteMaquinaria reporte)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reporte);

                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] =
                        "Reporte actualizado correctamente";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            return View(reporte);
        }

        // ELIMINAR
        public async Task<IActionResult> Eliminar(int id)
        {
            var reporte =
                await _context.ReportesMaquinaria.FindAsync(id);

            if (reporte != null)
            {
                _context.ReportesMaquinaria.Remove(reporte);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}