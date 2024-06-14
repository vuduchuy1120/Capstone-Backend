using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updatePhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProducts_Pharses_PharseId",
                table: "EmployeeProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhases_Pharses_PharseId",
                table: "ProductPhases");

            migrationBuilder.DropTable(
                name: "Pharses");

            migrationBuilder.RenameColumn(
                name: "PharseId",
                table: "ProductPhases",
                newName: "PhaseId");

            migrationBuilder.RenameColumn(
                name: "PharseId",
                table: "EmployeeProducts",
                newName: "PhaseId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeProducts_PharseId",
                table: "EmployeeProducts",
                newName: "IX_EmployeeProducts_PhaseId");

            migrationBuilder.CreateTable(
                name: "Phases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phases", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProducts_Phases_PhaseId",
                table: "EmployeeProducts",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhases_Phases_PhaseId",
                table: "ProductPhases",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProducts_Phases_PhaseId",
                table: "EmployeeProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhases_Phases_PhaseId",
                table: "ProductPhases");

            migrationBuilder.DropTable(
                name: "Phases");

            migrationBuilder.RenameColumn(
                name: "PhaseId",
                table: "ProductPhases",
                newName: "PharseId");

            migrationBuilder.RenameColumn(
                name: "PhaseId",
                table: "EmployeeProducts",
                newName: "PharseId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeProducts_PhaseId",
                table: "EmployeeProducts",
                newName: "IX_EmployeeProducts_PharseId");

            migrationBuilder.CreateTable(
                name: "Pharses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharses", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProducts_Pharses_PharseId",
                table: "EmployeeProducts",
                column: "PharseId",
                principalTable: "Pharses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhases_Pharses_PharseId",
                table: "ProductPhases",
                column: "PharseId",
                principalTable: "Pharses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
