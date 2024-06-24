using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateShipEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReturnQuantity",
                table: "ShipmentDetails",
                newName: "ProductPhaseType");

            migrationBuilder.AddColumn<DateTime>(
                name: "ShipDate",
                table: "ShipOrders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ShipperId",
                table: "ShipOrders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ShipOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShipDate",
                table: "Shipments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ShipperId",
                table: "Shipments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Shipments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShipOrders_ShipperId",
                table: "ShipOrders",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipperId",
                table: "Shipments",
                column: "ShipperId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Users_ShipperId",
                table: "Shipments",
                column: "ShipperId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipOrders_Users_ShipperId",
                table: "ShipOrders",
                column: "ShipperId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Users_ShipperId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipOrders_Users_ShipperId",
                table: "ShipOrders");

            migrationBuilder.DropIndex(
                name: "IX_ShipOrders_ShipperId",
                table: "ShipOrders");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_ShipperId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ShipDate",
                table: "ShipOrders");

            migrationBuilder.DropColumn(
                name: "ShipperId",
                table: "ShipOrders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ShipOrders");

            migrationBuilder.DropColumn(
                name: "ShipDate",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ShipperId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Shipments");

            migrationBuilder.RenameColumn(
                name: "ProductPhaseType",
                table: "ShipmentDetails",
                newName: "ReturnQuantity");
        }
    }
}
