using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _2022051702 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LyDoKhoa",
                table: "DeXuat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianKhoa",
                table: "DeXuat",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeXuatGhiChu",
                columns: table => new
                {
                    DeXuatCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProcessStepId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeXuatGhiChu", x => new { x.DeXuatCode, x.ProcessStepId })
                        .Annotation("SqlServer:Clustered", true);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeXuatGhiChu");

            migrationBuilder.DropColumn(
                name: "LyDoKhoa",
                table: "DeXuat");

            migrationBuilder.DropColumn(
                name: "ThoiGianKhoa",
                table: "DeXuat");
        }
    }
}
