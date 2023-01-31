using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _03012022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Formula",
                table: "CashFollowGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormulaType",
                table: "CashFollowGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReadOnlyCellInRow",
                table: "CashFollowGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RulesCellOnRow",
                table: "CashFollowGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkipCellAts",
                table: "CashFollowGroups",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Formula",
                table: "CashFollowGroups");

            migrationBuilder.DropColumn(
                name: "FormulaType",
                table: "CashFollowGroups");

            migrationBuilder.DropColumn(
                name: "ReadOnlyCellInRow",
                table: "CashFollowGroups");

            migrationBuilder.DropColumn(
                name: "RulesCellOnRow",
                table: "CashFollowGroups");

            migrationBuilder.DropColumn(
                name: "SkipCellAts",
                table: "CashFollowGroups");
        }
    }
}
