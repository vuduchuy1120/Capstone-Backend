using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityInStock",
                table: "MaterialHistories");

            migrationBuilder.DropColumn(
                name: "QuantityPerUnit",
                table: "MaterialHistories");

            migrationBuilder.AddColumn<double>(
                name: "VAT",
                table: "Orders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "QuantityInStock",
                table: "Materials",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VAT",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "QuantityInStock",
                table: "Materials");

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
    }
}
