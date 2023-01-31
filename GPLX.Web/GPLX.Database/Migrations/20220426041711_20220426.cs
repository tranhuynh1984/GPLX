using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace GPLX.Database.Migrations
{
    public partial class _20220426 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HDKCB",
                columns: table => new
                {
                    IDHD = table.Column<int>(type: "int", nullable: false),
                    MaLoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MaHD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenHD = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Del = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    ND = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NS = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HDKCB", x => x.IDHD)
                        .Annotation("SqlServer:Clustered", true);
                });
            migrationBuilder.CreateIndex(
                name: "IX_HDKCB_IDHD",
                table: "HDKCB",
                column: "IDHD");

            migrationBuilder.CreateIndex(
                name: "IX_HDKCB_MaHD",
                table: "HDKCB",
                column: "MaHD");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "HDKCB");
            
        }
    }
}
