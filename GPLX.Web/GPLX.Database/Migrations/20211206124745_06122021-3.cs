using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _061220213 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathExcel",
                table: "InvestmentPlan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PathPdf",
                table: "InvestmentPlan",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathExcel",
                table: "InvestmentPlan");

            migrationBuilder.DropColumn(
                name: "PathPdf",
                table: "InvestmentPlan");
        }
    }
}
