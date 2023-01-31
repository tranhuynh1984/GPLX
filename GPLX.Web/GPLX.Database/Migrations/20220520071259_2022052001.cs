using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _2022052001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeXuatKhoaMaCTV",
                columns: table => new
                {
                    DeXuatCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaCTV = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ThoiGianKhoa = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDoKhoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeXuatKhoaMaCTV", x => new { x.DeXuatCode, x.MaCTV })
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DeXuatLuanChuyenMa",
                columns: table => new
                {
                    DeXuatCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaCTV = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ThoiGianKhoa = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeXuatLuanChuyenMa", x => new { x.DeXuatCode, x.MaCTV })
                        .Annotation("SqlServer:Clustered", true);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeXuatKhoaMaCTV");

            migrationBuilder.DropTable(
                name: "DeXuatLuanChuyenMa");
        }
    }
}
