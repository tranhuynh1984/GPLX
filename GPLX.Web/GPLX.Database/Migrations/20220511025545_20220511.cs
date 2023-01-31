using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _20220511 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfileCKCP",
                columns: table => new
                {
                    ProfileCKMa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CPMa = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileCKCP", x => new { x.ProfileCKMa, x.CPMa })
                        .Annotation("SqlServer:Clustered", true);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileCKCP");
        }
    }
}
