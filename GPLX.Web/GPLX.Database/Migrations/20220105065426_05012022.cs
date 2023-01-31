using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _05012022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupFor",
                table: "ProfitPlanGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequire",
                table: "ProfitPlanGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RulesCellOnRow",
                table: "ProfitPlanGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Style",
                table: "ProfitPlanGroups",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupFor",
                table: "ProfitPlanGroups");

            migrationBuilder.DropColumn(
                name: "IsRequire",
                table: "ProfitPlanGroups");

            migrationBuilder.DropColumn(
                name: "RulesCellOnRow",
                table: "ProfitPlanGroups");

            migrationBuilder.DropColumn(
                name: "Style",
                table: "ProfitPlanGroups");
        }
    }
}
