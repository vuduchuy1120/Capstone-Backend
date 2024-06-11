using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateSetAndSetProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetProducts_Set_SetId",
                table: "SetProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Set",
                table: "Set");

            migrationBuilder.RenameTable(
                name: "Set",
                newName: "Sets");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sets",
                table: "Sets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SetProducts_Sets_SetId",
                table: "SetProducts",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetProducts_Sets_SetId",
                table: "SetProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sets",
                table: "Sets");

            migrationBuilder.RenameTable(
                name: "Sets",
                newName: "Set");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Set",
                table: "Set",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SetProducts_Set_SetId",
                table: "SetProducts",
                column: "SetId",
                principalTable: "Set",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
