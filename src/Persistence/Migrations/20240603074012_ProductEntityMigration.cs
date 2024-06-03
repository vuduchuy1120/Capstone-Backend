using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProductEntityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pharse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Size = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsGroup = table.Column<bool>(type: "boolean", nullable: false),
                    IsInProcessing = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    IsBluePrint = table.Column<bool>(type: "boolean", nullable: false),
                    IsMainImage = table.Column<bool>(type: "boolean", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPharse",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    PharseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SalaryPerProduct = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPharse", x => new { x.PharseId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ProductPharse_Pharse_PharseId",
                        column: x => x.PharseId,
                        principalTable: "Pharse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPharse_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductUnit",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityPerUnit = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUnit", x => new { x.ProductId, x.SubProductId });
                    table.ForeignKey(
                        name: "FK_ProductUnit_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductUnit_Product_SubProductId",
                        column: x => x.SubProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPharse_ProductId",
                table: "ProductPharse",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUnit_SubProductId",
                table: "ProductUnit",
                column: "SubProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "ProductPharse");

            migrationBuilder.DropTable(
                name: "ProductUnit");

            migrationBuilder.DropTable(
                name: "Pharse");

            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
