using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YenilikciEgitimPlatformu.Migrations
{
    /// <inheritdoc />
    public partial class CagriBilgisiModelsEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BasvuruLinki",
                table: "CagriBilgileri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DuzenlenenKurum",
                table: "CagriBilgileri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DuzenlenenKurumLogoUrl",
                table: "CagriBilgileri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GoruntulenmeSayisi",
                table: "CagriBilgileri",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasvuruLinki",
                table: "CagriBilgileri");

            migrationBuilder.DropColumn(
                name: "DuzenlenenKurum",
                table: "CagriBilgileri");

            migrationBuilder.DropColumn(
                name: "DuzenlenenKurumLogoUrl",
                table: "CagriBilgileri");

            migrationBuilder.DropColumn(
                name: "GoruntulenmeSayisi",
                table: "CagriBilgileri");
        }
    }
}
