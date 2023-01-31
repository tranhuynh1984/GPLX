using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _31122021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecialUnitFollowConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UnitShortName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialUnitFollowConfigs", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialUnitFollowConfigs_Id",
                table: "SpecialUnitFollowConfigs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialUnitFollowConfigs_UnitCode",
                table: "SpecialUnitFollowConfigs",
                column: "UnitCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecialUnitFollowConfigs");
        }
    }
}
