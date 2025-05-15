using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBank.Migrations
{
    /// <inheritdoc />
    public partial class migi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HesapBilgileri_Sube_SubeId",
                table: "HesapBilgileri");

            migrationBuilder.AlterColumn<string>(
                name: "subeBilgisi",
                table: "Kisiler",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SubeId",
                table: "HesapBilgileri",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "IBAN",
                table: "HesapBilgileri",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_HesapBilgileri_Sube_SubeId",
                table: "HesapBilgileri",
                column: "SubeId",
                principalTable: "Sube",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HesapBilgileri_Sube_SubeId",
                table: "HesapBilgileri");

            migrationBuilder.AlterColumn<string>(
                name: "subeBilgisi",
                table: "Kisiler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubeId",
                table: "HesapBilgileri",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IBAN",
                table: "HesapBilgileri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HesapBilgileri_Sube_SubeId",
                table: "HesapBilgileri",
                column: "SubeId",
                principalTable: "Sube",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
