using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _14122021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserUnitsManages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OfficeCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OfficeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUnitsManages", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserUnitsManages_Id",
                table: "UserUnitsManages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserUnitsManages_OfficeCode_UserId",
                table: "UserUnitsManages",
                columns: new[] { "OfficeCode", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserUnitsManages_UserId",
                table: "UserUnitsManages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserUnitsManages");
        }
    }
}
