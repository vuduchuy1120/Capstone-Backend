using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Testaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImage_Product_ProductId",
                table: "ProductImage");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPharse_Pharse_PharseId",
                table: "ProductPharse");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPharse_Product_ProductId",
                table: "ProductPharse");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductUnit_Product_ProductId",
                table: "ProductUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductUnit_Product_SubProductId",
                table: "ProductUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductUnit",
                table: "ProductUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductPharse",
                table: "ProductPharse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductImage",
                table: "ProductImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pharse",
                table: "Pharse");

            migrationBuilder.RenameTable(
                name: "ProductUnit",
                newName: "ProductUnits");

            migrationBuilder.RenameTable(
                name: "ProductPharse",
                newName: "ProductPhases");

            migrationBuilder.RenameTable(
                name: "ProductImage",
                newName: "ProductImages");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "Pharse",
                newName: "Pharses");

            migrationBuilder.RenameIndex(
                name: "IX_ProductUnit_SubProductId",
                table: "ProductUnits",
                newName: "IX_ProductUnits_SubProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductPharse_ProductId",
                table: "ProductPhases",
                newName: "IX_ProductPhases_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImages",
                newName: "IX_ProductImages_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductUnits",
                table: "ProductUnits",
                columns: new[] { "ProductId", "SubProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductPhases",
                table: "ProductPhases",
                columns: new[] { "PharseId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pharses",
                table: "Pharses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhases_Pharses_PharseId",
                table: "ProductPhases",
                column: "PharseId",
                principalTable: "Pharses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhases_Products_ProductId",
                table: "ProductPhases",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUnits_Products_ProductId",
                table: "ProductUnits",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUnits_Products_SubProductId",
                table: "ProductUnits",
                column: "SubProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhases_Pharses_PharseId",
                table: "ProductPhases");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhases_Products_ProductId",
                table: "ProductPhases");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductUnits_Products_ProductId",
                table: "ProductUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductUnits_Products_SubProductId",
                table: "ProductUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductUnits",
                table: "ProductUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductPhases",
                table: "ProductPhases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductImages",
                table: "ProductImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pharses",
                table: "Pharses");

            migrationBuilder.RenameTable(
                name: "ProductUnits",
                newName: "ProductUnit");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "ProductPhases",
                newName: "ProductPharse");

            migrationBuilder.RenameTable(
                name: "ProductImages",
                newName: "ProductImage");

            migrationBuilder.RenameTable(
                name: "Pharses",
                newName: "Pharse");

            migrationBuilder.RenameIndex(
                name: "IX_ProductUnits_SubProductId",
                table: "ProductUnit",
                newName: "IX_ProductUnit_SubProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductPhases_ProductId",
                table: "ProductPharse",
                newName: "IX_ProductPharse_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImage",
                newName: "IX_ProductImage_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductUnit",
                table: "ProductUnit",
                columns: new[] { "ProductId", "SubProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductPharse",
                table: "ProductPharse",
                columns: new[] { "PharseId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductImage",
                table: "ProductImage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pharse",
                table: "Pharse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImage_Product_ProductId",
                table: "ProductImage",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPharse_Pharse_PharseId",
                table: "ProductPharse",
                column: "PharseId",
                principalTable: "Pharse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPharse_Product_ProductId",
                table: "ProductPharse",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUnit_Product_ProductId",
                table: "ProductUnit",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUnit_Product_SubProductId",
                table: "ProductUnit",
                column: "SubProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
