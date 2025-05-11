using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBank.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKisiModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Aktif",
                table: "Kisiler",
                newName: "InternetBank");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InternetBank",
                table: "Kisiler",
                newName: "Aktif");
        }
    }
}
