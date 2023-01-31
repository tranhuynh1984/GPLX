using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _28112021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSub",
                table: "CostEstimateItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSub",
                table: "CashFollow",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSub",
                table: "ActuallySpent",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSub",
                table: "CostEstimateItem");

            migrationBuilder.DropColumn(
                name: "IsSub",
                table: "CashFollow");

            migrationBuilder.DropColumn(
                name: "IsSub",
                table: "ActuallySpent");
        }
    }
}
