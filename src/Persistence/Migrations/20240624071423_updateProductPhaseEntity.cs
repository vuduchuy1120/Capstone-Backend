using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateProductPhaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "ProductPhases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProductPhases_CompanyId",
                table: "ProductPhases",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhases_Companies_CompanyId",
                table: "ProductPhases",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhases_Companies_CompanyId",
                table: "ProductPhases");

            migrationBuilder.DropIndex(
                name: "IX_ProductPhases_CompanyId",
                table: "ProductPhases");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ProductPhases");
        }
    }
}
