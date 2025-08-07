using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMinimalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserJobs_DateTimeCreated",
                table: "UserJobs",
                column: "DateTimeCreated");

            migrationBuilder.CreateIndex(
                name: "IX_UserJobs_UserId_DateTimeCreated",
                table: "UserJobs",
                columns: new[] { "UserId", "DateTimeCreated" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserJobs_DateTimeCreated",
                table: "UserJobs");

            migrationBuilder.DropIndex(
                name: "IX_UserJobs_UserId_DateTimeCreated",
                table: "UserJobs");
        }
    }
}
