using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _2022060201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiaKCB",
                columns: table => new
                {
                    IDGiaCT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDHD = table.Column<int>(type: "int", nullable: false),
                    MaCP = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    SL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Del = table.Column<int>(type: "int", nullable: false),
                    NgayAuto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaKCB", x => x.IDGiaCT)
                        .Annotation("SqlServer:Clustered", true);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiaKCB");
        }
    }
}
