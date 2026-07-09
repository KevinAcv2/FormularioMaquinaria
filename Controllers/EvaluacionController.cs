using FormularioMaquinaria.Models;
using Maquinarias.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maquinarias.Controllers
{
    public class EvaluacionController : Controller
    {
        private readonly AppDbContext _context;

        public EvaluacionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Crear(int id)
        {
            var reporte = await _context.ReportesMaquinaria
                .FirstOrDefaultAsync(x => x.Id == id);

            if (reporte == null)
                return NotFound();

            var modelo = new EvaluacionOperador
            {
                ReporteMaquinariaId = reporte.Id
            };

            ViewBag.Reporte = reporte;

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] EvaluacionOperador evaluacion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                evaluacion.FechaEvaluacion = DateTime.UtcNow;

                _context.EvaluacionesOperadores.Add(evaluacion);

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
    
}