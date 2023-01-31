using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _08122021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProportionExpense",
                table: "ProfitPlanAggregates");

            migrationBuilder.DropColumn(
                name: "ProportionProfitTax",
                table: "ProfitPlanAggregates");

            migrationBuilder.DropColumn(
                name: "TotalExpense",
                table: "ProfitPlanAggregates");

            migrationBuilder.RenameColumn(
                name: "TotalRevenue",
                table: "ProfitPlanAggregates",
                newName: "TotalCosh");

            migrationBuilder.RenameColumn(
                name: "TotalProfitTax",
                table: "ProfitPlanAggregates",
                newName: "Proportion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalCosh",
                table: "ProfitPlanAggregates",
                newName: "TotalRevenue");

            migrationBuilder.RenameColumn(
                name: "Proportion",
                table: "ProfitPlanAggregates",
                newName: "TotalProfitTax");

            migrationBuilder.AddColumn<double>(
                name: "ProportionExpense",
                table: "ProfitPlanAggregates",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ProportionProfitTax",
                table: "ProfitPlanAggregates",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalExpense",
                table: "ProfitPlanAggregates",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
