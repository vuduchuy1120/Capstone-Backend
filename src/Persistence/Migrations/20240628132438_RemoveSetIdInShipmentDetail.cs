using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSetIdInShipmentDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentDetails_Sets_SetId",
                table: "ShipmentDetails");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentDetails_SetId",
                table: "ShipmentDetails");

            migrationBuilder.DropColumn(
                name: "SetId",
                table: "ShipmentDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SetId",
                table: "ShipmentDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetails_SetId",
                table: "ShipmentDetails",
                column: "SetId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentDetails_Sets_SetId",
                table: "ShipmentDetails",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id");
        }
    }
}
