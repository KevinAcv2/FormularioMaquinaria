using FormularioMaquinaria.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace FormularioMaquinaria.Pdf
{
    public static class ReporteDetallePdf
    {
        public static Document Generar(
            ReporteMaquinaria reporte,
            string logoPath)
        {
            return PdfTemplate.Crear(
                "REPORTE DE MAQUINARIA",
                "Detalle completo del reporte",
                logoPath,
                contenido =>
                {
                    contenido.Column(column =>
                    {
                        // Fecha de generación
                        column.Item()
                            .AlignRight()
                            .Text($"Valledupar, {DateTime.Now:dd/MM/yyyy}")
                            .FontSize(11);

                        column.Item().PaddingTop(20);

                        // Referencia
                        //column.Item().Text(text =>
                        //{
                        //    text.Span("REFERENCIA: ").Bold();
                        //    text.Span($"Reporte No. {reporte.Id}");
                        //});

                        //column.Item().PaddingTop(10);

                        // Asunto
                        //column.Item().Text(text =>
                        //{
                        //    text.Span("ASUNTO: ").Bold();
                        //    text.Span("Detalle del reporte de maquinaria.");
                        //});

                        column.Item().PaddingTop(20);

                        AgregarInformacionGeneral(column, reporte);

                        column.Item().PaddingTop(20);

                        AgregarHorometros(column, reporte);

                        column.Item().PaddingTop(20);

                        AgregarFotos(column, reporte);

                        column.Item().PaddingTop(20);

                        AgregarObservaciones(column, reporte);

                        column.Item().PaddingTop(20);

                        AgregarEvaluacion(column, reporte);

                        column.Item().PaddingTop(40);

                        AgregarFirma(column);

                        column.Item().PaddingTop(20);

                        AgregarNota(column);
                    });
                });
        }

        private static void AgregarInformacionGeneral(
            ColumnDescriptor column,
            ReporteMaquinaria reporte)
        {
            column.Item()

                .Border(1)

                .BorderColor("#D9E2EC")

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

                            .Padding(8)

                            .Column(col =>
                            {
                                col.Item()
                                    .Text(titulo)
                                    .Bold()
                                    .FontSize(9)
                                    .FontColor("#0F4C81");

                                col.Item()
                                    .PaddingTop(2)
                                    .Text(valor)
                                    .FontSize(10);
                            });
                    }

                    Celda("Operador", reporte.NombreOperador);

                    Celda("Máquina", reporte.NombreMaquina);

                    Celda("Tipo de Máquina", reporte.TipoMaquina);

                    Celda("Frente Operacional", reporte.FrenteOperacional);

                    Celda(
                        "Estado",
                        reporte.EstadoMaquina == 1
                            ? "Operativa"
                            : "No Operativa");

                    Celda(
                        "Fecha del Reporte",
                        reporte.Fecha.ToString("dd/MM/yyyy HH:mm"));
                });
        }

        private static void AgregarHorometros(
            ColumnDescriptor column,
            ReporteMaquinaria reporte)
        {
            column.Item()
                .Text("HORÓMETROS")
                .Bold()
                .FontSize(13)
                .FontColor("#0F4C81");

            column.Item().PaddingTop(10);

            column.Item().Row(row =>
            {
                void Tarjeta(RowDescriptor row, string titulo, string valor)
                {
                    row.RelativeItem()

                        .Border(1)

                        .BorderColor("#D9E2EC")

                        .Background("#F8FAFC")

                        .Padding(15)

                        .Column(col =>
                        {
                            col.Item()
                                .AlignCenter()
                                .Text(titulo)
                                .FontSize(10)
                                .FontColor("#6C757D");

                            col.Item()
                                .PaddingTop(8)
                                .AlignCenter()
                                .Text(valor)
                                .Bold()
                                .FontSize(24)
                                .FontColor("#0F4C81");
                        });
                }

                Tarjeta(
                    row,
                    "Horómetro Inicial",
                    reporte.HorometroInicial.ToString());

                row.ConstantItem(10);

                Tarjeta(
                    row,
                    "Horómetro Final",
                    reporte.HorometroFinal.ToString());

                row.ConstantItem(10);

                Tarjeta(
                    row,
                    "Horas Trabajadas",
                    reporte.HorasTrabajadas.ToString());
            });
        }

        private static void AgregarFotos(
            ColumnDescriptor column,
            ReporteMaquinaria reporte)
        {
            column.Item()
                .PaddingTop(20)
                .Text("FOTOGRAFÍAS DEL REPORTE")
                .Bold()
                .FontSize(13)
                .FontColor("#0F4C81");

            column.Item().PaddingTop(10);

            column.Item().Row(row =>
            {
                // FOTO INICIAL
                row.RelativeItem()
                    .Border(1)
                    .BorderColor("#D9E2EC")
                    .Padding(10)
                    .Column(col =>
                    {
                        col.Item()
                            .AlignCenter()
                            .Text("Horómetro Inicial")
                            .Bold()
                            .FontSize(10);

                        col.Item()
                            .PaddingTop(8)
                            .Height(180)
                            .Image(Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                reporte.FotoHorometroInicial.TrimStart('/')
                                    .Replace("/", Path.DirectorySeparatorChar.ToString())))
                            .FitArea();
                    });

                row.ConstantItem(15);

                // FOTO FINAL
                row.RelativeItem()
                    .Border(1)
                    .BorderColor("#D9E2EC")
                    .Padding(10)
                    .Column(col =>
                    {
                        col.Item()
                            .AlignCenter()
                            .Text("Horómetro Final")
                            .Bold()
                            .FontSize(10);

                        col.Item()
                            .PaddingTop(8)
                            .Height(180)
                            .Image(Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                reporte.FotoHorometroFinal.TrimStart('/')
                                    .Replace("/", Path.DirectorySeparatorChar.ToString())))
                            .FitArea();
                    });
            });
        }

        private static void AgregarObservaciones(
            ColumnDescriptor column,
            ReporteMaquinaria reporte)
        {
            column.Item()
                .PaddingTop(20)
                .Text("OBSERVACIONES")
                .Bold()
                .FontSize(13)
                .FontColor("#0F4C81");

            column.Item().PaddingTop(10);

            column.Item()
                .Border(1)
                .BorderColor("#D9E2EC")
                .Background("#F8FAFC")
                .Padding(15)
                .Text(string.IsNullOrWhiteSpace(reporte.Observaciones)
                    ? "No se registraron observaciones para este reporte."
                    : reporte.Observaciones)
                .FontSize(10);
        }

        private static void AgregarEvaluacion(
    ColumnDescriptor column,
    ReporteMaquinaria reporte)
        {
            column.Item()
                .PaddingTop(20)
                .Text("EVALUACIÓN DEL OPERADOR")
                .Bold()
                .FontSize(13)
                .FontColor("#0F4C81");

            column.Item().PaddingTop(10);

            if (reporte.Evaluacion == null)
            {
                column.Item()
                    .Border(1)
                    .BorderColor("#D9E2EC")
                    .Background("#F8FAFC")
                    .Padding(15)
                    .AlignCenter()
                    .Text("Este reporte aún no ha sido evaluado.")
                    .FontSize(11);

                return;
            }

            column.Item()
                .Border(1)
                .BorderColor("#D9E2EC")
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
                            .Padding(8)
                            .Column(col =>
                            {
                                col.Item()
                                    .Text(titulo)
                                    .Bold()
                                    .FontSize(9)
                                    .FontColor("#0F4C81");

                                col.Item()
                                    .PaddingTop(2)
                                    .Text(valor)
                                    .FontSize(10);
                            });
                    }

                    Celda("Horario", $"{reporte.Evaluacion.Horario}/5");
                    Celda("Manejo de Maquinaria", $"{reporte.Evaluacion.ManejoMaquinaria}/5");
                    Celda("Cuidado del Equipo", $"{reporte.Evaluacion.CuidadoEquipo}/5");
                    Celda("Seguridad Industrial", $"{reporte.Evaluacion.SeguridadIndustrial}/5");
                    Celda("Productividad", $"{reporte.Evaluacion.Productividad}/5");
                    Celda("Reporte de Novedades", $"{reporte.Evaluacion.ReporteNovedades}/5");
                    Celda("Fecha de Evaluación", reporte.Evaluacion.FechaEvaluacion.ToString("dd/MM/yyyy"));
                    Celda("Observación", reporte.Evaluacion.ObservacionSupervisor ?? "Sin observaciones");
                });
        }

        private static void AgregarFirma(ColumnDescriptor column)
        {
        }

        private static void AgregarNota(ColumnDescriptor column)
        {
        }
    }
}