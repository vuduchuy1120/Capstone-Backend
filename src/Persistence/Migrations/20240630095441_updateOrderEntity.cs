using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KindOfShipOrder",
                table: "ShipOrders",
                newName: "DeliveryMethod");

            migrationBuilder.AddColumn<int>(
                name: "ShippedQuantity",
                table: "OrderDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippedQuantity",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "DeliveryMethod",
                table: "ShipOrders",
                newName: "KindOfShipOrder");
        }
    }
}
