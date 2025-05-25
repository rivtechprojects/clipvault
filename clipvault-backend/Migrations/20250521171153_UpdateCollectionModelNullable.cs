using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ClipVault.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCollectionModelNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CollectionId",
                table: "Snippet",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ParentCollectionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collections_Collections_ParentCollectionId",
                        column: x => x.ParentCollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Snippet_CollectionId",
                table: "Snippet",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_ParentCollectionId",
                table: "Collections",
                column: "ParentCollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Snippet_Collections_CollectionId",
                table: "Snippet",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Snippet_Collections_CollectionId",
                table: "Snippet");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_Snippet_CollectionId",
                table: "Snippet");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Snippet");
        }
    }
}
