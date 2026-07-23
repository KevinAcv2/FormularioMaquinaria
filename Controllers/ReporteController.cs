using ClosedXML.Excel;
using FormularioMaquinaria.Models;
using FormularioMaquinaria.Pdf;
using Maquinarias.Data;
using Maquinarias.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
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

                TempData["Exito"] = "Reporte enviado correctamente.";

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

        // COMBOS
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

        [HttpGet]
        public async Task<IActionResult> ExportarPdf(int id)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var reporte = await _context.ReportesMaquinaria
                .Include(r => r.Evaluacion)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reporte == null)
                return NotFound();

            var logoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "img",
                "Logo.png");

            var documento = ReporteDetallePdf.Generar(
                reporte,
                logoPath);

            return File(
                documento.GeneratePdf(),
                "application/pdf",
                $"Reporte_{reporte.Id}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcel()
        {
            var reportes = await _context.ReportesMaquinaria
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            using var workbook = new XLWorkbook();

            var hoja = workbook.Worksheets.Add("Historial Reportes");

            // Encabezados
            hoja.Cell(1, 1).Value = "Fecha";
            hoja.Cell(1, 2).Value = "Operador";
            hoja.Cell(1, 3).Value = "Máquina";
            hoja.Cell(1, 4).Value = "Horas Trabajadas";
            hoja.Cell(1, 5).Value = "Estado";

            // Estilo encabezado
            var encabezado = hoja.Range(1, 1, 1, 5);

            encabezado.Style.Font.Bold = true;
            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Fill.BackgroundColor = XLColor.SteelBlue;
            encabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            encabezado.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int fila = 2;

            foreach (var reporte in reportes)
            {
                hoja.Cell(fila, 1).Value = reporte.Fecha.ToString("dd/MM/yyyy");
                hoja.Cell(fila, 2).Value = reporte.NombreOperador;
                hoja.Cell(fila, 3).Value = reporte.NombreMaquina;
                hoja.Cell(fila, 4).Value = reporte.HorasTrabajadas;
                hoja.Cell(fila, 5).Value = reporte.EstadoMaquina;

                hoja.Cell(fila, 5).Value =
                    reporte.EstadoMaquina == 1
                    ? "Operativa"
                    : "No Operativa";

                fila++;
            }

            // Bordes
            var rango = hoja.Range(1, 1, fila - 1, 5);

            rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Centrar columnas
            hoja.Column(1).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            hoja.Column(4).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            hoja.Column(5).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "HistorialReportes.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ImprimirHistorial()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var reportes = await _context.ReportesMaquinaria
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            var logoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "img",
                "Logo.png");

            var documento = HistorialReportesPdf.Generar(
                reportes,
                logoPath);

            return File(
                documento.GeneratePdf(),
                "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ExportarPdfHistorial()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var reportes = await _context.ReportesMaquinaria
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            var logoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "img",
                "Logo.png");

            var documento = HistorialReportesPdf.Generar(
                reportes,
                logoPath);

            return File(
                documento.GeneratePdf(),
                "application/pdf",
                "HistorialReportes.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var reporte = await _context.ReportesMaquinaria
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reporte == null)
            {
                return Json(new
                {
                    exito = false
                });
            }

            _context.ReportesMaquinaria.Remove(reporte);

            await _context.SaveChangesAsync();

            return Json(new
            {
                exito = true
            });
        }

        public IActionResult IniciarJornada()
        {
            return View();
        }

    }
}