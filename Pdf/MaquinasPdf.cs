using FormularioMaquinaria.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FormularioMaquinaria.Pdf
{
    public static class MaquinasPdf
    {
        public static Document Generar(
            List<Maquina> maquinas,
            string logoPath)
                {
                    return PdfTemplate.Crear(
                        "GESTIÓN DE MÀQUINAS",
                        "Administración de maquinas registradas",
                        logoPath,
                        contenido =>
                        {
                            contenido.Column(column =>
                            {
                                // Fecha
                                    //column.Item()
                                    //.AlignRight()
                                    //.Text($"Valledupar, {DateTime.Now:dd/MM/yyyy}")
                                    //.FontSize(11);

                                // Referencia
                                //column.Item().PaddingTop(20);

                                //column.Item().Text(text =>
                                //{
                                //    text.Span("REFERENCIA: ")
                                //        .Bold();

                                //    text.Span("Listado General de maquinas");
                                //});

                                // Asunto
                                //column.Item().PaddingTop(12);

                                //column.Item().Text(text =>
                                //{
                                //    text.Span("ASUNTO: ")
                                //        .Bold();

                                //    text.Span("Maquinas registrados en el sistema.");
                                //});

                                // Información
                                column.Item().PaddingTop(15);

                                AgregarInformacion(column, maquinas);

                                // Tarjeta resumen
                                column.Item().PaddingTop(15);

                                AgregarResumen(column, maquinas);

                                // Tabla
                                column.Item().PaddingTop(15);

                                AgregarTabla(column, maquinas);

                                // Firma
                                //column.Item().PaddingTop(40);

                                //AgregarFirma(column);

                                // Nota
                                //column.Item().PaddingTop(20);

                                //AgregarNota(column);
                            });
                        });
                }

        private static void AgregarInformacion(
            ColumnDescriptor column,
            List<Maquina> maquinas)
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

                    Celda("Documento", "Listado de Maquinarias");

                    Celda("Empresa", "Avenida Del Río");

                    Celda("Fecha",
                        DateTime.Now.ToString("dd/MM/yyyy"));

                    Celda("Hora",
                        DateTime.Now.ToString("HH:mm"));

                    Celda("Ciudad",
                         "Valledupar/Cesar");

                    Celda("Generado por",
                        "Sistema de Gestión Maquinarias");
                });
        }

        private static void AgregarResumen(
            ColumnDescriptor column,
            List<Maquina> maquinas)
                {
                    column.Item()

                        .AlignCenter()

                        .Width(220)

                        .Border(1)

                        .BorderColor("#0F4C81")

                        .Background("#F8FBFE")

                        .PaddingVertical(12)

                        .PaddingHorizontal(20)

                        .Column(card =>
                        {
                            card.Item()

                                .AlignCenter()

                                .Text("TOTAL DE MAQUINARIAS")

                                .Bold()

                                .FontSize(11)

                                .FontColor("#0F4C81");

                            card.Item()

                                .PaddingTop(8)

                                .AlignCenter()

                                .Text(maquinas.Count.ToString())

                                .Bold()

                                .FontSize(28)

                                .FontColor("#0F4C81");

                            card.Item()

                                .PaddingTop(5)

                                .AlignCenter()

                                .Text("Registradas en el sistema")

                                .FontSize(9)

                                .FontColor(Colors.Grey.Darken1);
                        });
                }

        private static void AgregarTabla(
            ColumnDescriptor column,
            List<Maquina> maquinas)
        {
            column.Item()

                .Border(1)

                .BorderColor("#C7D2DB")

                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.RelativeColumn();
                        columns.ConstantColumn(130);
                    });

                    // ===== ENCABEZADOS =====

                    table.Header(header =>
                    {
                        void HeaderCell(string texto)
                        {
                            header.Cell()

                                .Background("#D9E6F2")

                                .Border(1)

                                .BorderColor("#BFCAD6")

                                .PaddingVertical(8)

                                .PaddingHorizontal(6)

                                .AlignCenter()

                                .Text(texto)

                                .Bold()

                                .FontSize(10)

                                .FontColor("#0F2F44");
                        }

                        HeaderCell("ID");
                        HeaderCell("MÁQUINA");
                        HeaderCell("ESTADO");
                    });

                    // ===== FILAS =====

                    int fila = 0;

                    foreach (var maquina in maquinas)
                    {
                        string color =
                            fila % 2 == 0
                            ? "#FFFFFF"
                            : "#F8FAFC";

                        fila++;

                        void BodyCell(
                            string texto,
                            bool centro = false)
                        {
                            var cell = table.Cell()
                                .Background(color)
                                .Border(1)
                                .BorderColor("#D9E2EC")
                                .PaddingVertical(7)
                                .PaddingHorizontal(6);

                            if (centro)
                            {
                                cell = cell.AlignCenter(); 
                            }

                            cell.Text(texto)
                                .FontSize(9.5f)
                                .FontColor("#2C3E50");
                        }

                        BodyCell(
                            maquina.Id.ToString(),
                            true);

                        BodyCell(
                            maquina.Nombre ?? "Sin nombre");

                        BodyCell(
                            maquina.Estado == "0"
                                ? "OPERATIVA"
                                : "NO OPERATIVA",
                            true);
                    }
                });
        }
        private static void AgregarFirma(ColumnDescriptor column)
        {
            column.Item().PaddingTop(120);

            column.Item()
                .AlignCenter()
                .Width(220)
                .LineHorizontal(1);

            column.Item()
                .PaddingTop(8)
                .AlignCenter()
                .Text("Administrador del Sistema")
                .Bold()
                .FontSize(11);

            column.Item()
                .AlignCenter()
                .Text("Sistema de Gestión de Maquinaria")
                .FontSize(10);

            column.Item()
                .AlignCenter()
                .Text("AV Río")
                .FontSize(10)
                .FontColor("#6C757D");
        }

        private static void AgregarNota(ColumnDescriptor column)
        {
            column.Item().PaddingTop(25);

            column.Item()

                .Border(1)

                .BorderColor("#D9E2EC")

                .Background("#F8FAFC")

                .Padding(12)

                .Text(text =>
                {
                    text.Span("NOTA: ")
                        .Bold()
                        .FontSize(9)
                        .FontColor("#4B5563");

                    text.Span("Este documento fue generado automáticamente por el Sistema de Gestión de Maquinaria. La información aquí contenida corresponde a los registros existentes en la base de datos al momento de su generación.")
                    .FontSize(9)
                    .FontColor("#4B5563");
                });
        }
    }
}