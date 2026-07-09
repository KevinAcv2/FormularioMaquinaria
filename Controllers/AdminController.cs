using Maquinarias.Data;
using FormularioMaquinaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Maquinarias.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<IActionResult> Index(
        string buscar,
        string tipoMaquina)
        {
            var reportes = _context.ReportesMaquinaria.AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
            {
                reportes = reportes.Where(x =>
                    x.NombreOperador.Contains(buscar) ||
                    x.NombreMaquina.Contains(buscar));
            }

            if (!string.IsNullOrEmpty(tipoMaquina))
            {
                reportes = reportes.Where(x =>
                    x.TipoMaquina == tipoMaquina);
            }

            // ESTADÍSTICAS

            ViewBag.TotalReportes =
                await _context.ReportesMaquinaria.CountAsync();

            ViewBag.TotalOperadores =
                await _context.Operadores.CountAsync();

            ViewBag.TotalMaquinas =
                await _context.Maquinas.CountAsync();

            ViewBag.MaquinasOperativas =
                await _context.Maquinas
                .CountAsync(x => x.Estado == "1");

            ViewBag.MaquinasNoOperativas =
                await _context.Maquinas
                .CountAsync(x => x.Estado == "0");

            ViewBag.HorasTotales =
                await _context.ReportesMaquinaria
                .SumAsync(x =>
                    x.HorometroFinal -
                    x.HorometroInicial);

            // REPORTES DEL DÍA
            var hoy = DateTime.UtcNow.Date;
            var manana = hoy.AddDays(1);

            ViewBag.ReportesHoy = await _context.ReportesMaquinaria
                .CountAsync(x => x.Fecha >= hoy && x.Fecha < manana);

            // TOTAL DE OBSERVACIONES

            ViewBag.TotalObservaciones =
                await _context.ReportesMaquinaria
                .CountAsync(x =>
                    !string.IsNullOrEmpty(x.Observaciones));

            // ALERTAS (por ahora serán las máquinas fuera de servicio)

            ViewBag.TotalAlertas =
                await _context.Maquinas
                .CountAsync(x => x.Estado == "0");

            // LISTADOS

            ViewBag.Operadores =
                await _context.Operadores
                .OrderBy(x => x.Nombre)
                .ToListAsync();

            ViewBag.Maquinas =
                await _context.Maquinas
                .OrderBy(x => x.Nombre)
                .ToListAsync();

            ViewBag.RankingOperadores =
                await _context.ReportesMaquinaria

                .GroupBy(x => x.NombreOperador)

                .Select(g => new
                {
                    Operador = g.Key,

                    TotalReportes = g.Count(),

                    HorasTrabajadas =
                        g.Sum(x =>
                            x.HorometroFinal -
                            x.HorometroInicial),

                    Observaciones =
                        g.Count(x =>
                            !string.IsNullOrEmpty(x.Observaciones))
                })

                .OrderByDescending(x => x.HorasTrabajadas)

                .ToListAsync();

            // HORAS POR MÁQUINA
            var horasMaquina = await _context.ReportesMaquinaria
                .GroupBy(x => x.NombreMaquina)
                .Select(x => new
                {
                    Maquina = x.Key,
                    Horas = x.Sum(r => r.HorasTrabajadas)
                })
                .ToListAsync();

            ViewBag.MaquinasChart =
                horasMaquina.Select(x => x.Maquina).ToList();

            ViewBag.HorasChart =
                horasMaquina.Select(x => x.Horas).ToList();


            // REPORTES POR OPERADOR
            var reportesOperador = await _context.ReportesMaquinaria
                .GroupBy(x => x.NombreOperador)
                .Select(x => new
                {
                    Operador = x.Key,
                    Total = x.Count()
                })
                .ToListAsync();

            ViewBag.OperadoresChart =
                reportesOperador.Select(x => x.Operador).ToList();

            ViewBag.ReportesChart =
                reportesOperador.Select(x => x.Total).ToList();

            // ÚLTIMOS REPORTES
            ViewBag.UltimosReportes = await _context.ReportesMaquinaria
                .OrderByDescending(x => x.Fecha)
                .Take(5)
                .ToListAsync();

            // MÁQUINAS CRÍTICAS
            ViewBag.MaquinasCriticas = await _context.ReportesMaquinaria
                .Where(x => x.EstadoMaquina == 0)
                .OrderByDescending(x => x.Fecha)
                .Take(5)
                .ToListAsync();

            // MÁQUINAS FUERA DE SERVICIO
            ViewBag.MaquinasFueraServicio = await _context.Maquinas
                .Where(x => x.Estado == "0")
                .OrderBy(x => x.Nombre)
                .ToListAsync();

            // OPERADORES SIN REPORTE HOY
            var operadoresConReporteHoy = await _context.ReportesMaquinaria
                .Where(x => x.Fecha >= hoy && x.Fecha < manana)
                .Select(x => x.NombreOperador)
                .Distinct()
                .ToListAsync();

            ViewBag.OperadoresSinReporte =
                await _context.Operadores.CountAsync(x =>
                    !operadoresConReporteHoy.Contains(x.Nombre));

            // ACTIVIDAD RECIENTE
            ViewBag.ActividadReciente = await _context.ReportesMaquinaria
                .OrderByDescending(x => x.Fecha)
                .Take(8)
                .ToListAsync();

            // RESUMEN DEL PERÍODO

            ViewBag.TotalEvaluaciones = await _context.EvaluacionesOperadores.CountAsync();

            //reportes sin evaluacion 
            ViewBag.PendientesEvaluacion =
                await _context.ReportesMaquinaria
                    .CountAsync(r =>
                        !_context.EvaluacionesOperadores
                            .Any(e => e.ReporteMaquinariaId == r.Id));

            ViewBag.TotalHoras = await _context.ReportesMaquinaria
                .SumAsync(r => (decimal?)r.HorasTrabajadas) ?? 0;


            // Promedio de horas por día

            var dias = await _context.ReportesMaquinaria
                .Select(r => r.Fecha.Date)
                .Distinct()
                .CountAsync();

            ViewBag.PromedioHorasDia = dias > 0
                ? Math.Round((decimal)ViewBag.TotalHoras / dias, 1)
                : 0;

            return View(await reportes
                .OrderByDescending(x => x.Fecha)
                .ToListAsync());
        }

        // EDITAR - CARGAR FORMULARIO
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var reporte =
                await _context.ReportesMaquinaria.FindAsync(id);

            if (reporte == null)
            {
                return NotFound();
            }

            return View(reporte);
        }

        // EDITAR - GUARDAR CAMBIOS
        [HttpPost]
        public async Task<IActionResult> Editar(
            ReporteMaquinaria reporte)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reporte);

                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] =
                        "Reporte actualizado correctamente";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            return View(reporte);
        }

        // ELIMINAR
        public async Task<IActionResult> Eliminar(int id)
        {
            var reporte =
                await _context.ReportesMaquinaria.FindAsync(id);

            if (reporte != null)
            {
                _context.ReportesMaquinaria.Remove(reporte);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // EXPORTAR EXCEL
        public async Task<IActionResult> ExportarExcel(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            var reportes = _context.ReportesMaquinaria.AsQueryable();

            // FILTRO FECHAS

            if (fechaInicio.HasValue)
            {
                reportes = reportes.Where(x =>
                    x.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                reportes = reportes.Where(x =>
                    x.Fecha <= fechaFin.Value);
            }

            var lista = await reportes
                .OrderByDescending(x => x.Fecha)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet =
                    workbook.Worksheets.Add("Reportes");

                // ENCABEZADOS

                worksheet.Cell(1, 1).Value = "Fecha";
                worksheet.Cell(1, 2).Value = "Operador";
                worksheet.Cell(1, 3).Value = "Máquina";
                worksheet.Cell(1, 4).Value = "Tipo";
                worksheet.Cell(1, 5).Value = "Horómetro Inicial";
                worksheet.Cell(1, 6).Value = "Horómetro Final";
                worksheet.Cell(1, 7).Value = "Horas Trabajadas";
                worksheet.Cell(1, 8).Value = "Observaciones";

                // ESTILOS

                var encabezado =
                    worksheet.Range("A1:H1");

                encabezado.Style.Font.Bold = true;

                encabezado.Style.Fill.BackgroundColor =
                    XLColor.DarkBlue;

                encabezado.Style.Font.FontColor =
                    XLColor.White;

                int fila = 2;

                foreach (var item in lista)
                {
                    worksheet.Cell(fila, 1)
                        .Value = item.Fecha.ToString("dd/MM/yyyy");

                    worksheet.Cell(fila, 2)
                        .Value = item.NombreOperador;

                    worksheet.Cell(fila, 3)
                        .Value = item.NombreMaquina;

                    worksheet.Cell(fila, 4)
                        .Value = item.TipoMaquina;

                    worksheet.Cell(fila, 5)
                        .Value = item.HorometroInicial;

                    worksheet.Cell(fila, 6)
                        .Value = item.HorometroFinal;

                    worksheet.Cell(fila, 7)
                        .Value =
                            item.HorometroFinal -
                            item.HorometroInicial;

                    worksheet.Cell(fila, 8)
                        .Value = item.Observaciones;

                    fila++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    var contenido =
                        stream.ToArray();

                    return File(
                        contenido,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ReporteMaquinaria.xlsx");
                }
            }
        }

        // EXPORTAR PDF
        public async Task<IActionResult> ExportarPdf(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            QuestPDF.Settings.License =
                LicenseType.Community;

            var reportes =
                _context.ReportesMaquinaria.AsQueryable();

            // FILTROS

            if (fechaInicio.HasValue)
            {
                reportes = reportes.Where(x =>
                    x.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                reportes = reportes.Where(x =>
                    x.Fecha <= fechaFin.Value);
            }

            var lista = await reportes
                .OrderByDescending(x => x.Fecha)
                .ToListAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("REPORTE OPERACIONAL DE MAQUINARIA")
                        .FontSize(22)
                        .Bold()
                        .FontColor(Colors.Blue.Darken3);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        // ENCABEZADOS

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle)
                                .Text("Fecha");

                            header.Cell().Element(CellStyle)
                                .Text("Operador");

                            header.Cell().Element(CellStyle)
                                .Text("Máquina");

                            header.Cell().Element(CellStyle)
                                .Text("Inicial");

                            header.Cell().Element(CellStyle)
                                .Text("Final");

                            header.Cell().Element(CellStyle)
                                .Text("Horas");
                        });

                        // FILAS

                        foreach (var item in lista)
                        {
                            table.Cell().Element(CellBody)
                                .Text(item.Fecha
                                .ToString("dd/MM/yyyy"));

                            table.Cell().Element(CellBody)
                                .Text(item.NombreOperador);

                            table.Cell().Element(CellBody)
                                .Text(item.NombreMaquina);

                            table.Cell().Element(CellBody)
                                .Text(item.HorometroInicial
                                .ToString());

                            table.Cell().Element(CellBody)
                                .Text(item.HorometroFinal
                                .ToString());

                            table.Cell().Element(CellBody)
                                .Text(
                                (item.HorometroFinal -
                                 item.HorometroInicial)
                                 .ToString());
                        }

                        static IContainer CellStyle(
                            IContainer container)
                        {
                            return container
                                .Padding(5)
                                .Background(
                                    Colors.Blue.Darken2)
                                .Border(1)
                                .BorderColor(Colors.White)
                                .AlignCenter()
                                .DefaultTextStyle(x =>
                                    x.FontColor(
                                        Colors.White)
                                     .Bold());
                        }

                        static IContainer CellBody(
                            IContainer container)
                        {
                            return container
                                .Padding(5)
                                .BorderBottom(1)
                                .BorderColor(
                                    Colors.Grey.Lighten2);
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generado el ");
                            x.Span(DateTime.Now
                                .ToString("dd/MM/yyyy HH:mm"));
                        });
                });
            });

            var stream = new MemoryStream();

            pdf.GeneratePdf(stream);

            stream.Position = 0;

            return File(
                stream,
                "application/pdf",
                "ReporteMaquinaria.pdf");
        }
    }
}