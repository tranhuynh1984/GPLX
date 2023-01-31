using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _16122021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathSignature",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathSignature",
                table: "Users");
        }
    }
}
