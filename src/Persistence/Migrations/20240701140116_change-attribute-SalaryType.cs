using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeattributeSalaryType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalaryHistory_Users_UserId",
                table: "SalaryHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPay_Users_UserId",
                table: "SalaryPay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaryPay",
                table: "SalaryPay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaryHistory",
                table: "SalaryHistory");

            migrationBuilder.RenameTable(
                name: "SalaryPay",
                newName: "SalaryPays");

            migrationBuilder.RenameTable(
                name: "SalaryHistory",
                newName: "SalaryHistories");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPay_UserId",
                table: "SalaryPays",
                newName: "IX_SalaryPays_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryHistory_UserId",
                table: "SalaryHistories",
                newName: "IX_SalaryHistories_UserId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "SalaryHistories",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaryPays",
                table: "SalaryPays",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaryHistories",
                table: "SalaryHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryHistories_Users_UserId",
                table: "SalaryHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPays_Users_UserId",
                table: "SalaryPays",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalaryHistories_Users_UserId",
                table: "SalaryHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPays_Users_UserId",
                table: "SalaryPays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaryPays",
                table: "SalaryPays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaryHistories",
                table: "SalaryHistories");

            migrationBuilder.RenameTable(
                name: "SalaryPays",
                newName: "SalaryPay");

            migrationBuilder.RenameTable(
                name: "SalaryHistories",
                newName: "SalaryHistory");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryPays_UserId",
                table: "SalaryPay",
                newName: "IX_SalaryPay_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SalaryHistories_UserId",
                table: "SalaryHistory",
                newName: "IX_SalaryHistory_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "SalaryHistory",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaryPay",
                table: "SalaryPay",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaryHistory",
                table: "SalaryHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryHistory_Users_UserId",
                table: "SalaryHistory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPay_Users_UserId",
                table: "SalaryPay",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
