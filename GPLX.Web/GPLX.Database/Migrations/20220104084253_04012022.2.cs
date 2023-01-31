using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _040120222 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupFor",
                table: "InvestmentPlanContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RulesCellOnRow",
                table: "InvestmentPlanContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkipCellAts",
                table: "InvestmentPlanContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Style",
                table: "InvestmentPlanContents",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupFor",
                table: "InvestmentPlanContents");

            migrationBuilder.DropColumn(
                name: "RulesCellOnRow",
                table: "InvestmentPlanContents");

            migrationBuilder.DropColumn(
                name: "SkipCellAts",
                table: "InvestmentPlanContents");

            migrationBuilder.DropColumn(
                name: "Style",
                table: "InvestmentPlanContents");
        }
    }
}
