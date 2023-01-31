using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _26112021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComparingType",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComparingType",
                table: "Payments");
        }
    }
}
