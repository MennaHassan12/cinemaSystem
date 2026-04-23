using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemaSystem.Migrations
{
    /// <inheritdoc />
    public partial class addNewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubImagesUrls",
                table: "Movies",
                newName: "SubImages");

            migrationBuilder.RenameColumn(
                name: "MainImageUrl",
                table: "Movies",
                newName: "MainImage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubImages",
                table: "Movies",
                newName: "SubImagesUrls");

            migrationBuilder.RenameColumn(
                name: "MainImage",
                table: "Movies",
                newName: "MainImageUrl");
        }
    }
}
