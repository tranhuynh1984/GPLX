using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _2022051403 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeXuat",
                columns: table => new
                {
                    DeXuatCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DeXuatName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaBacsi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    LoaiDeXuatCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProcessId = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeXuat", x => x.DeXuatCode)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DeXuatChiTiet",
                columns: table => new
                {
                    DeXuatCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValueOld = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValueNew = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeXuatChiTiet", x => new { x.DeXuatCode, x.FieldName })
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeXuat_DeXuatCode",
                table: "DeXuat",
                column: "DeXuatCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeXuat");

            migrationBuilder.DropTable(
                name: "DeXuatChiTiet");
        }
    }
}
