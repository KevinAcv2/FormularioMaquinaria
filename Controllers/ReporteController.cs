using Maquinarias.Data;
using Maquinarias.Models;
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

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarCombos();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(
            ReporteMaquinaria reporte,
            IFormFile fotoInicial,
            IFormFile fotoFinal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string uploadsFolder = Path.Combine(
                        _environment.WebRootPath,
                        "uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // FOTO INICIAL

                    if (fotoInicial != null)
                    {
                        string nombreArchivo =
                            Guid.NewGuid().ToString() +
                            Path.GetExtension(fotoInicial.FileName);

                        string rutaCompleta =
                            Path.Combine(uploadsFolder, nombreArchivo);

                        using (var stream = new FileStream(
                            rutaCompleta,
                            FileMode.Create))
                        {
                            await fotoInicial.CopyToAsync(stream);
                        }

                        reporte.FotoHorometroInicial =
                            "/uploads/" + nombreArchivo;
                    }

                    // FOTO FINAL

                    if (fotoFinal != null)
                    {
                        string nombreArchivo =
                            Guid.NewGuid().ToString() +
                            Path.GetExtension(fotoFinal.FileName);

                        string rutaCompleta =
                            Path.Combine(uploadsFolder, nombreArchivo);

                        using (var stream = new FileStream(
                            rutaCompleta,
                            FileMode.Create))
                        {
                            await fotoFinal.CopyToAsync(stream);
                        }

                        reporte.FotoHorometroFinal =
                            "/uploads/" + nombreArchivo;
                    }

                    reporte.Fecha = DateTime.UtcNow;

                    _context.ReportesMaquinaria.Add(reporte);

                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] =
                        "Reporte enviado correctamente";

                    return RedirectToAction(nameof(Crear));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            await CargarCombos();

            return View(reporte);
        }

        [HttpPost]
        public async Task<IActionResult> LeerHorometro(
            IFormFile imagen)
        {
            try
            {
                if (imagen == null || imagen.Length == 0)
                {
                    return Json(new
                    {
                        exito = false,
                        mensaje = "No se recibió ninguna imagen"
                    });
                }

                var texto =
                    await _ocrService.LeerTextoAsync(imagen);

                var match =
                    Regex.Match(
                        texto,
                        @"\d+([.,]\d+)?");

                if (!match.Success)
                {
                    return Json(new
                    {
                        exito = false,
                        mensaje =
                            "No se encontró un número en la imagen",
                        textoDetectado = texto
                    });
                }

                string numero =
                    match.Value.Replace(",", ".");

                return Json(new
                {
                    exito = true,
                    valor = numero,
                    textoDetectado = texto
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exito = false,
                    mensaje = ex.Message
                });
            }
        }

        private async Task CargarCombos()
        {
            ViewBag.Operadores = new SelectList(
            await _context.Operadores
            .OrderBy(o => o.Nombre)
            .ToListAsync(),
            "Nombre",
            "Nombre");

            ViewBag.Maquinas = new SelectList(
                await _context.Maquinas
                    .Where(m => m.Estado == "Operativa")
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                "Nombre",
                "Nombre");

        }
    }
}