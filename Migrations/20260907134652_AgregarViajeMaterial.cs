using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class AgregarViajeMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViajesMateriales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Recibidor = table.Column<string>(type: "text", nullable: false),
                    FrenteOperacionalId = table.Column<int>(type: "integer", nullable: false),
                    Zona = table.Column<string>(type: "text", nullable: false),
                    Tramo = table.Column<string>(type: "text", nullable: false),
                    NumeroRecibo = table.Column<string>(type: "text", nullable: false),
                    Placa = table.Column<string>(type: "text", nullable: false),
                    VolumenM3 = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Material = table.Column<string>(type: "text", nullable: false),
                    HoraSalida = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    HoraLlegada = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Origen = table.Column<string>(type: "text", nullable: false),
                    AbscisaDescargue = table.Column<string>(type: "text", nullable: false),
                    Destino = table.Column<string>(type: "text", nullable: false),
                    EvidenciaDescarga = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViajesMateriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViajesMateriales_FrentesOperacionales_FrenteOperacionalId",
                        column: x => x.FrenteOperacionalId,
                        principalTable: "FrentesOperacionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViajesMateriales_FrenteOperacionalId",
                table: "ViajesMateriales",
                column: "FrenteOperacionalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViajesMateriales");
        }
    }
}
