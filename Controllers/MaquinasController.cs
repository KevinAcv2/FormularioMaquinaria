using FormularioMaquinaria.Models;
using Maquinarias.Data;
using Maquinarias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinarias.Controllers
{
    public class MaquinasController : Controller
    {
        private readonly AppDbContext _context;

        public MaquinasController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(
                await _context.Maquinas
                .OrderBy(x => x.Nombre)
                .ToListAsync());
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                _context.Maquinas.Add(maquina);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(maquina);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var maquina =
                await _context.Maquinas.FindAsync(id);

            if (maquina == null)
            {
                return NotFound();
            }

            return View(maquina);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                _context.Maquinas.Update(maquina);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(maquina);
        }
        public async Task<IActionResult> Eliminar(int id)
        {
            var maquina =
                await _context.Maquinas.FindAsync(id);

            if (maquina != null)
            {
                _context.Maquinas.Remove(maquina);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}