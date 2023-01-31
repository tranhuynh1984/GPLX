using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _15122021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId_GroupId",
                table: "UserGroups",
                columns: new[] { "UserId", "GroupId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.AddColumn<string>(
                name: "GroupCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
