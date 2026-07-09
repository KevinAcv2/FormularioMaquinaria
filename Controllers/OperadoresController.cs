using FormularioMaquinaria.Models;
using Maquinarias.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using FormularioMaquinaria.Pdf;
public class OperadoresController : Controller
{
    private readonly AppDbContext _context;

    public OperadoresController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> ExportarExcel()
    {
        var operadores = await _context.Operadores
            .Include(o => o.Maquina)
            .OrderBy(o => o.Nombre)
            .ToListAsync();

        using var workbook = new XLWorkbook();

        var hoja = workbook.Worksheets.Add("Operadores");

        // Encabezados
        hoja.Cell(1, 1).Value = "ID";
        hoja.Cell(1, 2).Value = "Nombre";
        hoja.Cell(1, 3).Value = "Máquina";
        hoja.Cell(1, 4).Value = "Frente Operacional";

        int fila = 2;

        foreach (var operador in operadores)
        {
            hoja.Cell(fila, 1).Value = operador.Id;
            hoja.Cell(fila, 2).Value = operador.Nombre;
            hoja.Cell(fila, 3).Value = operador.Maquina?.Nombre ?? "Sin asignar";
            hoja.Cell(fila, 4).Value = operador.FrenteOperacional;

            fila++;
        }

        hoja.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        var contenido = stream.ToArray();

        return File(
            contenido,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Operadores.xlsx");
    }

    // EXPORTAR PDF
    public async Task<IActionResult> ExportarPdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var operadores = await _context.Operadores
            .Include(o => o.Maquina)
            .OrderBy(o => o.Nombre)
            .ToListAsync();

        var logoPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "img",
            "Logo.png");

        var pdf = PdfTemplate.Crear(
    "GESTIÓN DE OPERADORES",
    "Administración de operadores autorizados",
    logoPath,
    contenido =>
    {
        contenido.Column(column =>
        {
            // INFORMACIÓN GENERAL

            column.Item()
    .Border(1)
    .BorderColor("#C8D2DC")
    .Background("#F8FAFC")
    .Padding(12)
    .Table(table =>
    {
        table.ColumnsDefinition(columns =>
        {
            columns.RelativeColumn();
            columns.RelativeColumn();
        });

        void Celda(string titulo, string valor)
        {
            table.Cell()
                .Border(1)
                .BorderColor("#D9E2EC")
                .Padding(6)
                .Column(col =>
                {
                    col.Item()
                        .Text(titulo)
                        .Bold()
                        .FontSize(9)
                        .FontColor("#0F4C81");

                    col.Item()
                        .Text(valor)
                        .FontSize(10);
                });
        }

        Celda("Documento", "Listado de Operadores");
        Celda("Empresa", "AV Río");

        Celda("Fecha", DateTime.Now.ToString("dd/MM/yyyy"));
        Celda("Hora", DateTime.Now.ToString("HH:mm"));

        Celda("Total Operadores", operadores.Count.ToString());
        Celda("Generado por", "Sistema de Gestión");
    });

            column.Item().PaddingVertical(20);

            // TARJETA RESUMEN

            column.Item()
                .AlignCenter()
                .Width(250)
                .Background("#F5F7FA")
                .Border(1)
                .BorderColor("#D9E2EC")
                .PaddingVertical(12)
                .PaddingHorizontal(20)
                .Text(text =>
                {
                    text.Span("Total de Operadores: ")
                        .Bold()
                        .FontSize(15);

                    text.Span(operadores.Count.ToString())
                        .Bold()
                        .FontColor(Colors.Blue.Darken2)
                        .FontSize(16);
                });

            column.Item().PaddingVertical(20);

            // TABLA

            column.Item()
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(45);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        void Encabezado(string texto)
                        {
                            header.Cell()
                                .Background("#D9E2EC")
                                .Border(1)
                                .BorderColor("#B8C4CE")
                                .PaddingVertical(8)
                                .PaddingHorizontal(6)
                                .AlignCenter()
                                .Text(texto)
                                .Bold()
                                .FontSize(10)
                                .FontColor("#0F2F44");
                        }

                        Encabezado("ID");
                        Encabezado("NOMBRE");
                        Encabezado("MÁQUINA");
                        Encabezado("FRENTE");
                    });

                    int fila = 0;

                    foreach (var item in operadores)
                    {
                        string color = fila % 2 == 0
                            ? "#FFFFFF"
                            : "#F7F9FB";

                        fila++;

                        void Celda(string valor, bool centro = false)
                        {
                            if (centro)
                            {
                                table.Cell()
                                    .Background(color)
                                    .Border(1)
                                    .BorderColor("#E1E7EC")
                                    .PaddingVertical(8)
                                    .PaddingHorizontal(6)
                                    .AlignCenter()
                                    .Text(valor)
                                    .FontSize(10)
                                    .FontColor("#2C3E50");
                            }
                            else
                            {
                                table.Cell()
                                    .Background(color)
                                    .Border(1)
                                    .BorderColor("#E1E7EC")
                                    .PaddingVertical(8)
                                    .PaddingHorizontal(6)
                                    .Text(valor)
                                    .FontSize(10)
                                    .FontColor("#2C3E50");
                            }
                        }

                        Celda(item.Id.ToString(), true);

                        Celda(item.Nombre);

                        Celda(item.Maquina?.Nombre ?? "Sin asignar");

                        Celda(item.FrenteOperacional);
                    }
                });
        });
    });

        var bytes = pdf.GeneratePdf();

        return File(
            bytes,
            "application/pdf",
            "Operadores.pdf");
    }

    public async Task<IActionResult> Index(string buscar,int? maquinaId,string frente)
    {
        var operadores = _context.Operadores
            .Include(o => o.Maquina)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            operadores = operadores.Where(o =>
                o.Nombre.Contains(buscar));
        }

        if (maquinaId.HasValue)
        {
            operadores = operadores.Where(o =>
                o.MaquinaId == maquinaId);
        }

        if (!string.IsNullOrWhiteSpace(frente))
        {
            operadores = operadores.Where(o =>
                o.FrenteOperacional == frente);
        }

        ViewBag.Maquinas = new SelectList(
            await _context.Maquinas.OrderBy(m => m.Nombre).ToListAsync(),
            "Id",
            "Nombre",
            maquinaId);

        ViewBag.Buscar = buscar;
        ViewBag.Frente = frente;

        return View(await operadores.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Operador operador)
    {
        if (ModelState.IsValid)
        {
            _context.Operadores.Add(operador);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Maquinas = new SelectList(
            await _context.Maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync(),
            "Id",
            "Nombre"
        );

        return View("Index", await _context.Operadores
            .Include(o => o.Maquina)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CrearModal([FromBody] Operador operador)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Operadores.Add(operador);

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var operador = await _context.Operadores.FindAsync(id);

        if (operador == null)
            return NotFound();

        ViewBag.Maquinas = new SelectList(
            await _context.Maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync(),
            "Id",
            "Nombre"
        );

        return View(operador);
    }

    [HttpPost]
    public async Task<IActionResult> Editar([FromBody] Operador operador)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Maquinas = new SelectList(
                await _context.Maquinas.ToListAsync(),
                "Id",
                "Nombre"
            );

            return View(operador);
        }

        _context.Operadores.Update(operador);
        await _context.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Ok();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        var operador = await _context.Operadores.FindAsync(id);

        if (operador != null)
        {
            _context.Operadores.Remove(operador);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
