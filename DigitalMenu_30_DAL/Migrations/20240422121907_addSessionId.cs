using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMenu_30_DAL.Migrations
{
    /// <inheritdoc />
    public partial class addSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Tables",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Orders",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Orders");
        }
    }
}
