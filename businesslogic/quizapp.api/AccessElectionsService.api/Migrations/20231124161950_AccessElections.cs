using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AccessElectionsService.api.Migrations
{
    /// <inheritdoc />
    public partial class AccessElections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AE");

            migrationBuilder.CreateTable(
                name: "QuestionSeverity",
                schema: "AE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionSeverityText = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSeverity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionType",
                schema: "AE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionTypeText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Section",
                schema: "AE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                schema: "AE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                schema: "AE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    QuestionNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    QuestionSeverityId = table.Column<int>(type: "int", nullable: false),
                    QuestionTypeId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_QuestionSeverity_QuestionSeverityId",
                        column: x => x.QuestionSeverityId,
                        principalSchema: "AE",
                        principalTable: "QuestionSeverity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Question_QuestionType_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalSchema: "AE",
                        principalTable: "QuestionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Question_Section_SectionId",
                        column: x => x.SectionId,
                        principalSchema: "AE",
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Question_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalSchema: "AE",
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswer",
                schema: "AE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionAnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "AE",
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswerFinding",
                schema: "AE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionAnswerFindingText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionAnswerId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswerFinding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionAnswerFinding_QuestionAnswer_QuestionAnswerId",
                        column: x => x.QuestionAnswerId,
                        principalSchema: "AE",
                        principalTable: "QuestionAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswerRecommendation",
                schema: "AE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionAnswerRecommendationText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionAnswerId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswerRecommendation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionAnswerRecommendation_QuestionAnswer_QuestionAnswerId",
                        column: x => x.QuestionAnswerId,
                        principalSchema: "AE",
                        principalTable: "QuestionAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "AE",
                table: "QuestionSeverity",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "ModifiedBy", "ModifiedOn", "QuestionSeverityText" },
                values: new object[,]
                {
                    { 1, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 }
                });

            migrationBuilder.InsertData(
                schema: "AE",
                table: "QuestionType",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "ModifiedBy", "ModifiedOn", "QuestionTypeText" },
                values: new object[,]
                {
                    { 1, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "blank" },
                    { 2, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "singleSelection" },
                    { 3, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "multipleChoiceSelection" }
                });

            migrationBuilder.InsertData(
                schema: "AE",
                table: "Section",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "A" },
                    { 2, "B" },
                    { 3, "C" },
                    { 4, "D" },
                    { 5, "E" },
                    { 6, "F" },
                    { 7, "G" },
                    { 8, "H" },
                    { 9, "I" },
                    { 10, "J" },
                    { 11, "K" },
                    { 12, "L" },
                    { 13, "M" },
                    { 14, "N" },
                    { 15, "O" },
                    { 16, "P" },
                    { 17, "Q" },
                    { 18, "R" },
                    { 19, "S" },
                    { 20, "T" },
                    { 21, "U" },
                    { 22, "V" },
                    { 23, "W" },
                    { 24, "X" },
                    { 25, "Y" },
                    { 26, "Z" }
                });

            migrationBuilder.InsertData(
                schema: "AE",
                table: "Zone",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "PARKING" },
                    { 2, "PATHWAYS" },
                    { 3, "ACCESSIBLE_ENTERANCE" },
                    { 4, "INTERIOR_ROUTES" },
                    { 5, "VOTING_AREAS" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionNumber",
                schema: "AE",
                table: "Question",
                column: "QuestionNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionSeverityId",
                schema: "AE",
                table: "Question",
                column: "QuestionSeverityId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionTypeId",
                schema: "AE",
                table: "Question",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_SectionId",
                schema: "AE",
                table: "Question",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_ZoneId",
                schema: "AE",
                table: "Question",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_QuestionId",
                schema: "AE",
                table: "QuestionAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswerFinding_QuestionAnswerId",
                schema: "AE",
                table: "QuestionAnswerFinding",
                column: "QuestionAnswerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswerRecommendation_QuestionAnswerId",
                schema: "AE",
                table: "QuestionAnswerRecommendation",
                column: "QuestionAnswerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionAnswerFinding",
                schema: "AE");

            migrationBuilder.DropTable(
                name: "QuestionAnswerRecommendation",
                schema: "AE");

            migrationBuilder.DropTable(
                name: "QuestionAnswer",
                schema: "AE");

            migrationBuilder.DropTable(
                name: "Question",
                schema: "AE");

            migrationBuilder.DropTable(
                name: "QuestionSeverity",
                schema: "AE");

            migrationBuilder.DropTable(
                name: "QuestionType",
                schema: "AE");

            migrationBuilder.DropTable(
                name: "Section",
                schema: "AE");

            migrationBuilder.DropTable(
                name: "Zone",
                schema: "AE");
        }
    }
}
