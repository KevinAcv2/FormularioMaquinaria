using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class CrearNovedadOperacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NovedadesOperacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReporteMaquinariaId = table.Column<int>(type: "integer", nullable: false),
                    HoraInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TipoNovedad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Activa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NovedadesOperacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NovedadesOperacion_ReportesMaquinaria_ReporteMaquinariaId",
                        column: x => x.ReporteMaquinariaId,
                        principalTable: "ReportesMaquinaria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NovedadesOperacion_ReporteMaquinariaId",
                table: "NovedadesOperacion",
                column: "ReporteMaquinariaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NovedadesOperacion");
        }
    }
}
