using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addForeignKeyInShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Shipments_FromId",
                table: "Shipments",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ToId",
                table: "Shipments",
                column: "ToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Companies_FromId",
                table: "Shipments",
                column: "FromId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Companies_ToId",
                table: "Shipments",
                column: "ToId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Companies_FromId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Companies_ToId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_FromId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_ToId",
                table: "Shipments");
        }
    }
}
