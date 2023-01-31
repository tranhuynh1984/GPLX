using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _2022061201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChuKy",
                table: "DeXuatGhiChu",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateByRole",
                table: "DeXuatGhiChu",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenCongTy",
                table: "DeXuat",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChuKy",
                table: "DeXuatGhiChu");

            migrationBuilder.DropColumn(
                name: "CreateByRole",
                table: "DeXuatGhiChu");

            migrationBuilder.DropColumn(
                name: "TenCongTy",
                table: "DeXuat");
        }
    }
}
