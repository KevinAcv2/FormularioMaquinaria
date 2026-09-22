using Maquinarias.Data;
using Maquinarias.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Maquinarias.Controllers
{
    public class FrenteOperacionalController : Controller
    {
        private readonly AppDbContext _context;

        public FrenteOperacionalController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(FrenteOperacional frente)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Los datos del frente operacional no son válidos.";
                return RedirectToAction("Index", "Operadores");
            }

            if (string.IsNullOrWhiteSpace(frente.Nombre))
            {
                TempData["Error"] = "El nombre del frente es obligatorio.";
                return RedirectToAction("Index", "Operadores");
            }

            frente.Nombre = frente.Nombre.Trim();

            _context.FrentesOperacionales.Add(frente);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Frente operacional agregado correctamente.";
            return RedirectToAction("Index", "Operadores");
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var frente = _context.FrentesOperacionales.Find(id);

            if (frente != null)
            {
                var operadoresAsociados = _context.Operadores
                    .Where(o => o.FrenteOperacionalId == id)
                    .ToList();

                foreach (var operador in operadoresAsociados)
                {
                    operador.FrenteOperacionalId = null;
                }

                _context.FrentesOperacionales.Remove(frente);

                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Operadores");
        }
    }
}