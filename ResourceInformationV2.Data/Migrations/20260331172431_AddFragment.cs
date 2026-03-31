using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceInformationV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFragment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseEventsFragment",
                table: "Sources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseFaqsFragment",
                table: "Sources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseNotesFragment",
                table: "Sources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsePeopleFragment",
                table: "Sources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsePublicationsFragment",
                table: "Sources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseResourcesFragment",
                table: "Sources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 31, 12, 24, 31, 93, DateTimeKind.Local).AddTicks(6632));

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "LastUpdated", "UseEventsFragment", "UseFaqsFragment", "UseNotesFragment", "UsePeopleFragment", "UsePublicationsFragment", "UseResourcesFragment" },
                values: new object[] { new DateTime(2026, 3, 31, 12, 24, 31, 93, DateTimeKind.Local).AddTicks(6514), false, false, false, false, false, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseEventsFragment",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "UseFaqsFragment",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "UseNotesFragment",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "UsePeopleFragment",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "UsePublicationsFragment",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "UseResourcesFragment",
                table: "Sources");

            migrationBuilder.UpdateData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2026, 2, 18, 8, 21, 35, 236, DateTimeKind.Local).AddTicks(6239));

            migrationBuilder.UpdateData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: -1,
                column: "LastUpdated",
                value: new DateTime(2026, 2, 18, 8, 21, 35, 236, DateTimeKind.Local).AddTicks(6139));
        }
    }
}
