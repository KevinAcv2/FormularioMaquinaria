using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEvaluacionOperador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CuidadoEquipo",
                table: "ReportesMaquinaria",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Horario",
                table: "ReportesMaquinaria",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ManejoMaquinaria",
                table: "ReportesMaquinaria",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionSupervisor",
                table: "ReportesMaquinaria",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Productividad",
                table: "ReportesMaquinaria",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReporteNovedades",
                table: "ReportesMaquinaria",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeguridadIndustrial",
                table: "ReportesMaquinaria",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuidadoEquipo",
                table: "ReportesMaquinaria");

            migrationBuilder.DropColumn(
                name: "Horario",
                table: "ReportesMaquinaria");

            migrationBuilder.DropColumn(
                name: "ManejoMaquinaria",
                table: "ReportesMaquinaria");

            migrationBuilder.DropColumn(
                name: "ObservacionSupervisor",
                table: "ReportesMaquinaria");

            migrationBuilder.DropColumn(
                name: "Productividad",
                table: "ReportesMaquinaria");

            migrationBuilder.DropColumn(
                name: "ReporteNovedades",
                table: "ReportesMaquinaria");

            migrationBuilder.DropColumn(
                name: "SeguridadIndustrial",
                table: "ReportesMaquinaria");
        }
    }
}
