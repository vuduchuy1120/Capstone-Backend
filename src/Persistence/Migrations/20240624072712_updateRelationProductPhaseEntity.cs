using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateRelationProductPhaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductPhases",
                table: "ProductPhases");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductPhases",
                table: "ProductPhases",
                columns: new[] { "PhaseId", "ProductId", "CompanyId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductPhases",
                table: "ProductPhases");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductPhases",
                table: "ProductPhases",
                columns: new[] { "PhaseId", "ProductId" });
        }
    }
}
