using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClipVault.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToCollectionAndSnippet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Snippet",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Collections",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Snippet");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Collections");
        }
    }
}
