using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class CrearFrenteOperacional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrenteOperacional",
                table: "Operadores");

            migrationBuilder.AddColumn<int>(
                name: "FrenteOperacionalId",
                table: "Operadores",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FrentesOperacionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrentesOperacionales", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FrentesOperacionales",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "FRENTE PADEL" },
                    { 2, "FRENTE PANAMÁ" },
                    { 3, "CANTERA RIO SECO" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Operadores_FrenteOperacionalId",
                table: "Operadores",
                column: "FrenteOperacionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operadores_FrentesOperacionales_FrenteOperacionalId",
                table: "Operadores",
                column: "FrenteOperacionalId",
                principalTable: "FrentesOperacionales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operadores_FrentesOperacionales_FrenteOperacionalId",
                table: "Operadores");

            migrationBuilder.DropTable(
                name: "FrentesOperacionales");

            migrationBuilder.DropIndex(
                name: "IX_Operadores_FrenteOperacionalId",
                table: "Operadores");

            migrationBuilder.DropColumn(
                name: "FrenteOperacionalId",
                table: "Operadores");

            migrationBuilder.AddColumn<string>(
                name: "FrenteOperacional",
                table: "Operadores",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
