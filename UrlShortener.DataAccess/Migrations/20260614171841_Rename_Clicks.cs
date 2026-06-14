using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Rename_Clicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Clics",
                table: "Urls",
                newName: "Clicks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Clicks",
                table: "Urls",
                newName: "Clics");
        }
    }
}
