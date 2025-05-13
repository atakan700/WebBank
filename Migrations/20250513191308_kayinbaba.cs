using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBank.Migrations
{
    /// <inheritdoc />
    public partial class kayinbaba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "subeBilgisi",
                table: "Kisiler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "subeBilgisi",
                table: "Kisiler");
        }
    }
}
