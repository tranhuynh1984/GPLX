using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _20220509 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stt",
                table: "Relationship",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stt",
                table: "LoaiDeXuat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stt",
                table: "DMBS_ChuyenKhoa",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stt",
                table: "Relationship");

            migrationBuilder.DropColumn(
                name: "Stt",
                table: "LoaiDeXuat");

            migrationBuilder.DropColumn(
                name: "Stt",
                table: "DMBS_ChuyenKhoa");
        }
    }
}
