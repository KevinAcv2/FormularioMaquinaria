using FormularioMaquinaria.Models;
using Maquinarias.Data;
using Maquinarias.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinaria.Controllers
{
    public class OperadoresController : Controller
    {
        private readonly AppDbContext _context;

        public OperadoresController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Operador = new Operador();

            return View(
                await _context.Operadores
                .OrderBy(x => x.Nombre)
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Operador operador)
        {
            if (ModelState.IsValid)
            {
                _context.Operadores.Add(operador);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var operador =
                await _context.Operadores.FindAsync(id);

            if (operador == null)
            {
                return NotFound();
            }
            return View(operador);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Operador operador)
        {
            if (ModelState.IsValid)
            {
                _context.Operadores.Update(operador);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(operador);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var operador =
                await _context.Operadores.FindAsync(id);

            if (operador != null)
            {
                _context.Operadores.Remove(operador);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}