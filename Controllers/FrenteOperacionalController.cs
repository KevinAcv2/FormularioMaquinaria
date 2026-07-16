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
        [HttpPost]
        public IActionResult Crear([FromBody] FrenteOperacional frente)
        {
            if (string.IsNullOrWhiteSpace(frente.Nombre))
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Debe ingresar un nombre."
                });
            }

            bool existe = _context.FrentesOperacionales
                .Any(f => f.Nombre == frente.Nombre);

            if (existe)
            {
                return Json(new
                {
                    success = false,
                    mensaje = "Ese frente ya existe."
                });
            }

            _context.FrentesOperacionales.Add(frente);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                id = frente.Id,
                nombre = frente.Nombre
            });
        }

        public IActionResult Eliminar(int id)
        {
            var frente = _context.FrentesOperacionales.Find(id);

            if (frente != null)
            {
                _context.FrentesOperacionales.Remove(frente);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Operador");
        }
    }
}