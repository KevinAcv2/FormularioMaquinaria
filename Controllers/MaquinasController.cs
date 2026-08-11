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

        // =========================================================
        // LISTAR MÁQUINAS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(string? buscar, string? estado)
        {
            var maquinas = _context.Maquinas.AsQueryable();

            // BUSCAR POR NOMBRE
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                maquinas = maquinas.Where(m =>
                    m.Nombre != null &&
                    m.Nombre.Contains(buscar));
            }

            // FILTRAR POR ESTADO
            if (!string.IsNullOrWhiteSpace(estado))
            {
                maquinas = maquinas.Where(m =>
                    m.Estado == estado);
            }

            // CONSERVAR FILTROS EN LA VISTA
            ViewBag.Buscar = buscar;
            ViewBag.Estado = estado;

            var resultado = await maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return View(resultado);
        }


        // =========================================================
        // CREAR MÁQUINA
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Maquina maquina)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(maquina.Nombre))
            {
                return BadRequest(new
                {
                    mensaje = "El nombre de la máquina es obligatorio."
                });
            }

            if (maquina.Estado != "0" && maquina.Estado != "1")
            {
                return BadRequest(new
                {
                    mensaje = "El estado de la máquina no es válido."
                });
            }

            maquina.Nombre = maquina.Nombre.Trim();

            _context.Maquinas.Add(maquina);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                exito = true
            });
        }


        // =========================================================
        // EDITAR MÁQUINA
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> Editar([FromBody] Maquina maquina)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(maquina.Nombre))
            {
                return BadRequest(new
                {
                    mensaje = "El nombre de la máquina es obligatorio."
                });
            }

            if (maquina.Estado != "0" && maquina.Estado != "1")
            {
                return BadRequest(new
                {
                    mensaje = "El estado de la máquina no es válido."
                });
            }

            var maquinaDB = await _context.Maquinas
                .FirstOrDefaultAsync(m => m.Id == maquina.Id);

            if (maquinaDB == null)
            {
                return NotFound(new
                {
                    mensaje = "La máquina no existe."
                });
            }

            maquinaDB.Nombre = maquina.Nombre.Trim();
            maquinaDB.Estado = maquina.Estado;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                exito = true
            });
        }


        // =========================================================
        // ELIMINAR MÁQUINA
        // =========================================================

        // ELIMINAR MÁQUINA
        [HttpPost]
        public async Task<IActionResult> Eliminar([FromBody] int id)
        {
            try
            {
                var maquina = await _context.Maquinas.FindAsync(id);

                if (maquina == null)
                {
                    return NotFound(new
                    {
                        exito = false,
                        mensaje = "La máquina no fue encontrada."
                    });
                }

                // Buscar operadores que tienen asignada esta máquina
                var operadores = await _context.Operadores
                    .Where(o => o.MaquinaId == id)
                    .ToListAsync();

                // Quitar la asignación de la máquina
                foreach (var operador in operadores)
                {
                    operador.MaquinaId = null;
                }

                // Guardar la desasignación
                await _context.SaveChangesAsync();

                // Eliminar la máquina
                _context.Maquinas.Remove(maquina);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    exito = true,
                    mensaje = "La máquina fue eliminada correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Ocurrió un error al eliminar la máquina."
                });
            }
        }


        // =========================================================
        // EXPORTAR PDF
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ExportarPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var maquinas = await _context.Maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            var logoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "img",
                "Logo.png");

            var pdf = MaquinasPdf.Generar(
                maquinas,
                logoPath);

            var bytes = pdf.GeneratePdf();

            return File(
                bytes,
                "application/pdf",
                "Maquinas.pdf");
        }


        // =========================================================
        // EXPORTAR EXCEL
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ExportarExcel()
        {
            var maquinas = await _context.Maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            using var workbook = new XLWorkbook();

            var hoja = workbook.Worksheets.Add("Maquinarias");

            // ENCABEZADOS
            hoja.Cell(1, 1).Value = "ID";
            hoja.Cell(1, 2).Value = "Nombre";
            hoja.Cell(1, 3).Value = "Estado";

            // ESTILO ENCABEZADOS
            var encabezado = hoja.Range(1, 1, 1, 3);

            encabezado.Style.Font.Bold = true;
            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Fill.BackgroundColor = XLColor.SteelBlue;
            encabezado.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
            encabezado.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            int fila = 2;

            foreach (var maquina in maquinas)
            {
                hoja.Cell(fila, 1).Value = maquina.Id;
                hoja.Cell(fila, 2).Value = maquina.Nombre;

                // 1 = OPERATIVA
                // 0 = NO OPERATIVA
                hoja.Cell(fila, 3).Value =
                    maquina.Estado == "1"
                        ? "OPERATIVA"
                        : "NO OPERATIVA";

                fila++;
            }

            // BORDES
            var rango = hoja.Range(
                1,
                1,
                fila - 1,
                3);

            rango.Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            rango.Style.Border.InsideBorder =
                XLBorderStyleValues.Thin;

            // CENTRAR ID
            hoja.Column(1).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            // CENTRAR ESTADO
            hoja.Column(3).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            // AJUSTAR COLUMNAS
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