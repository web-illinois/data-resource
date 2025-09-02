using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceInformationV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmailAndApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiSecretCurrent",
                table: "Sources",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApiSecretLastChanged",
                table: "Sources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiSecretPrevious",
                table: "Sources",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ForceApiToDraft",
                table: "Sources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2025, 8, 29, 15, 38, 50, 863, DateTimeKind.Local).AddTicks(2570));

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ApiSecretCurrent", "ApiSecretLastChanged", "ApiSecretPrevious", "ForceApiToDraft", "LastUpdated" },
                values: new object[] { "", null, "", true, new DateTime(2025, 8, 29, 15, 38, 50, 863, DateTimeKind.Local).AddTicks(2254) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiSecretCurrent",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "ApiSecretLastChanged",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "ApiSecretPrevious",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "ForceApiToDraft",
                table: "Sources");

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2025, 8, 12, 12, 36, 7, 742, DateTimeKind.Local).AddTicks(5826));

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2025, 8, 12, 12, 36, 7, 742, DateTimeKind.Local).AddTicks(5447));
        }
    }
}
