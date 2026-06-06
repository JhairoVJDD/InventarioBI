using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioBI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarComentarioAnomalia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comentario",
                table: "ConteosFisicos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comentario",
                table: "ConteosFisicos");
        }
    }
}
