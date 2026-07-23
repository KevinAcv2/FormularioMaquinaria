using FormularioMaquinaria.Models;
using Maquinarias.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Maquinarias.ViewModels;

namespace FormularioMaquinaria.Controllers
{
    public class NovedadOperacionController : Controller
    {
        private readonly AppDbContext _context;

        public NovedadOperacionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(NovedadOperacionViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var novedad = new NovedadOperacion
            {
                ReporteMaquinariaId = model.ReporteMaquinariaId,
                TipoNovedad = model.TipoNovedad,
                Observacion = model.Observacion,
                HoraInicio = DateTime.UtcNow,
                Activa = true
            };

            _context.NovedadesOperacion.Add(novedad);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Novedad registrada correctamente."
            });
        }
    }
}