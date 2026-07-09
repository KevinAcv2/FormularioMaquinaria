using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Maquinas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maquinas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportesMaquinaria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreOperador = table.Column<string>(type: "text", nullable: false),
                    FrenteOperacional = table.Column<string>(type: "text", nullable: false),
                    NombreMaquina = table.Column<string>(type: "text", nullable: false),
                    TipoMaquina = table.Column<string>(type: "text", nullable: false),
                    HorometroInicial = table.Column<decimal>(type: "numeric", nullable: false),
                    HorometroFinal = table.Column<decimal>(type: "numeric", nullable: false),
                    HorasTrabajadas = table.Column<decimal>(type: "numeric", nullable: false),
                    EstadoMaquina = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FotoHorometroInicial = table.Column<string>(type: "text", nullable: false),
                    FotoHorometroFinal = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesMaquinaria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    FrenteOperacional = table.Column<string>(type: "text", nullable: false),
                    MaquinaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operadores_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionesOperadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReporteMaquinariaId = table.Column<int>(type: "integer", nullable: false),
                    Horario = table.Column<int>(type: "integer", nullable: false),
                    ManejoMaquinaria = table.Column<int>(type: "integer", nullable: false),
                    CuidadoEquipo = table.Column<int>(type: "integer", nullable: false),
                    SeguridadIndustrial = table.Column<int>(type: "integer", nullable: false),
                    Productividad = table.Column<int>(type: "integer", nullable: false),
                    ReporteNovedades = table.Column<int>(type: "integer", nullable: false),
                    ObservacionSupervisor = table.Column<string>(type: "text", nullable: true),
                    FechaEvaluacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionesOperadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluacionesOperadores_ReportesMaquinaria_ReporteMaquinaria~",
                        column: x => x.ReporteMaquinariaId,
                        principalTable: "ReportesMaquinaria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesOperadores_ReporteMaquinariaId",
                table: "EvaluacionesOperadores",
                column: "ReporteMaquinariaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operadores_MaquinaId",
                table: "Operadores",
                column: "MaquinaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluacionesOperadores");

            migrationBuilder.DropTable(
                name: "Operadores");

            migrationBuilder.DropTable(
                name: "ReportesMaquinaria");

            migrationBuilder.DropTable(
                name: "Maquinas");
        }
    }
}
