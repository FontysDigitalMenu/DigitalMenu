using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMenu_30_DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeviceIdToTableSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "CartItems",
                newName: "TableSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableSessionId",
                table: "CartItems",
                newName: "DeviceId");
        }
    }
}
