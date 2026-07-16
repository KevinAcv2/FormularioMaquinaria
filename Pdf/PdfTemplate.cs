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
            var encabezado = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "pdf",
                "encabezado.png");

            var pie = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "pdf",
                "pie.png");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);

                    page.Margin(0);

                    // ENCABEZADO
                    page.Header().Element(header =>
                    {
                        header.Image(encabezado)
                              .FitWidth();
                    });

                    // CONTENIDO
                    page.Content()
                        .PaddingHorizontal(40)
                        .PaddingTop(70)
                        .Element(contenido);

                    // PIE
                    page.Footer().Element(footer =>
                    {
                        footer.Image(pie)
                              .FitWidth();
                    });
                });
            });
        }
    }
}