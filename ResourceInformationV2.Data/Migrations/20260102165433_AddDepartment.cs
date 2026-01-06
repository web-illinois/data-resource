using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceInformationV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilterDepartmentTitle",
                table: "Sources",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2026, 1, 2, 10, 54, 30, 744, DateTimeKind.Local).AddTicks(4039));

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "FilterDepartmentTitle", "LastUpdated" },
                values: new object[] { "", new DateTime(2026, 1, 2, 10, 54, 30, 744, DateTimeKind.Local).AddTicks(3898) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilterDepartmentTitle",
                table: "Sources");

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2025, 8, 30, 16, 30, 6, 41, DateTimeKind.Local).AddTicks(9589));

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2025, 8, 30, 16, 30, 6, 41, DateTimeKind.Local).AddTicks(9450));
        }
    }
}
