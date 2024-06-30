using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateShipOrderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemStatus",
                table: "ShipOrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "KindOfShipOrder",
                table: "ShipOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FailureQuantity",
                table: "ProductPhases",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KindOfShipOrder",
                table: "ShipOrders");

            migrationBuilder.DropColumn(
                name: "FailureQuantity",
                table: "ProductPhases");

            migrationBuilder.AddColumn<int>(
                name: "ItemStatus",
                table: "ShipOrderDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
