using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OverTime",
                table: "Attendances",
                newName: "HourOverTime");

            migrationBuilder.AddColumn<bool>(
                name: "IsAttendance",
                table: "Attendances",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverTime",
                table: "Attendances",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAttendance",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "IsOverTime",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "HourOverTime",
                table: "Attendances",
                newName: "OverTime");
        }
    }
}
