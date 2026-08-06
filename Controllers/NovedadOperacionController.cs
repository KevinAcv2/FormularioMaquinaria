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
        public async Task<IActionResult> Crear(
            NovedadOperacionViewModel model,
            IFormFile? EvidenciaNovedad)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var novedad = new NovedadOperacion
            {
                ReporteMaquinariaId = model.ReporteMaquinariaId,
                TipoNovedad = model.TipoNovedad,
                Observacion = model.Observacion ?? string.Empty,
                HoraInicio = DateTime.UtcNow,
                Activa = true
            };

            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (EvidenciaNovedad != null)
            {
                string nombreArchivo =
                    Guid.NewGuid() +
                    Path.GetExtension(EvidenciaNovedad.FileName);

                string ruta =
                    Path.Combine(uploadsFolder, nombreArchivo);

                using var stream =
                    new FileStream(ruta, FileMode.Create);

                await EvidenciaNovedad.CopyToAsync(stream);

                novedad.EvidenciaInicio =
                    "/uploads/" + nombreArchivo;
            }

            _context.NovedadesOperacion.Add(novedad);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Novedad registrada correctamente."
            });
        }
    }
}