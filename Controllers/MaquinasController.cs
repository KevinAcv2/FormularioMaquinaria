using ClosedXML.Excel;
using FormularioMaquinaria.Models;
using FormularioMaquinaria.Pdf;
using Maquinarias.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
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
        public async Task<IActionResult> Index(
            string? buscar,
            string? estado,
            string? buscarVolqueta,
            string? estadoVolqueta)
        {
            // =====================================================
            // MÁQUINAS
            // =====================================================

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

            // CONSERVAR FILTROS DE MÁQUINAS
            ViewBag.Buscar = buscar;
            ViewBag.Estado = estado;

            var resultado = await maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync();


            // =====================================================
            // VOLQUETAS
            // =====================================================

            var volquetas = _context.Volquetas.AsQueryable();

            // BUSCAR POR PLACA
            if (!string.IsNullOrWhiteSpace(buscarVolqueta))
            {
                buscarVolqueta = buscarVolqueta.Trim().ToUpper();

                volquetas = volquetas.Where(v =>
                    v.Placa.ToUpper().Contains(buscarVolqueta));
            }

            // FILTRAR POR ESTADO
            if (!string.IsNullOrWhiteSpace(estadoVolqueta))
            {
                volquetas = volquetas.Where(v =>
                    v.Estado == estadoVolqueta);
            }

            ViewBag.BuscarVolqueta = buscarVolqueta;
            ViewBag.EstadoVolqueta = estadoVolqueta;

            ViewBag.Volquetas = await volquetas
                .OrderBy(v => v.Placa)
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

        [HttpGet]
        public async Task<IActionResult> ExportarExcel()
        {
            var maquinas = await _context.Maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            var volquetas = await _context.Volquetas
                .OrderBy(v => v.Placa)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var hoja = workbook.Worksheets.Add("Gestión de Equipos");

            int fila = 1;

            // -----------------------------------------------------
            // SECCIÓN 1: MÁQUINAS
            // -----------------------------------------------------
            hoja.Cell(fila, 1).Value = "🚜 Máquinas Registradas";
            hoja.Cell(fila, 1).Style.Font.Bold = true;
            hoja.Cell(fila, 1).Style.Font.FontSize = 13;
            fila++;

            hoja.Cell(fila, 1).Value = "ID";
            hoja.Cell(fila, 2).Value = "Nombre / Tipo";
            hoja.Cell(fila, 3).Value = "Estado";

            var encMaquinas = hoja.Range(fila, 1, fila, 3);
            encMaquinas.Style.Font.Bold = true;
            encMaquinas.Style.Font.FontColor = XLColor.White;
            encMaquinas.Style.Fill.BackgroundColor = XLColor.SteelBlue;
            encMaquinas.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            encMaquinas.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            fila++;

            int inicioFilaM = fila;
            foreach (var m in maquinas)
            {
                hoja.Cell(fila, 1).Value = m.Id;
                hoja.Cell(fila, 2).Value = m.Nombre;
                hoja.Cell(fila, 3).Value = m.Estado == "1" ? "OPERATIVA" : "NO OPERATIVA";
                fila++;
            }

            if (maquinas.Any())
            {
                var rangoM = hoja.Range(inicioFilaM - 1, 1, fila - 1, 3);
                rangoM.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rangoM.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Centrar todo el contenido de la tabla de máquinas
                rangoM.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangoM.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            fila += 3; // Espacio entre tablas

            // -----------------------------------------------------
            // SECCIÓN 2: VOLQUETAS
            // -----------------------------------------------------
            hoja.Cell(fila, 1).Value = "🚛 Volquetas Registradas";
            hoja.Cell(fila, 1).Style.Font.Bold = true;
            hoja.Cell(fila, 1).Style.Font.FontSize = 13;
            fila++;

            hoja.Cell(fila, 1).Value = "ID";
            hoja.Cell(fila, 2).Value = "Placa";
            hoja.Cell(fila, 3).Value = "Propiedad";
            hoja.Cell(fila, 4).Value = "Empresa Propietaria";
            hoja.Cell(fila, 5).Value = "Estado";

            var encVolquetas = hoja.Range(fila, 1, fila, 5);
            encVolquetas.Style.Font.Bold = true;
            encVolquetas.Style.Font.FontColor = XLColor.White;
            encVolquetas.Style.Fill.BackgroundColor = XLColor.SteelBlue;
            encVolquetas.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            encVolquetas.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            fila++;

            int inicioFilaV = fila;
            foreach (var v in volquetas)
            {
                hoja.Cell(fila, 1).Value = v.Id;
                hoja.Cell(fila, 2).Value = v.Placa;
                hoja.Cell(fila, 3).Value = v.Propiedad;
                hoja.Cell(fila, 4).Value = string.IsNullOrWhiteSpace(v.EmpresaPropietaria) ? "—" : v.EmpresaPropietaria;
                hoja.Cell(fila, 5).Value = v.Estado == "1" ? "ACTIVA" : "INACTIVA";
                fila++;
            }

            if (volquetas.Any())
            {
                var rangoV = hoja.Range(inicioFilaV - 1, 1, fila - 1, 5);
                rangoV.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rangoV.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Centrar todo el contenido de la tabla de volquetas
                rangoV.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rangoV.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            // Ajustar automáticamente el ancho de las columnas
            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Gestion_Equipos.xlsx");
        }

        // =========================================================
        // EXPORTAR PDF CON MÁQUINAS Y VOLQUETAS (QUESTPDF)
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> ExportarPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var maquinas = await _context.Maquinas.OrderBy(m => m.Nombre).ToListAsync();
            var volquetas = await _context.Volquetas.OrderBy(v => v.Placa).ToListAsync();

            var logoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "img", "Logo.jpg");

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    // ENCABEZADO (Corregido)
                    page.Header().Row(row =>
                    {
                        if (System.IO.File.Exists(logoPath))
                        {
                            row.ConstantItem(60).Image(logoPath);
                        }

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("AVENIDA DEL RÍO").FontSize(18).Bold().FontColor("#0f2f44");
                            col.Item().Text("Reporte Consolidado de Gestión de Equipos").FontSize(12).FontColor("#6c757d");
                        });

                        row.ConstantItem(100).AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(10);
                    });

                    // CONTENIDO
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        // TABLA DE MÁQUINAS (Corregido)
                        col.Item().Text("🚜 Máquinas Registradas").FontSize(14).Bold().FontColor("#0f2f44");
                        col.Item().PaddingBottom(10).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(cd =>
                            {
                                cd.ConstantColumn(40);
                                cd.RelativeColumn();
                                cd.ConstantColumn(100);
                            });

                            tabla.Header(h =>
                            {
                                h.Cell().Background("#0f2f44").Padding(5).Text(x => x.Span("ID").FontColor("#ffffff").Bold());
                                h.Cell().Background("#0f2f44").Padding(5).Text(x => x.Span("Nombre / Tipo").FontColor("#ffffff").Bold());
                                h.Cell().Background("#0f2f44").Padding(5).Text(x => x.Span("Estado").FontColor("#ffffff").Bold());
                            });

                            foreach (var m in maquinas)
                            {
                                tabla.Cell().BorderBottom(1).BorderColor("#eef1f4").Padding(5).Text(m.Id.ToString());
                                tabla.Cell().BorderBottom(1).BorderColor("#eef1f4").Padding(5).Text(m.Nombre ?? "");
                                tabla.Cell().BorderBottom(1).BorderColor("#eef1f4").Padding(5)
                                    .Text(x => x.Span(m.Estado == "1" ? "OPERATIVA" : "NO OPERATIVA")
                                                 .Bold()
                                                 .FontColor(m.Estado == "1" ? "#198754" : "#dc3545"));
                            }
                        });

                        col.Item().PaddingVertical(10);

                        // TABLA DE VOLQUETAS (Corregido)
                        col.Item().Text("🚛 Volquetas Registradas").FontSize(14).Bold().FontColor("#0f2f44");
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(cd =>
                            {
                                cd.ConstantColumn(40);
                                cd.ConstantColumn(90);
                                cd.ConstantColumn(90);
                                cd.RelativeColumn();
                                cd.ConstantColumn(90);
                            });

                            tabla.Header(h =>
                            {
                                h.Cell().Background("#0f2f44").Padding(5).Text(x => x.Span("ID").FontColor("#ffffff").Bold());
                                h.Cell().Background("#0f2f44").Padding(5).Text(x => x.Span("Placa").FontColor("#ffffff").Bold());
                                h.Cell().Background("#0f2f44").Padding(5).Text(x => x.Span("Propiedad").FontColor("#ffffff").Bold());
                                h.Cell().Background("#0f2f44").Padding(5).Text(x => x.Span("Empresa").FontColor("#ffffff").Bold());
                                h.Cell().Background("#0f2f44").Padding(5).Text(x => x.Span("Estado").FontColor("#ffffff").Bold());
                            });

                            foreach (var v in volquetas)
                            {
                                tabla.Cell().BorderBottom(1).BorderColor("#eef1f4").Padding(5).Text(v.Id.ToString());
                                tabla.Cell().BorderBottom(1).BorderColor("#eef1f4").Padding(5).Text(v.Placa);
                                tabla.Cell().BorderBottom(1).BorderColor("#eef1f4").Padding(5).Text(v.Propiedad);
                                tabla.Cell().BorderBottom(1).BorderColor("#eef1f4").Padding(5).Text(string.IsNullOrWhiteSpace(v.EmpresaPropietaria) ? "—" : v.EmpresaPropietaria);
                                tabla.Cell().BorderBottom(1).BorderColor("#eef1f4").Padding(5)
                                    .Text(x => x.Span(v.Estado == "1" ? "ACTIVA" : "INACTIVA")
                                                 .Bold()
                                                 .FontColor(v.Estado == "1" ? "#198754" : "#dc3545"));
                            }
                        });
                    });

                    // PIE DE PÁGINA
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return File(pdf.GeneratePdf(), "application/pdf", "Gestion_Equipos.pdf");
        }
    }
}