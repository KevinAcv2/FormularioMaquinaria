using ClosedXML.Excel;
using FormularioMaquinaria.Models;
using FormularioMaquinaria.Pdf;
using Maquinarias.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FormularioMaquinaria.Controllers;

public class OperadoresController : Controller
{
    private readonly AppDbContext _context;

    public OperadoresController(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // INDEX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index(
        string? buscar,
        int? maquinaId,
        int? frenteId)
    {
        var operadores = _context.Operadores
            .Include(o => o.Maquina)
            .Include(o => o.FrenteOperacional)
            .AsQueryable();

        // BUSCAR POR NOMBRE
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            buscar = buscar.Trim();

            operadores = operadores.Where(o =>
                o.Nombre.Contains(buscar));
        }

        // FILTRAR POR MÁQUINA
        if (maquinaId.HasValue)
        {
            operadores = operadores.Where(o =>
                o.MaquinaId == maquinaId.Value);
        }

        // FILTRAR POR FRENTE
        if (frenteId.HasValue)
        {
            operadores = operadores.Where(o =>
                o.FrenteOperacionalId == frenteId.Value);
        }

        // LISTA DE MÁQUINAS
        ViewBag.Maquinas = new SelectList(
            await _context.Maquinas
                .OrderBy(m => m.Nombre)
                .ToListAsync(),
            "Id",
            "Nombre",
            maquinaId
        );

        // CONSERVAR VALORES DE LOS FILTROS
        ViewBag.Buscar = buscar;
        ViewBag.MaquinaId = maquinaId;
        ViewBag.FrenteId = frenteId;

        // LISTA DE FRENTES
        ViewBag.Frentes = await _context.FrentesOperacionales
            .OrderBy(f => f.Nombre)
            .ToListAsync();

        var resultado = await operadores
            .OrderBy(o => o.Nombre)
            .ToListAsync();

        return View(resultado);
    }


    // =========================================================
    // CREAR OPERADOR DESDE MODAL
    // =========================================================

    [HttpPost]
    public async Task<IActionResult> CrearModal(
        [FromBody] Operador operador)
    {
        if (operador == null)
        {
            return BadRequest(new
            {
                mensaje = "Los datos del operador son obligatorios."
            });
        }

        if (string.IsNullOrWhiteSpace(operador.Nombre))
        {
            return BadRequest(new
            {
                mensaje = "El nombre del operador es obligatorio."
            });
        }

        operador.Nombre = operador.Nombre.Trim();

        // Verificar que la máquina exista si fue seleccionada
        if (operador.MaquinaId.HasValue)
        {
            bool maquinaExiste = await _context.Maquinas
                .AnyAsync(m => m.Id == operador.MaquinaId.Value);

            if (!maquinaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "La máquina seleccionada no existe."
                });
            }
        }

        // Verificar que el frente exista si fue seleccionado
        if (operador.FrenteOperacionalId.HasValue)
        {
            bool frenteExiste = await _context.FrentesOperacionales
                .AnyAsync(f =>
                    f.Id == operador.FrenteOperacionalId.Value);

            if (!frenteExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El frente seleccionado no existe."
                });
            }
        }

        // Evitar operadores duplicados
        bool operadorExiste = await _context.Operadores
            .AnyAsync(o =>
                o.Nombre.ToLower() == operador.Nombre.ToLower());

        if (operadorExiste)
        {
            return BadRequest(new
            {
                mensaje = "Ya existe un operador con ese nombre."
            });
        }

        _context.Operadores.Add(operador);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            exito = true,
            mensaje = "Operador creado correctamente."
        });
    }


    // =========================================================
    // EDITAR OPERADOR DESDE MODAL
    // =========================================================

    [HttpPost]
    public async Task<IActionResult> Editar(
        [FromBody] Operador operador)
    {
        if (operador == null)
        {
            return BadRequest(new
            {
                mensaje = "Los datos del operador son obligatorios."
            });
        }

        if (operador.Id <= 0)
        {
            return BadRequest(new
            {
                mensaje = "El ID del operador no es válido."
            });
        }

        if (string.IsNullOrWhiteSpace(operador.Nombre))
        {
            return BadRequest(new
            {
                mensaje = "El nombre del operador es obligatorio."
            });
        }

        // Buscar el operador REAL en la base de datos
        var operadorExistente = await _context.Operadores
            .FirstOrDefaultAsync(o => o.Id == operador.Id);

        if (operadorExistente == null)
        {
            return NotFound(new
            {
                mensaje = "El operador no existe."
            });
        }

        // Verificar máquina
        if (operador.MaquinaId.HasValue)
        {
            bool maquinaExiste = await _context.Maquinas
                .AnyAsync(m => m.Id == operador.MaquinaId.Value);

            if (!maquinaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "La máquina seleccionada no existe."
                });
            }
        }

        // Verificar frente
        if (operador.FrenteOperacionalId.HasValue)
        {
            bool frenteExiste = await _context.FrentesOperacionales
                .AnyAsync(f =>
                    f.Id == operador.FrenteOperacionalId.Value);

            if (!frenteExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El frente seleccionado no existe."
                });
            }
        }

        // Verificar nombre duplicado
        bool nombreDuplicado = await _context.Operadores
            .AnyAsync(o =>
                o.Id != operador.Id &&
                o.Nombre.ToLower() == operador.Nombre.Trim().ToLower());

        if (nombreDuplicado)
        {
            return BadRequest(new
            {
                mensaje = "Ya existe otro operador con ese nombre."
            });
        }

        // ACTUALIZAR SOLO LOS CAMPOS PERMITIDOS
        operadorExistente.Nombre = operador.Nombre.Trim();
        operadorExistente.MaquinaId = operador.MaquinaId;
        operadorExistente.FrenteOperacionalId =
            operador.FrenteOperacionalId;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            exito = true,
            mensaje = "Operador actualizado correctamente."
        });
    }


    // =========================================================
    // ELIMINAR OPERADOR
    // =========================================================

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (id <= 0)
        {
            return Json(new
            {
                exito = false,
                mensaje = "El ID del operador no es válido."
            });
        }

        var operador = await _context.Operadores
            .FirstOrDefaultAsync(o => o.Id == id);

        if (operador == null)
        {
            return Json(new
            {
                exito = false,
                mensaje = "El operador no existe."
            });
        }

        _context.Operadores.Remove(operador);

        await _context.SaveChangesAsync();

        return Json(new
        {
            exito = true,
            mensaje = "Operador eliminado correctamente."
        });
    }


    // =========================================================
    // EXPORTAR EXCEL
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> ExportarExcel()
    {
        var operadores = await _context.Operadores
            .Include(o => o.Maquina)
            .Include(o => o.FrenteOperacional)
            .OrderBy(o => o.Nombre)
            .ToListAsync();

        using var workbook = new XLWorkbook();

        var hoja = workbook.Worksheets.Add("Operadores");

        // ENCABEZADOS
        hoja.Cell(1, 1).Value = "ID";
        hoja.Cell(1, 2).Value = "Nombre";
        hoja.Cell(1, 3).Value = "Máquina";
        hoja.Cell(1, 4).Value = "Frente Operacional";

        // ESTILO ENCABEZADO
        var encabezado = hoja.Range(1, 1, 1, 4);

        encabezado.Style.Font.Bold = true;
        encabezado.Style.Font.FontColor = XLColor.White;
        encabezado.Style.Fill.BackgroundColor = XLColor.SteelBlue;
        encabezado.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
        encabezado.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        int fila = 2;

        foreach (var operador in operadores)
        {
            hoja.Cell(fila, 1).Value = operador.Id;
            hoja.Cell(fila, 2).Value = operador.Nombre;
            hoja.Cell(fila, 3).Value =
                operador.Maquina?.Nombre ?? "Sin asignar";
            hoja.Cell(fila, 4).Value =
                operador.FrenteOperacional?.Nombre ?? "Sin asignar";

            fila++;
        }

        // BORDES
        var rango = hoja.Range(
            1,
            1,
            fila - 1,
            4);

        rango.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        rango.Style.Border.InsideBorder =
            XLBorderStyleValues.Thin;

        // CENTRAR ID
        hoja.Column(1).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        // AJUSTAR COLUMNAS
        hoja.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Operadores.xlsx");
    }


    // =========================================================
    // EXPORTAR PDF
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> ExportarPdf()
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        var operadores = await _context.Operadores
            .Include(o => o.Maquina)
            .Include(o => o.FrenteOperacional)
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
                    // =================================================
                    // INFORMACIÓN GENERAL
                    // =================================================

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

                            void Celda(
                                string titulo,
                                string valor)
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

                            Celda(
                                "Documento",
                                "Listado de Operadores");

                            Celda(
                                "Empresa",
                                "AV Río");

                            Celda(
                                "Fecha",
                                DateTime.Now
                                    .ToString("dd/MM/yyyy"));

                            Celda(
                                "Hora",
                                DateTime.Now
                                    .ToString("HH:mm"));

                            Celda(
                                "Total Operadores",
                                operadores.Count.ToString());

                            Celda(
                                "Generado por",
                                "Sistema de Gestión");
                        });


                    column.Item()
                        .PaddingVertical(20);


                    // =================================================
                    // TARJETA RESUMEN
                    // =================================================

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
                            text.Span(
                                    "Total de Operadores: ")
                                .Bold()
                                .FontSize(15);

                            text.Span(
                                    operadores.Count.ToString())
                                .Bold()
                                .FontColor(
                                    Colors.Blue.Darken2)
                                .FontSize(16);
                        });


                    column.Item()
                        .PaddingVertical(20);


                    // =================================================
                    // TABLA
                    // =================================================

                    column.Item()
                        .Border(1)
                        .BorderColor(
                            Colors.Grey.Lighten2)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(
                                columns =>
                                {
                                    columns.ConstantColumn(45);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(2);
                                });

                            table.Header(header =>
                            {
                                void Encabezado(
                                    string texto)
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
                                string color =
                                    fila % 2 == 0
                                        ? "#FFFFFF"
                                        : "#F7F9FB";

                                fila++;


                                void Celda(
                                    string valor,
                                    bool centro = false)
                                {
                                    var celda = table.Cell()
                                        .Background(color)
                                        .Border(1)
                                        .BorderColor("#E1E7EC")
                                        .PaddingVertical(8)
                                        .PaddingHorizontal(6);

                                    if (centro)
                                    {
                                        celda = celda.AlignCenter();
                                    }

                                    celda.Text(valor)
                                        .FontSize(10)
                                        .FontColor("#2C3E50");
                                }


                                Celda(
                                    item.Id.ToString(),
                                    true);

                                Celda(item.Nombre);

                                Celda(
                                    item.Maquina?.Nombre
                                    ?? "Sin asignar");

                                Celda(
                                    item.FrenteOperacional?.Nombre
                                    ?? "Sin asignar");
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
}