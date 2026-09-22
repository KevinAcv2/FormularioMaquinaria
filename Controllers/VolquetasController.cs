using FormularioMaquinaria.Models;
using Maquinarias.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinarias.Controllers
{
    public class VolquetasController : Controller
    {
        private readonly AppDbContext _context;

        public VolquetasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Volquetas
        public async Task<IActionResult> Index()
        {
            var volquetas = await _context.Volquetas
                .OrderBy(v => v.Placa)
                .ToListAsync();

            return View(volquetas);
        }

        // POST: Volquetas/Crear
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Volqueta volqueta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Los datos de la volqueta no son válidos."
                });
            }

            volqueta.Placa = volqueta.Placa.Trim().ToUpper();
            volqueta.Propiedad = volqueta.Propiedad.Trim().ToUpper();

            if (volqueta.Propiedad != "PROPIA" &&
                volqueta.Propiedad != "ALQUILADA")
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "La propiedad debe ser PROPIA o ALQUILADA."
                });
            }

            bool existe = await _context.Volquetas
                .AnyAsync(v => v.Placa.ToUpper() == volqueta.Placa);

            if (existe)
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Ya existe una volqueta registrada con esa placa."
                });
            }

            volqueta.Estado = "1";

            _context.Volquetas.Add(volqueta);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                exito = true,
                mensaje = "Volqueta registrada correctamente."
            });
        }

        // POST: Volquetas/Editar
        [HttpPost]
        public async Task<IActionResult> Editar([FromBody] Volqueta volqueta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Los datos de la volqueta no son válidos."
                });
            }

            var existente = await _context.Volquetas
                .FirstOrDefaultAsync(v => v.Id == volqueta.Id);

            if (existente == null)
            {
                return NotFound(new
                {
                    exito = false,
                    mensaje = "La volqueta no existe."
                });
            }

            volqueta.Placa = volqueta.Placa.Trim().ToUpper();
            volqueta.Propiedad = volqueta.Propiedad.Trim().ToUpper();

            if (volqueta.Propiedad != "PROPIA" &&
                volqueta.Propiedad != "ALQUILADA")
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "La propiedad debe ser PROPIA o ALQUILADA."
                });
            }

            bool existeOtra = await _context.Volquetas
                .AnyAsync(v =>
                    v.Id != volqueta.Id &&
                    v.Placa.ToUpper() == volqueta.Placa);

            if (existeOtra)
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Ya existe otra volqueta registrada con esa placa."
                });
            }

            existente.Placa = volqueta.Placa;
            existente.Propiedad = volqueta.Propiedad;
            existente.EmpresaPropietaria = volqueta.EmpresaPropietaria?.Trim();
            existente.Estado = volqueta.Estado;
            existente.Observaciones = volqueta.Observaciones?.Trim();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                exito = true,
                mensaje = "Volqueta actualizada correctamente."
            });
        }

        // POST: Volquetas/CambiarEstado
        [HttpPost]
        public async Task<IActionResult> CambiarEstado([FromBody] int id)
        {
            var volqueta = await _context.Volquetas
                .FirstOrDefaultAsync(v => v.Id == id);

            if (volqueta == null)
            {
                return NotFound(new
                {
                    exito = false,
                    mensaje = "La volqueta no existe."
                });
            }

            volqueta.Estado = volqueta.Estado == "1" ? "0" : "1";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                exito = true,
                mensaje = volqueta.Estado == "1"
                    ? "Volqueta activada correctamente."
                    : "Volqueta desactivada correctamente."
            });
        }
    }
}