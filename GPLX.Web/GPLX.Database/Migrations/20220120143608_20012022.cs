using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _20012022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BankBIDV",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BankBIDVRevenue",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BankTCB",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Cash",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ContributionRevenue",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiffRevenue",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LoanAvailable",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OperatingRevenue",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankBIDV",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "BankBIDVRevenue",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "BankTCB",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "ContributionRevenue",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "DiffRevenue",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "LoanAvailable",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "OperatingRevenue",
                table: "CostEstimate");
        }
    }
}
