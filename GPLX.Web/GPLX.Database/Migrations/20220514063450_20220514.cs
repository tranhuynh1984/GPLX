using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _20220514 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcessRoleId",
                table: "ProcessStep",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProcessMapRole",
                columns: table => new
                {
                    IDRole = table.Column<int>(type: "int", nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessMapRole", x => new { x.IDRole, x.GroupCode })
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ProcessRole",
                columns: table => new
                {
                    IDRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessRole", x => x.IDRole)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessMapRole_GroupCode",
                table: "ProcessMapRole",
                column: "GroupCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessMapRole_IDRole",
                table: "ProcessMapRole",
                column: "IDRole");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessMapRole");

            migrationBuilder.DropTable(
                name: "ProcessRole");

            migrationBuilder.DropColumn(
                name: "ProcessRoleId",
                table: "ProcessStep");
        }
    }
}
