using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaterialAndMaterialHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "QuantityPerUnit",
                table: "Materials",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "MaterialHistories",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<double>(
                name: "QuantityInStock",
                table: "MaterialHistories",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "QuantityPerUnit",
                table: "MaterialHistories",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityInStock",
                table: "MaterialHistories");

            migrationBuilder.DropColumn(
                name: "QuantityPerUnit",
                table: "MaterialHistories");

            migrationBuilder.AlterColumn<int>(
                name: "QuantityPerUnit",
                table: "Materials",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "MaterialHistories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
