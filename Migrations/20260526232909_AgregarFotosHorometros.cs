using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFotosHorometros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FotoHorometroFinal",
                table: "ReportesMaquinaria",
                newName: "FotoHorometroFinal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FotoHorometroFinal",
                table: "ReportesMaquinaria",
                newName: "FotoHorometroFinal");
        }
    }
}
