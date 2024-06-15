using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixPhaseName2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhases_Pharses_PhaseId",
                table: "ProductPhases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pharses",
                table: "Pharses");

            migrationBuilder.RenameTable(
                name: "Pharses",
                newName: "Phases");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Phases",
                table: "Phases",
                column: "Id");

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
                name: "FK_ProductPhases_Phases_PhaseId",
                table: "ProductPhases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Phases",
                table: "Phases");

            migrationBuilder.RenameTable(
                name: "Phases",
                newName: "Pharses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pharses",
                table: "Pharses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhases_Pharses_PhaseId",
                table: "ProductPhases",
                column: "PhaseId",
                principalTable: "Pharses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
