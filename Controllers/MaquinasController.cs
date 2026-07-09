using FormularioMaquinaria.Models;
using Maquinarias.Data;
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

        // LISTAR MÁQUINAS
        public async Task<IActionResult> Index(string buscar, string estado)
        {
            var maquinas = _context.Maquinas.AsQueryable();

            // Buscar por nombre
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                maquinas = maquinas.Where(x =>
                    x.Nombre.Contains(buscar));
            }

            // Filtrar por estado
            if (!string.IsNullOrWhiteSpace(estado))
            {
                maquinas = maquinas.Where(x =>
                    x.Estado == estado);
            }

            // Guardar los filtros para la vista
            ViewBag.Buscar = buscar;
            ViewBag.Estado = estado;

            return View(await maquinas
                .OrderBy(x => x.Nombre)
                .ToListAsync());
        }

        // MOSTRAR FORMULARIO
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // GUARDAR MÁQUINA
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Maquina maquina)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Maquinas.Add(maquina);

            await _context.SaveChangesAsync();

            return Ok();
        }
        // CARGAR EDICIÓN
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var maquina = await _context.Maquinas.FindAsync(id);

            if (maquina == null)
            {
                return NotFound();
            }

            return View(maquina);
        }

        // GUARDAR EDICIÓN
        [HttpPost]
        public async Task<IActionResult> Editar([FromBody] Maquina maquina)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var maquinaDB = await _context.Maquinas.FindAsync(maquina.Id);

            if (maquinaDB == null)
            {
                return NotFound();
            }

            maquinaDB.Nombre = maquina.Nombre;
            maquinaDB.Estado = maquina.Estado;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // ELIMINAR
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var maquina = await _context.Maquinas.FindAsync(id);

            if (maquina != null)
            {
                // Buscar operadores que usan esta máquina
                var operadores = await _context.Operadores
                    .Where(o => o.MaquinaId == id)
                    .ToListAsync();

                // Quitar la asignación
                foreach (var operador in operadores)
                {
                    operador.MaquinaId = null;
                }

                await _context.SaveChangesAsync();

                // Ahora sí eliminar la máquina
                _context.Maquinas.Remove(maquina);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}