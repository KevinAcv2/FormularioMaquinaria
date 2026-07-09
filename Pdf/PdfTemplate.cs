using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FormularioMaquinaria.Pdf
{
    public static class PdfTemplate
    {
        public static Document Crear(
            string titulo,
            string subtitulo,
            string logoPath,
            Action<IContainer> contenido)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);

                    page.Margin(30);

                    page.Header().Element(x =>
                    {
                        CrearEncabezado(
                            x,
                            titulo,
                            subtitulo,
                            logoPath);
                    });

                    page.Content().Layers(layers =>
                    {
                        // Marca de agua (se elimina .Opacity porque IContainer no lo define;
                        // se usa un color más claro para simular transparencia)
                        layers.Layer()
                            .AlignCenter()
                            .AlignMiddle()
                            .Text("AV RÍO")
                            .Bold()
                            .FontSize(90)
                            .FontColor("#EAF6FB");

                        // Contenido principal
                        layers.PrimaryLayer()
                            .PaddingVertical(15)
                            .Column(column =>
                            {
                                contenido(column.Item());
                            });
                    });

                    page.Footer().Element(CrearPiePagina);
                });
            });
        }

        private static void CrearEncabezado(
            IContainer container,
            string titulo,
            string subtitulo,
            string logoPath)
        {
            container.Column(column =>
            {
                // Franja superior
                column.Item()
                    .Height(10)
                    .Background("#8CC5DF");

                column.Item().PaddingBottom(8);

                column.Item().Row(row =>
                {
                    // Espacio izquierdo
                    row.RelativeItem();

                    // Cuadro de información + logo
                    row.ConstantItem(260)
                        .Column(col =>
                        {
                            col.Item().Row(r =>
                            {
                                // Logo
                                r.ConstantItem(110)
                                    .Height(60)
                                    .Image(logoPath);

                                // Información del documento
                                r.RelativeItem()
                                    .Border(1)
                                    .BorderColor("#C8D2DC")
                                    .Padding(6)
                                    .Column(info =>
                                    {
                                        info.Item().Text("Código")
                                            .Bold()
                                            .FontSize(8);

                                        info.Item().Text("FO-GM-001")
                                            .FontSize(8);

                                        info.Item().PaddingTop(3);

                                        info.Item().Text("Versión")
                                            .Bold()
                                            .FontSize(8);

                                        info.Item().Text("1.0")
                                            .FontSize(8);

                                        info.Item().PaddingTop(3);

                                        info.Item().Text("Fecha")
                                            .Bold()
                                            .FontSize(8);

                                        info.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                                            .FontSize(8);
                                    });
                            });
                        });
                });

                column.Item()
                    .PaddingTop(8)
                    .Text($"Valledupar, {DateTime.Now:dd 'de' MMMM 'de' yyyy}")
                    .FontSize(10);

                column.Item().PaddingTop(20);

                column.Item()
                    .AlignCenter()
                    .Text(titulo)
                    .Bold()
                    .FontSize(20);

                column.Item()
                    .AlignCenter()
                    .Text(subtitulo)
                    .FontSize(11)
                    .FontColor(Colors.Grey.Darken1);

                column.Item().PaddingTop(20);

                column.Item()
                    .Border(1)
                    .BorderColor("#D9D9D9")
                    .Background("#FAFAFA")
                    .Padding(12)
                    .Text(text =>
                    {
                        text.Span("REFERENCIA: ")
                            .Bold();

                        text.Span("Reporte generado automáticamente por el Sistema de Gestión de Maquinaria.");
                    });

                column.Item().PaddingTop(15);

                column.Item().PaddingTop(12);

                column.Item()
                    .LineHorizontal(1)
                    .LineColor("#D9D9D9");

                column.Item().PaddingBottom(20);
            });
        }

        private static void CrearPiePagina(IContainer container)
        {
            container.Column(column =>
            {
                column.Item()
                    .LineHorizontal(1)
                    .LineColor("#D9E2EC");

                column.Item().PaddingTop(8);

                column.Item().Row(row =>
                {
                    row.RelativeItem()
                        .Text(text =>
                        {
                            text.Span("Sistema de Gestión de Maquinaria")
                                .Bold()
                                .FontSize(9);

                            text.Span(" | AV Río")
                                .FontSize(9);
                        });

                    row.RelativeItem()
                        .AlignCenter()
                        .Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                        .FontSize(9);

                    row.RelativeItem()
                        .AlignRight()
                        .Text(text =>
                        {
                            text.Span("Página ");

                            text.CurrentPageNumber();

                            text.Span(" de ");

                            text.TotalPages();
                        });
                });
            });
        }
    }
}