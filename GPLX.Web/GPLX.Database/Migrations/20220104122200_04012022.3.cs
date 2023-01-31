using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _040120223 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupFor",
                table: "RevenuePlanContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequire",
                table: "RevenuePlanContents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Style",
                table: "RevenuePlanContents",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupFor",
                table: "RevenuePlanContents");

            migrationBuilder.DropColumn(
                name: "IsRequire",
                table: "RevenuePlanContents");

            migrationBuilder.DropColumn(
                name: "Style",
                table: "RevenuePlanContents");
        }
    }
}
