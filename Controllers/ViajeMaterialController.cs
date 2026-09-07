using FormularioMaquinaria.Models;
using Maquinarias.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Maquinarias.Controllers
{
    public class ViajeMaterialController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ViajeMaterialController(
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        // ============================================================
        // GET: /ViajeMaterial/Crear
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarFrentes();

            var modelo = new ViajeMaterial
            {
                Fecha = DateTime.Now
            };

            return View(modelo);
        }


        // ============================================================
        // POST: /ViajeMaterial/Crear
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            ViajeMaterial viaje,
            IFormFile? evidenciaDescarga)
        {
            // ========================================================
            // VALIDAR FRENTE OPERACIONAL
            // ========================================================

            if (viaje.FrenteOperacionalId <= 0)
            {
                ModelState.AddModelError(
                    nameof(viaje.FrenteOperacionalId),
                    "Debe seleccionar un frente operacional.");
            }
            else
            {
                bool frenteExiste = await _context.FrentesOperacionales
                    .AnyAsync(f => f.Id == viaje.FrenteOperacionalId);

                if (!frenteExiste)
                {
                    ModelState.AddModelError(
                        nameof(viaje.FrenteOperacionalId),
                        "El frente operacional seleccionado no existe.");
                }
            }


            // ========================================================
            // VALIDAR EVIDENCIA
            // ========================================================

            if (evidenciaDescarga == null ||
                evidenciaDescarga.Length == 0)
            {
                ModelState.AddModelError(
                    "EvidenciaDescarga",
                    "Debe adjuntar una fotografía de la descarga.");
            }
            else
            {
                // Tamaño máximo: 10 MB

                const long maximoBytes = 10 * 1024 * 1024;

                if (evidenciaDescarga.Length > maximoBytes)
                {
                    ModelState.AddModelError(
                        "EvidenciaDescarga",
                        "La fotografía no puede superar los 10 MB.");
                }


                // Tipos de imágenes permitidos

                var tiposPermitidos = new[]
                {
                    "image/jpeg",
                    "image/png",
                    "image/webp"
                };

                if (!tiposPermitidos.Contains(
                    evidenciaDescarga.ContentType.ToLowerInvariant()))
                {
                    ModelState.AddModelError(
                        "EvidenciaDescarga",
                        "La evidencia debe ser una imagen JPG, PNG o WEBP.");
                }
            }


            // ========================================================
            // VALIDAR VOLUMEN
            // ========================================================

            if (viaje.VolumenM3 <= 0)
            {
                ModelState.AddModelError(
                    nameof(viaje.VolumenM3),
                    "El volumen debe ser mayor que cero.");
            }


            // ========================================================
            // VALIDAR HORAS
            // ========================================================

            if (viaje.HoraLlegada < viaje.HoraSalida)
            {
                ModelState.AddModelError(
                    nameof(viaje.HoraLlegada),
                    "La hora de llegada no puede ser menor que la hora de salida.");
            }


            // ========================================================
            // SI EXISTEN ERRORES
            // ========================================================

            if (!ModelState.IsValid)
            {
                await CargarFrentes();

                return View(viaje);
            }


            try
            {
                // ====================================================
                // CREAR CARPETA DE UPLOADS
                // ====================================================

                string uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                // ====================================================
                // GUARDAR EVIDENCIA
                // ====================================================

                viaje.EvidenciaDescarga =
                    await GuardarArchivo(
                        evidenciaDescarga!,
                        uploadsFolder);


                // ====================================================
                // ASIGNAR FECHA DEL REGISTRO
                // ====================================================

                viaje.Fecha = DateTime.Now;


                // ====================================================
                // GUARDAR EN BASE DE DATOS
                // ====================================================

                _context.ViajesMateriales.Add(viaje);

                await _context.SaveChangesAsync();


                // ====================================================
                // MENSAJE DE ÉXITO
                // ====================================================

                TempData["Exito"] =
                    "El viaje de material fue registrado correctamente.";


                return RedirectToAction(nameof(Crear));
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Ocurrió un error al registrar el viaje: "
                    + ex.Message;

                await CargarFrentes();

                return View(viaje);
            }
        }


        // ============================================================
        // GET: /ViajeMaterial/Historial
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Historial(
            string? buscar,
            DateTime? desde,
            DateTime? hasta)
        {
            var consulta = _context.ViajesMateriales
                .Include(v => v.FrenteOperacional)
                .AsQueryable();


            // ========================================================
            // BUSCADOR
            // ========================================================

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                consulta = consulta.Where(v =>
                    v.Recibidor.Contains(buscar) ||
                    v.NumeroRecibo.Contains(buscar) ||
                    v.Placa.Contains(buscar) ||
                    v.Material.Contains(buscar) ||
                    v.Origen.Contains(buscar) ||
                    v.Destino.Contains(buscar));
            }


            // ========================================================
            // FECHA DESDE
            // ========================================================

            if (desde.HasValue)
            {
                consulta = consulta.Where(v =>
                    v.Fecha.Date >= desde.Value.Date);
            }


            // ========================================================
            // FECHA HASTA
            // ========================================================

            if (hasta.HasValue)
            {
                consulta = consulta.Where(v =>
                    v.Fecha.Date <= hasta.Value.Date);
            }


            // ========================================================
            // CONSULTAR VIAJES
            // ========================================================

            var viajes = await consulta
                .OrderByDescending(v => v.Fecha)
                .ThenByDescending(v => v.Id)
                .ToListAsync();


            ViewBag.Buscar = buscar;

            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");

            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");


            return View(viajes);
        }


        // ============================================================
        // GET: /ViajeMaterial/Detalle/5
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var viaje = await _context.ViajesMateriales
                .Include(v => v.FrenteOperacional)
                .FirstOrDefaultAsync(v => v.Id == id);


            if (viaje == null)
            {
                return NotFound();
            }


            return View(viaje);
        }


        // ============================================================
        // CARGAR FRENTES OPERACIONALES
        // ============================================================

        private async Task CargarFrentes()
        {
            var frentes = await _context.FrentesOperacionales
                .OrderBy(f => f.Nombre)
                .ToListAsync();


            ViewBag.FrentesOperacionales =
                new SelectList(
                    frentes,
                    "Id",
                    "Nombre");
        }


        // ============================================================
        // GUARDAR ARCHIVO
        // ============================================================

        private async Task<string> GuardarArchivo(
            IFormFile file,
            string folder)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }


            // Obtener extensión

            string extension =
                Path.GetExtension(file.FileName)
                .ToLowerInvariant();


            // Crear nombre único

            string nombreArchivo =
                $"{Guid.NewGuid()}{extension}";


            // Crear ruta física

            string ruta = Path.Combine(
                folder,
                nombreArchivo);


            // Guardar archivo

            await using var stream = new FileStream(
                ruta,
                FileMode.Create);


            await file.CopyToAsync(stream);


            // Retornar ruta para guardar en PostgreSQL

            return $"/uploads/{nombreArchivo}";
        }
    }
}
