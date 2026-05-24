using Maquinarias.Data;
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

        public async Task<IActionResult> Index(
            string buscar,
            string tipoMaquina)
        {
            var reportes = _context.ReportesMaquinaria.AsQueryable();

            // BUSCADOR
            if (!string.IsNullOrEmpty(buscar))
            {
                reportes = reportes.Where(x =>
                    x.NombreOperador.Contains(buscar) ||
                    x.NombreMaquina.Contains(buscar));
            }

            // FILTRO TIPO MAQUINA
            if (!string.IsNullOrEmpty(tipoMaquina))
            {
                reportes = reportes.Where(x =>
                    x.TipoMaquina == tipoMaquina);
            }

            return View(await reportes
                .OrderByDescending(x => x.Fecha)
                .ToListAsync());
        }
    }
}