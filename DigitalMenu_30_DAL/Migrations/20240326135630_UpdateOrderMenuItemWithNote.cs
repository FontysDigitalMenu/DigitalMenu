using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMenu_30_DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderMenuItemWithNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "OrderMenuItems",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "OrderMenuItems");
        }
    }
}
