using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClipVault.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageIdToSnippet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Snippet");

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Snippet",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Language",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Python" },
                    { 2, "JavaScript" },
                    { 3, "C#" },
                    { 4, "Java" },
                    { 5, "Ruby" },
                    { 6, "Go" },
                    { 7, "PHP" },
                    { 8, "Swift" },
                    { 9, "Kotlin" },
                    { 10, "TypeScript" }
                });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Machine Learning");

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Automation");

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Web Scraping");

            migrationBuilder.CreateIndex(
                name: "IX_Snippet_LanguageId",
                table: "Snippet",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Snippet_Language_LanguageId",
                table: "Snippet",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Snippet_Language_LanguageId",
                table: "Snippet");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropIndex(
                name: "IX_Snippet_LanguageId",
                table: "Snippet");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Snippet");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Snippet",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "C#");

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "JavaScript");

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Python");
        }
    }
}
