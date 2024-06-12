using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixPhaseName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhases_Pharses_PharseId",
                table: "ProductPhases");

            migrationBuilder.RenameColumn(
                name: "PharseId",
                table: "ProductPhases",
                newName: "PhaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhases_Pharses_PhaseId",
                table: "ProductPhases",
                column: "PhaseId",
                principalTable: "Pharses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhases_Pharses_PhaseId",
                table: "ProductPhases");

            migrationBuilder.RenameColumn(
                name: "PhaseId",
                table: "ProductPhases",
                newName: "PharseId");

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
