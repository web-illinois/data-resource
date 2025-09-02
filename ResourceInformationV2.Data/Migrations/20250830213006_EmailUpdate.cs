using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceInformationV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmailUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SourceEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailType = table.Column<int>(type: "int", nullable: false),
                    ReplyTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SendToReviewEmail = table.Column<bool>(type: "bit", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceEmails_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_SourceEmails_SourceId",
                table: "SourceEmails",
                column: "SourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SourceEmails");

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
                column: "LastUpdated",
                value: new DateTime(2025, 8, 29, 15, 38, 50, 863, DateTimeKind.Local).AddTicks(2254));
        }
    }
}
