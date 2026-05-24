using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class InicialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportesMaquinaria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreOperador = table.Column<string>(type: "text", nullable: false),
                    NombreMaquina = table.Column<string>(type: "text", nullable: false),
                    TipoMaquina = table.Column<string>(type: "text", nullable: false),
                    HorometroInicial = table.Column<decimal>(type: "numeric", nullable: false),
                    HorometroFinal = table.Column<decimal>(type: "numeric", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesMaquinaria", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportesMaquinaria");
        }
    }
}
