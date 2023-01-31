using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class addTableDMGG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMGG",
                columns: table => new
                {
                    MaGG = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenGG = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GiamGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMGG", x => x.MaGG)
                        .Annotation("SqlServer:Clustered", true);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMGG");
        }
    }
}
