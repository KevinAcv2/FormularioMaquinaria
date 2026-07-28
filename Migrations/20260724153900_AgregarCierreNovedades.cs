using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCierreNovedades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoNovedad",
                table: "NovedadesOperacion",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "NovedadesOperacion",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciaFin",
                table: "NovedadesOperacion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciaInicio",
                table: "NovedadesOperacion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionFin",
                table: "NovedadesOperacion",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvidenciaFin",
                table: "NovedadesOperacion");

            migrationBuilder.DropColumn(
                name: "EvidenciaInicio",
                table: "NovedadesOperacion");

            migrationBuilder.DropColumn(
                name: "ObservacionFin",
                table: "NovedadesOperacion");

            migrationBuilder.AlterColumn<string>(
                name: "TipoNovedad",
                table: "NovedadesOperacion",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "NovedadesOperacion",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
