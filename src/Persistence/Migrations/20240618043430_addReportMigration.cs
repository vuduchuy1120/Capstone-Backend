using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addReportMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Company_CompanyId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Products_ProductId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Sets_SetId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetail_MaterialHistories_MaterialHistoryId",
                table: "ShipmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetail_Phases_PhaseId",
                table: "ShipmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetail_Products_ProductId",
                table: "ShipmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetail_Sets_SetId",
                table: "ShipmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetail_ShipOrder_ShipOrderId",
                table: "ShipmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetail_Shipment_ShipmentId",
                table: "ShipmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipOrder_Order_OrderId",
                table: "ShipOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipOrder",
                table: "ShipOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipmentDetail",
                table: "ShipmentDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shipment",
                table: "Shipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.RenameTable(
                name: "ShipOrder",
                newName: "ShipOrders");

            migrationBuilder.RenameTable(
                name: "ShipmentDetail",
                newName: "ShipmentDetails");

            migrationBuilder.RenameTable(
                name: "Shipment",
                newName: "Shipments");

            migrationBuilder.RenameTable(
                name: "OrderDetail",
                newName: "OrderDetails");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "Company",
                newName: "Companies");

            migrationBuilder.RenameIndex(
                name: "IX_ShipOrder_OrderId",
                table: "ShipOrders",
                newName: "IX_ShipOrders_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetail_ShipOrderId",
                table: "ShipmentDetails",
                newName: "IX_ShipmentDetails_ShipOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetail_ShipmentId",
                table: "ShipmentDetails",
                newName: "IX_ShipmentDetails_ShipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetail_SetId",
                table: "ShipmentDetails",
                newName: "IX_ShipmentDetails_SetId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetail_ProductId",
                table: "ShipmentDetails",
                newName: "IX_ShipmentDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetail_PhaseId",
                table: "ShipmentDetails",
                newName: "IX_ShipmentDetails_PhaseId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetail_MaterialHistoryId",
                table: "ShipmentDetails",
                newName: "IX_ShipmentDetails_MaterialHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetail_SetId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_SetId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CompanyId",
                table: "Orders",
                newName: "IX_Orders_CompanyId");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipOrders",
                table: "ShipOrders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipmentDetails",
                table: "ShipmentDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shipments",
                table: "Shipments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Sets_SetId",
                table: "OrderDetails",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Companies_CompanyId",
                table: "Orders",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetails_MaterialHistories_MaterialHistoryId",
                table: "ShipmentDetails",
                column: "MaterialHistoryId",
                principalTable: "MaterialHistories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetails_Phases_PhaseId",
                table: "ShipmentDetails",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetails_Products_ProductId",
                table: "ShipmentDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetails_Sets_SetId",
                table: "ShipmentDetails",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetails_ShipOrders_ShipOrderId",
                table: "ShipmentDetails",
                column: "ShipOrderId",
                principalTable: "ShipOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetails_Shipments_ShipmentId",
                table: "ShipmentDetails",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipOrders_Orders_OrderId",
                table: "ShipOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Sets_SetId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Companies_CompanyId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetails_MaterialHistories_MaterialHistoryId",
                table: "ShipmentDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetails_Phases_PhaseId",
                table: "ShipmentDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetails_Products_ProductId",
                table: "ShipmentDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetails_Sets_SetId",
                table: "ShipmentDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetails_ShipOrders_ShipOrderId",
                table: "ShipmentDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetails_Shipments_ShipmentId",
                table: "ShipmentDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipOrders_Orders_OrderId",
                table: "ShipOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Users_CompanyId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipOrders",
                table: "ShipOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shipments",
                table: "Shipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipmentDetails",
                table: "ShipmentDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "ShipOrders",
                newName: "ShipOrder");

            migrationBuilder.RenameTable(
                name: "Shipments",
                newName: "Shipment");

            migrationBuilder.RenameTable(
                name: "ShipmentDetails",
                newName: "ShipmentDetail");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");

            migrationBuilder.RenameTable(
                name: "OrderDetails",
                newName: "OrderDetail");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Company");

            migrationBuilder.RenameIndex(
                name: "IX_ShipOrders_OrderId",
                table: "ShipOrder",
                newName: "IX_ShipOrder_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetails_ShipOrderId",
                table: "ShipmentDetail",
                newName: "IX_ShipmentDetail_ShipOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetails_ShipmentId",
                table: "ShipmentDetail",
                newName: "IX_ShipmentDetail_ShipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetails_SetId",
                table: "ShipmentDetail",
                newName: "IX_ShipmentDetail_SetId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetails_ProductId",
                table: "ShipmentDetail",
                newName: "IX_ShipmentDetail_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetails_PhaseId",
                table: "ShipmentDetail",
                newName: "IX_ShipmentDetail_PhaseId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentDetails_MaterialHistoryId",
                table: "ShipmentDetail",
                newName: "IX_ShipmentDetail_MaterialHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CompanyId",
                table: "Order",
                newName: "IX_Order_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_SetId",
                table: "OrderDetail",
                newName: "IX_OrderDetail_SetId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetail",
                newName: "IX_OrderDetail_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetail",
                newName: "IX_OrderDetail_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipOrder",
                table: "ShipOrder",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shipment",
                table: "Shipment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipmentDetail",
                table: "ShipmentDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetail",
                table: "OrderDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Company_CompanyId",
                table: "Order",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Products_ProductId",
                table: "OrderDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Sets_SetId",
                table: "OrderDetail",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetail_MaterialHistories_MaterialHistoryId",
                table: "ShipmentDetail",
                column: "MaterialHistoryId",
                principalTable: "MaterialHistories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetail_Phases_PhaseId",
                table: "ShipmentDetail",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetail_Products_ProductId",
                table: "ShipmentDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetail_Sets_SetId",
                table: "ShipmentDetail",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetail_ShipOrder_ShipOrderId",
                table: "ShipmentDetail",
                column: "ShipOrderId",
                principalTable: "ShipOrder",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetail_Shipment_ShipmentId",
                table: "ShipmentDetail",
                column: "ShipmentId",
                principalTable: "Shipment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipOrder_Order_OrderId",
                table: "ShipOrder",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
