using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceInformationV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeactivateOnReview = table.Column<bool>(type: "bit", nullable: false),
                    DeletedByEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilterAudienceTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilterOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilterTag1Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilterTag2Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilterTag3Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilterTag4Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilterTopicTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTest = table.Column<bool>(type: "bit", nullable: false),
                    NumberOfDaysForReview = table.Column<int>(type: "int", nullable: false),
                    ReviewEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UseEvents = table.Column<bool>(type: "bit", nullable: false),
                    UseFaqs = table.Column<bool>(type: "bit", nullable: false),
                    UseNotes = table.Column<bool>(type: "bit", nullable: false),
                    UsePeople = table.Column<bool>(type: "bit", nullable: false),
                    UsePublications = table.Column<bool>(type: "bit", nullable: false),
                    UseResources = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instructions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryType = table.Column<int>(type: "int", nullable: false),
                    FieldType = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instructions_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryType = table.Column<int>(type: "int", nullable: false),
                    ChangedByNetId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailSent = table.Column<bool>(type: "bit", nullable: false),
                    FieldType = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecurityEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFullAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsOwner = table.Column<bool>(type: "bit", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    IsRequested = table.Column<bool>(type: "bit", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecurityEntries_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    TagType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "BaseUrl", "Code", "CreatedByEmail", "DeactivateOnReview", "DeletedByEmail", "FilterAudienceTitle", "FilterOrder", "FilterTag1Title", "FilterTag2Title", "FilterTag3Title", "FilterTag4Title", "FilterTopicTitle", "IsActive", "IsTest", "LastUpdated", "NumberOfDaysForReview", "ReviewEmail", "Title", "UseEvents", "UseFaqs", "UseNotes", "UsePeople", "UsePublications", "UseResources" },
                values: new object[] { -1, "", "test", "jonker@illinois.edu", false, "", "", "", "", "", "", "", "", false, true, new DateTime(2025, 8, 12, 12, 36, 7, 742, DateTimeKind.Local).AddTicks(5447), 0, "", "Test Entry", false, false, false, false, false, false });

            migrationBuilder.InsertData(
                table: "SecurityEntries",
                columns: new[] { "Id", "DepartmentTag", "Email", "IsActive", "IsFullAdmin", "IsOwner", "IsPublic", "IsRequested", "LastUpdated", "SourceId" },
                values: new object[] { -1, "", "jonker@illinois.edu", true, false, true, false, false, new DateTime(2025, 8, 12, 12, 36, 7, 742, DateTimeKind.Local).AddTicks(5826), -1 });

            migrationBuilder.CreateIndex(
                name: "IX_Instructions_SourceId",
                table: "Instructions",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_SourceId",
                table: "Logs",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityEntries_SourceId",
                table: "SecurityEntries",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_SourceId",
                table: "Tags",
                column: "SourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instructions");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "SecurityEntries");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Sources");
        }
    }
}
