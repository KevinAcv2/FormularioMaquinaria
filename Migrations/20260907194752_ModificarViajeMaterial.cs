using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class ModificarViajeMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoraSalida",
                table: "ViajesMateriales");

            migrationBuilder.DropColumn(
                name: "Recibidor",
                table: "ViajesMateriales");

            migrationBuilder.AddColumn<int>(
                name: "RecibidorId",
                table: "ViajesMateriales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Recibidores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Habilitado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recibidores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViajesMateriales_RecibidorId",
                table: "ViajesMateriales",
                column: "RecibidorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ViajesMateriales_Recibidores_RecibidorId",
                table: "ViajesMateriales",
                column: "RecibidorId",
                principalTable: "Recibidores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ViajesMateriales_Recibidores_RecibidorId",
                table: "ViajesMateriales");

            migrationBuilder.DropTable(
                name: "Recibidores");

            migrationBuilder.DropIndex(
                name: "IX_ViajesMateriales_RecibidorId",
                table: "ViajesMateriales");

            migrationBuilder.DropColumn(
                name: "RecibidorId",
                table: "ViajesMateriales");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HoraSalida",
                table: "ViajesMateriales",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Recibidor",
                table: "ViajesMateriales",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
