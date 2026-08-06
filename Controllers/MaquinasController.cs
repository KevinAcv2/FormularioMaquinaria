using ClosedXML.Excel;
using FormularioMaquinaria.Models;
using FormularioMaquinaria.Pdf;
using Maquinarias.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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
                    x.Nombre != null && x.Nombre.Contains(buscar));
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

        // EXPORTAR PDF
        [HttpGet]
        public async Task<IActionResult> ExportarPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var maquinas = await _context.Maquinas
                .OrderBy(x => x.Nombre)
                .ToListAsync();

            var logoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "img",
                "Logo.png");

            foreach (var m in maquinas)
            {
                Console.WriteLine($"Id={m.Id}");
                Console.WriteLine($"Nombre={m.Nombre}");
                Console.WriteLine($"Estado={m.Estado}");
            }

            var pdf = MaquinasPdf.Generar(
                maquinas,
                logoPath);

            var bytes = pdf.GeneratePdf();

            return File(
                bytes,
                "application/pdf",
                "Maquinas.pdf");
        }

        public async Task<IActionResult> ExportarExcel()
        {
            var maquinas = await _context.Maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            using var workbook = new XLWorkbook();

            var hoja = workbook.Worksheets.Add("Maquinarias");

            // Encabezados
            hoja.Cell(1, 1).Value = "ID";
            hoja.Cell(1, 2).Value = "Nombre";
            hoja.Cell(1, 3).Value = "Estado";

            // Estilo de encabezados
            var encabezado = hoja.Range(1, 1, 1, 3);
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Fill.BackgroundColor = XLColor.SteelBlue;
            encabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            encabezado.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int fila = 2;

            foreach (var maquina in maquinas)
            {
                hoja.Cell(fila, 1).Value = maquina.Id;
                hoja.Cell(fila, 2).Value = maquina.Nombre;
                hoja.Cell(fila, 3).Value = maquina.Estado == "0"
                    ? "OPERATIVA"
                    : "NO OPERATIVA";

                fila++;
            }

            // Bordes para toda la tabla
            var rango = hoja.Range(1, 1, fila - 1, 3);
            rango.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Centrar la columna ID
            hoja.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Centrar la columna Estado
            hoja.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Ajustar automáticamente el ancho de las columnas
            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Maquinarias.xlsx");
        }
    }
}