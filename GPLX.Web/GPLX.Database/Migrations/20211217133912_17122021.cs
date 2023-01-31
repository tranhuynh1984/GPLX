using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _17122021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserConcurrently",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Selected = table.Column<bool>(type: "bit", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConcurrently", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserConcurrently_Id",
                table: "UserConcurrently",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserConcurrently_UnitId_UnitCode",
                table: "UserConcurrently",
                columns: new[] { "UnitId", "UnitCode" });

            migrationBuilder.CreateIndex(
                name: "IX_UserConcurrently_UserId",
                table: "UserConcurrently",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConcurrently_UserId_GroupCode",
                table: "UserConcurrently",
                columns: new[] { "UserId", "GroupCode" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConcurrently");
        }
    }
}
