using Maquinarias.Data;
using FormularioMaquinaria.Models;
using Maquinarias.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Maquinarias.Controllers
{
    public class ReporteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly OcrService _ocrService;

        public ReporteController(
            AppDbContext context,
            IWebHostEnvironment environment,
            OcrService ocrService)
        {   
            _context = context;
            _environment = environment;
            _ocrService = ocrService;
        }

        // GET: CREAR
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarCombos();
            return View();
        }

        // POST: CREAR
        [HttpPost]
        public async Task<IActionResult> Crear(
            ReporteMaquinaria reporte,
            IFormFile fotoInicial,
            IFormFile fotoFinal)
        {
            if (fotoInicial == null || fotoFinal == null)
            {
                ModelState.AddModelError("", "Las fotos son obligatorias");
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos();
                return View(reporte);
            }

            try
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                reporte.FotoHorometroInicial = await GuardarArchivo(fotoInicial, uploadsFolder);
                reporte.FotoHorometroFinal = await GuardarArchivo(fotoFinal, uploadsFolder);

                reporte.Fecha = DateTime.UtcNow;
                reporte.HorasTrabajadas = reporte.HorometroFinal - reporte.HorometroInicial;

                _context.ReportesMaquinaria.Add(reporte);
                await _context.SaveChangesAsync();

                // Actualizar el estado actual de la máquina
                var maquina = await _context.Maquinas
                    .FirstOrDefaultAsync(m => m.Nombre == reporte.NombreMaquina);

                if (maquina != null)
                {
                    maquina.Estado = reporte.EstadoMaquina.ToString();

                    await _context.SaveChangesAsync();
                }

                TempData["Mensaje"] = "Reporte enviado correctamente";

                return RedirectToAction(nameof(Crear));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                await CargarCombos();
                return View(reporte);
            }
        }

        // HISTORIAL DE REPORTES
        [HttpGet]
        public async Task<IActionResult> Historial(string buscar, DateTime? desde, DateTime? hasta, string estado)
        {
            var consulta = _context.ReportesMaquinaria.AsQueryable();

            // Buscar por operador o máquina
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                consulta = consulta.Where(r =>
                    r.NombreOperador.Contains(buscar) ||
                    r.NombreMaquina.Contains(buscar));
            }

            // Fecha desde
            if (desde.HasValue)
            {
                consulta = consulta.Where(r => r.Fecha.Date >= desde.Value.Date);
            }

            // Fecha hasta
            if (hasta.HasValue)
            {
                consulta = consulta.Where(r => r.Fecha.Date <= hasta.Value.Date);
            }

            // Estado
            if (!string.IsNullOrEmpty(estado))
            {
                if (estado == "Operativa")
                {
                    consulta = consulta.Where(r => r.EstadoMaquina == 1);
                }
                else if (estado == "No Operativa")
                {
                    consulta = consulta.Where(r => r.EstadoMaquina == 0);
                }
            }

            var reportes = await consulta
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            ViewBag.Buscar = buscar;
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
            ViewBag.Estado = estado;
            return View(reportes);
        }

        // VER REPORTE
        [HttpGet]
        public async Task<IActionResult> Ver(int id)
        {
            var reporte = await _context.ReportesMaquinaria
                .Include(r => r.Evaluacion)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reporte == null)
            {
                return NotFound();
            }
            return View(reporte);
        }

        // OCR HORÓMETRO
        [HttpPost]
        public async Task<IActionResult> LeerHorometro(IFormFile imagen)
        {
            if (imagen == null)
            {
                return Json(new { exito = false, mensaje = "Imagen vacía" });
            }

            var texto = await _ocrService.LeerTextoAsync(imagen);
            var match = Regex.Match(texto, @"\d+([.,]\d+)?");

            if (!match.Success)
            {
                return Json(new { exito = false, mensaje = "No se detectó número" });
            }

            return Json(new
            {
                exito = true,
                valor = match.Value.Replace(",", ".")
            });
        }

        // MAQUINA POR OPERADOR
        [HttpGet]
        public async Task<IActionResult> ObtenerMaquinaOperador(string nombre)
        {
            var operador = await _context.Operadores
                .Include(o => o.Maquina)
                .FirstOrDefaultAsync(o => o.Nombre == nombre);

            if (operador?.Maquina == null)
                return Json(new { exito = false });

            return Json(new
            {
                exito = true,
                maquina = operador.Maquina.Nombre,
                estado = operador.Maquina.Estado
            });
        }

        // GUARDAR ARCHIVO
        private async Task<string> GuardarArchivo(IFormFile file, string folder)
        {
            string nombreArchivo = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string ruta = Path.Combine(folder, nombreArchivo);

            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + nombreArchivo;
        }

        // COMBOS (CORREGIDO)
        private async Task CargarCombos()
        {
            ViewBag.Operadores = new SelectList(
                await _context.Operadores
                    .OrderBy(o => o.Nombre)
                    .ToListAsync(),
                "Nombre",
                "Nombre"
            );

            ViewBag.Maquinas = new SelectList(
                await _context.Maquinas
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                "Id",      
                "Nombre"   
            );
        }
    }
}