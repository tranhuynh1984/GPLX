using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _19012012 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CostEstimate_UserId_Status_CreatedDate_DepartmentId",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "NonRoutineCost",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "RoutineCost",
                table: "CostEstimate");

            migrationBuilder.RenameColumn(
                name: "PdfFile",
                table: "CostEstimate",
                newName: "PathPdf");

            migrationBuilder.RenameColumn(
                name: "ExcelFile",
                table: "CostEstimate",
                newName: "PathExcel");

            migrationBuilder.AlterColumn<double>(
                name: "WorkingBalanceCost",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TotalSpendingLoan",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TotalExpenditureCapital",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "RotationProposal",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "PlanCutCost",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "OperatingCost",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "InvestmentCost",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Funds",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "FinancialCost",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ExpectRevenue",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "EstimatedCost",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "EquityCost",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "EndAvailableBalance",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "BeginAvailableBalance",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "AccountBalance",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PathPdf",
                table: "CostEstimate",
                newName: "PdfFile");

            migrationBuilder.RenameColumn(
                name: "PathExcel",
                table: "CostEstimate",
                newName: "ExcelFile");

            migrationBuilder.AlterColumn<long>(
                name: "WorkingBalanceCost",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "TotalSpendingLoan",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "TotalExpenditureCapital",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "RotationProposal",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "PlanCutCost",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "OperatingCost",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "InvestmentCost",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "Funds",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "FinancialCost",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "ExpectRevenue",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "EstimatedCost",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "EquityCost",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "EndAvailableBalance",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "BeginAvailableBalance",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "AccountBalance",
                table: "CostEstimate",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "CostEstimate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "CostEstimate",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NonRoutineCost",
                table: "CostEstimate",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoutineCost",
                table: "CostEstimate",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostEstimate_UserId_Status_CreatedDate_DepartmentId",
                table: "CostEstimate",
                columns: new[] { "UserId", "Status", "CreatedDate", "DepartmentId" });
        }
    }
}
