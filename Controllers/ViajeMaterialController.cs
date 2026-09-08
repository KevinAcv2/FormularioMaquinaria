using System.Text.RegularExpressions;
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
        // CREAR VIAJE - GET
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarDatos();

            return View(new ViajeMaterial());
        }

        // ============================================================
        // CREAR VIAJE - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            ViajeMaterial viaje,
            IFormFile? evidenciaDescarga)
        {
            await ValidarViaje(viaje, evidenciaDescarga);

            if (!ModelState.IsValid)
            {
                await CargarDatos();

                return View(viaje);
            }

            try
            {
                string uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                viaje.EvidenciaDescarga =
                    await GuardarArchivo(
                        evidenciaDescarga!,
                        uploadsFolder);

                viaje.Placa =
                    viaje.Placa.Trim().ToUpperInvariant();

                viaje.EstablecerFechaRegistro();

                _context.ViajesMateriales.Add(viaje);

                await _context.SaveChangesAsync();

                TempData["Exito"] =
                    "El viaje de material fue registrado correctamente.";

                return RedirectToAction(nameof(Crear));
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Ocurrió un error al registrar el viaje: "
                    + ex.Message;

                await CargarDatos();

                return View(viaje);
            }
        }

        // ============================================================
        // HISTORIAL DE VIAJES
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Historial(
            string? buscar,
            DateTime? desde,
            DateTime? hasta)
        {
            var consulta = _context.ViajesMateriales
                .Include(v => v.FrenteOperacional)
                .Include(v => v.Recibidor)
                .AsQueryable();

            // BÚSQUEDA
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                consulta = consulta.Where(v =>
                    v.Recibidor!.Nombre.Contains(buscar) ||
                    v.NumeroRecibo.Contains(buscar) ||
                    v.Placa.Contains(buscar) ||
                    v.Material.Contains(buscar) ||
                    v.Origen.Contains(buscar) ||
                    v.Destino.Contains(buscar));
            }

            // FECHA DESDE
            if (desde.HasValue)
            {
                consulta = consulta.Where(v =>
                    v.Fecha.Date >= desde.Value.Date);
            }

            // FECHA HASTA
            if (hasta.HasValue)
            {
                consulta = consulta.Where(v =>
                    v.Fecha.Date <= hasta.Value.Date);
            }

            var viajes = await consulta
                .OrderByDescending(v => v.Fecha)
                .ThenByDescending(v => v.Id)
                .ToListAsync();

            ViewBag.Buscar = buscar;

            ViewBag.Desde =
                desde?.ToString("yyyy-MM-dd");

            ViewBag.Hasta =
                hasta?.ToString("yyyy-MM-dd");

            return View(viajes);
        }

        // ============================================================
        // REPORTE DE VIAJES DE MATERIAL
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Reporte(
            string? buscar,
            int? recibidorId,
            int? frenteOperacionalId,
            DateTime? desde,
            DateTime? hasta)
        {
            var consulta = _context.ViajesMateriales
                .Include(v => v.FrenteOperacional)
                .Include(v => v.Recibidor)
                .AsQueryable();

            // ========================================================
            // BÚSQUEDA GENERAL
            // ========================================================

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                var patron = $"%{buscar}%";

                consulta = consulta.Where(v =>
                    EF.Functions.ILike(
                        v.Recibidor!.Nombre,
                        patron)

                    ||

                    EF.Functions.ILike(
                        v.FrenteOperacional!.Nombre,
                        patron)

                    ||

                    EF.Functions.ILike(
                        v.Zona,
                        patron)

                    ||

                    EF.Functions.ILike(
                        v.Tramo,
                        patron)

                    ||

                    EF.Functions.ILike(
                        v.NumeroRecibo,
                        patron)

                    ||

                    EF.Functions.ILike(
                        v.Placa,
                        patron)

                    ||

                    EF.Functions.ILike(
                        v.Material,
                        patron)

                    ||

                    EF.Functions.ILike(
                        v.Origen,
                        patron)

                    ||

                    EF.Functions.ILike(
                        v.AbscisaDescargue,
                        patron)

                    ||

                    EF.Functions.ILike(
                        v.Destino,
                        patron)
                );
            }

            // ========================================================
            // FILTRO POR RECIBIDOR
            // ========================================================

            if (recibidorId.HasValue &&
                recibidorId.Value > 0)
            {
                consulta = consulta.Where(v =>
                    v.RecibidorId ==
                    recibidorId.Value);
            }

            // ========================================================
            // FILTRO POR FRENTE OPERACIONAL
            // ========================================================

            if (frenteOperacionalId.HasValue &&
                frenteOperacionalId.Value > 0)
            {
                consulta = consulta.Where(v =>
                    v.FrenteOperacionalId ==
                    frenteOperacionalId.Value);
            }

            // ========================================================
            // FECHA DESDE
            // ========================================================

            if (desde.HasValue)
            {
                var fechaDesde =
                    desde.Value.Date;

                consulta = consulta.Where(v =>
                    v.Fecha >= fechaDesde);
            }

            // ========================================================
            // FECHA HASTA
            // ========================================================

            if (hasta.HasValue)
            {
                var fechaHasta =
                    hasta.Value.Date.AddDays(1);

                consulta = consulta.Where(v =>
                    v.Fecha < fechaHasta);
            }

            // ========================================================
            // OBTENER VIAJES
            // ========================================================

            var viajes = await consulta
                .OrderByDescending(v => v.Fecha)
                .ThenByDescending(v => v.Id)
                .ToListAsync();

            // ========================================================
            // DATOS PARA FILTRO DE RECIBIDORES
            // ========================================================

            ViewBag.Recibidores =
                await _context.Recibidores
                    .Where(r => r.Habilitado)
                    .OrderBy(r => r.Nombre)
                    .ToListAsync();

            // ========================================================
            // DATOS PARA FILTRO DE FRENTES
            // ========================================================

            ViewBag.FrentesOperacionales =
                await _context.FrentesOperacionales
                    .OrderBy(f => f.Nombre)
                    .ToListAsync();

            // ========================================================
            // VALORES ACTUALES DE LOS FILTROS
            // ========================================================

            ViewBag.Buscar = buscar;

            ViewBag.RecibidorId =
                recibidorId;

            ViewBag.FrenteOperacionalId =
                frenteOperacionalId;

            ViewBag.Desde =
                desde?.ToString("yyyy-MM-dd");

            ViewBag.Hasta =
                hasta?.ToString("yyyy-MM-dd");

            // ========================================================
            // ESTADÍSTICAS
            // ========================================================

            ViewBag.TotalViajes =
                viajes.Count;

            ViewBag.VolumenTotal =
                viajes.Sum(v => v.VolumenM3);

            return View(viajes);
        }

        // ============================================================
        // DETALLE
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var viaje = await _context.ViajesMateriales
                .Include(v => v.FrenteOperacional)
                .Include(v => v.Recibidor)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (viaje == null)
            {
                return NotFound();
            }

            return View(viaje);
        }

        // ============================================================
        // VALIDAR VIAJE
        // ============================================================

        private async Task ValidarViaje(
            ViajeMaterial viaje,
            IFormFile? evidenciaDescarga)
        {
            await ValidarRecibidor(viaje);

            await ValidarFrenteOperacional(viaje);

            ValidarNumeroRecibo(viaje);

            ValidarVolumen(viaje);

            ValidarEvidencia(evidenciaDescarga);
        }

        // ============================================================
        // VALIDAR RECIBIDOR
        // ============================================================

        private async Task ValidarRecibidor(
            ViajeMaterial viaje)
        {
            if (viaje.RecibidorId <= 0)
            {
                ModelState.AddModelError(
                    nameof(viaje.RecibidorId),
                    "Debe seleccionar un recibidor.");

                return;
            }

            bool recibidorExiste =
                await _context.Recibidores
                    .AnyAsync(r =>
                        r.Id == viaje.RecibidorId &&
                        r.Habilitado);

            if (!recibidorExiste)
            {
                ModelState.AddModelError(
                    nameof(viaje.RecibidorId),
                    "El recibidor seleccionado no existe o está inactivo.");
            }
        }

        // ============================================================
        // VALIDAR FRENTE OPERACIONAL
        // ============================================================

        private async Task ValidarFrenteOperacional(
            ViajeMaterial viaje)
        {
            if (viaje.FrenteOperacionalId <= 0)
            {
                ModelState.AddModelError(
                    nameof(viaje.FrenteOperacionalId),
                    "Debe seleccionar un frente operacional.");

                return;
            }

            bool frenteExiste =
                await _context.FrentesOperacionales
                    .AnyAsync(f =>
                        f.Id == viaje.FrenteOperacionalId);

            if (!frenteExiste)
            {
                ModelState.AddModelError(
                    nameof(viaje.FrenteOperacionalId),
                    "El frente operacional seleccionado no existe.");
            }
        }

        // ============================================================
        // VALIDAR NÚMERO DE RECIBO
        // ============================================================

        private void ValidarNumeroRecibo(
            ViajeMaterial viaje)
        {
            if (string.IsNullOrWhiteSpace(
                    viaje.NumeroRecibo))
            {
                ModelState.AddModelError(
                    nameof(viaje.NumeroRecibo),
                    "El número de recibo es obligatorio.");

                return;
            }

            if (!Regex.IsMatch(
                    viaje.NumeroRecibo,
                    @"^\d+$"))
            {
                ModelState.AddModelError(
                    nameof(viaje.NumeroRecibo),
                    "El número de recibo solo puede contener números.");
            }
        }

        // ============================================================
        // VALIDAR VOLUMEN
        // ============================================================

        private void ValidarVolumen(
            ViajeMaterial viaje)
        {
            if (viaje.VolumenM3 <= 0)
            {
                ModelState.AddModelError(
                    nameof(viaje.VolumenM3),
                    "El volumen debe ser mayor que cero.");
            }
        }

        // ============================================================
        // VALIDAR EVIDENCIA
        // ============================================================

        private void ValidarEvidencia(
            IFormFile? evidenciaDescarga)
        {
            if (evidenciaDescarga == null ||
                evidenciaDescarga.Length == 0)
            {
                ModelState.AddModelError(
                    "EvidenciaDescarga",
                    "Debe adjuntar una fotografía de la descarga.");

                return;
            }

            const long maximoBytes =
                10 * 1024 * 1024;

            if (evidenciaDescarga.Length >
                maximoBytes)
            {
                ModelState.AddModelError(
                    "EvidenciaDescarga",
                    "La fotografía no puede superar los 10 MB.");
            }

            var tiposPermitidos = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!tiposPermitidos.Contains(
                    evidenciaDescarga.ContentType
                        .ToLowerInvariant()))
            {
                ModelState.AddModelError(
                    "EvidenciaDescarga",
                    "La evidencia debe ser una imagen JPG, PNG o WEBP.");
            }
        }

        // ============================================================
        // CARGAR DATOS PARA EL FORMULARIO
        // ============================================================

        private async Task CargarDatos()
        {
            var frentes =
                await _context.FrentesOperacionales
                    .OrderBy(f => f.Nombre)
                    .ToListAsync();

            var recibidores =
                await _context.Recibidores
                    .Where(r => r.Habilitado)
                    .OrderBy(r => r.Nombre)
                    .ToListAsync();

            ViewBag.FrentesOperacionales =
                new SelectList(
                    frentes,
                    "Id",
                    "Nombre");

            ViewBag.Recibidores =
                new SelectList(
                    recibidores,
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
            string extension =
                Path.GetExtension(
                    file.FileName)
                    .ToLowerInvariant();

            string nombreArchivo =
                $"{Guid.NewGuid()}{extension}";

            string ruta =
                Path.Combine(
                    folder,
                    nombreArchivo);

            await using var stream =
                new FileStream(
                    ruta,
                    FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/uploads/{nombreArchivo}";
        }
    }
}