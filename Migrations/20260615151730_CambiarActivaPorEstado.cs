using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormularioMaquinaria.Migrations
{
    /// <inheritdoc />
    public partial class CambiarActivaPorEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activa",
                table: "Maquinas");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Maquinas",
                newName: "Estado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Maquinas",
                newName: "Tipo");

            migrationBuilder.AddColumn<bool>(
                name: "Activa",
                table: "Maquinas",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
