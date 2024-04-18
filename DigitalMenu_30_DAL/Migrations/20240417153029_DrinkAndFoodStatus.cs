using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMenu_30_DAL.Migrations
{
    /// <inheritdoc />
    public partial class DrinkAndFoodStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Orders",
                newName: "FoodStatus");

            migrationBuilder.AddColumn<int>(
                name: "DrinkStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrinkStatus",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "FoodStatus",
                table: "Orders",
                newName: "Status");
        }
    }
}
