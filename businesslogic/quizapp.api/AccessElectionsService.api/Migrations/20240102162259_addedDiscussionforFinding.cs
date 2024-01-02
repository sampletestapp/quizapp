using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessElectionsService.api.Migrations
{
    /// <inheritdoc />
    public partial class addedDiscussionforFinding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionAnswerFindingDiscussion",
                schema: "AE",
                table: "QuestionAnswerFinding",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionAnswerFindingDiscussion",
                schema: "AE",
                table: "QuestionAnswerFinding");
        }
    }
}
