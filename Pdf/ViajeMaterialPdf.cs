using FormularioMaquinaria.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace FormularioMaquinaria.Pdf
{
    public static class ViajeMaterialPdf
    {
        public static IDocument Generar(IEnumerable<ViajeMaterial> viajes)
        {
            var listaViajes = viajes?.ToList() ?? new List<ViajeMaterial>();
            int totalViajes = listaViajes.Count;
            decimal volumenTotal = listaViajes.Sum(v => v.VolumenM3);

            return PdfTemplate.Crear(
                "Reporte de Viajes de Material",
                "Control de volumen y registros",
                "",
                container =>
                {
                    container.Column(col =>
                    {
                        // Resumen superior
                        col.Item().PaddingBottom(15).Row(row =>
                        {
                            row.RelativeItem().Text($"Total de Viajes: {totalViajes}").Bold().FontSize(10);
                            row.RelativeItem().AlignRight().Text($"Volumen Total: {volumenTotal:N2} m³").Bold().FontSize(10).FontColor("#28a745");
                        });

                        // Tabla de datos
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(52);   // Fecha (dd/MM/yyyy)
                                columns.RelativeColumn(1.4f); // Recibidor
                                columns.RelativeColumn(1.2f); // Frente
                                columns.ConstantColumn(42);   // Zona (Ampliado para que no se baje el texto)
                                columns.ConstantColumn(42);   // Tramo (Ampliado para que no se baje el texto)
                                columns.ConstantColumn(45);   // N.º Recibo
                                columns.ConstantColumn(50);   // Placa
                                columns.RelativeColumn(1f);     // Material
                                columns.ConstantColumn(55);   // Volumen
                            });

                            // Cabecera de la tabla
                            table.Header(header =>
                            {
                                header.Cell().Background("#0f2f44").Padding(4).Text("Fecha").FontColor("#fff").Bold().FontSize(8);
                                header.Cell().Background("#0f2f44").Padding(4).Text("Recibidor").FontColor("#fff").Bold().FontSize(8);
                                header.Cell().Background("#0f2f44").Padding(4).Text("Frente").FontColor("#fff").Bold().FontSize(8);
                                header.Cell().Background("#0f2f44").Padding(4).Text("Zona").FontColor("#fff").Bold().FontSize(8);
                                header.Cell().Background("#0f2f44").Padding(4).Text("Tramo").FontColor("#fff").Bold().FontSize(8);
                                header.Cell().Background("#0f2f44").Padding(4).Text("Recibo").FontColor("#fff").Bold().FontSize(8);
                                header.Cell().Background("#0f2f44").Padding(4).Text("Placa").FontColor("#fff").Bold().FontSize(8);
                                header.Cell().Background("#0f2f44").Padding(4).Text("Material").FontColor("#fff").Bold().FontSize(8);
                                header.Cell().Background("#0f2f44").Padding(4).Text("Volumen").FontColor("#fff").Bold().FontSize(8);
                            });

                            // Filas de datos con protección de nulos (?? "")
                            foreach (var v in listaViajes)
                            {
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(3).Text(v.Fecha.ToString("dd/MM/yyyy")).FontSize(7);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(3).Text(v.Recibidor?.Nombre ?? "").FontSize(7);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(3).Text(v.FrenteOperacional?.Nombre ?? "").FontSize(7);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(3).Text(v.Zona ?? "").FontSize(7);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(3).Text(v.Tramo ?? "").FontSize(7);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(3).Text(v.NumeroRecibo ?? "").FontSize(7);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(3).Text(v.Placa ?? "").FontSize(7);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(3).Text(v.Material ?? "").FontSize(7);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(3).Text($"{v.VolumenM3:N2} m³").FontSize(7);
                            }
                        });
                    });
                }
            );
        }
    }
}