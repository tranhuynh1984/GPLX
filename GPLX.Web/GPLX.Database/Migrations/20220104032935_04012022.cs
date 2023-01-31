using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _04012022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Formula",
                table: "CashFollowGroups");

            migrationBuilder.DropColumn(
                name: "FormulaType",
                table: "CashFollowGroups");

            migrationBuilder.RenameColumn(
                name: "ReadOnlyCellInRow",
                table: "CashFollowGroups",
                newName: "IsRequire");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRequire",
                table: "CashFollowGroups",
                newName: "ReadOnlyCellInRow");

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
        }
    }
}
