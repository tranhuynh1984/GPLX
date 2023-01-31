using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _2022061102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LyDoKhoa",
                table: "DeXuatKhoaMaCTV",
                newName: "TenCTV");

            migrationBuilder.AddColumn<string>(
                name: "TenCTV",
                table: "DeXuatLuanChuyenMa",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenCTV",
                table: "DeXuatLuanChuyenMa");

            migrationBuilder.RenameColumn(
                name: "TenCTV",
                table: "DeXuatKhoaMaCTV",
                newName: "LyDoKhoa");
        }
    }
}
