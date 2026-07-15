using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceInformationV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDynamicInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SecurityEntries",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: -1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "ApiSecretCurrent", "ApiSecretLastChanged", "ApiSecretPrevious", "BaseUrl", "Code", "CreatedByEmail", "DateLastUrlCheck", "DeactivateOnReview", "DeletedByEmail", "FilterAudienceTitle", "FilterDepartmentTitle", "FilterOrder", "FilterTag1Title", "FilterTag2Title", "FilterTag3Title", "FilterTag4Title", "FilterTopicTitle", "ForceApiToDraft", "IsActive", "IsTest", "LastUpdated", "NumberOfDaysForReview", "ReviewEmail", "Title", "UseEvents", "UseEventsFragment", "UseFaqs", "UseFaqsFragment", "UseNotes", "UseNotesFragment", "UsePeople", "UsePeopleFragment", "UsePublications", "UsePublicationsFragment", "UseResources", "UseResourcesFragment" },
                values: new object[] { -1, "", null, "", "", "test", "jonker@illinois.edu", null, false, "", "", "", "", "", "", "", "", "", true, false, true, new DateTime(2026, 3, 31, 12, 24, 31, 93, DateTimeKind.Local).AddTicks(6514), 0, "", "Test Entry", false, false, false, false, false, false, false, false, false, false, false, false });

            migrationBuilder.InsertData(
                table: "SecurityEntries",
                columns: new[] { "Id", "DepartmentTag", "Email", "IsActive", "IsFullAdmin", "IsOwner", "IsPublic", "IsRequested", "LastUpdated", "SourceId" },
                values: new object[] { -1, "", "jonker@illinois.edu", true, false, true, false, false, new DateTime(2026, 3, 31, 12, 24, 31, 93, DateTimeKind.Local).AddTicks(6632), -1 });
        }
    }
}
