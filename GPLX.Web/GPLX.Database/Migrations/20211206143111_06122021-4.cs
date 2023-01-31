using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _061220214 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "InvestmentPlan",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "TotalCapitalMedGroup",
                table: "InvestmentPlan",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalExpectCostInvestment",
                table: "InvestmentPlan",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalExpenditureCapital",
                table: "InvestmentPlan",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalSpendingLoan",
                table: "InvestmentPlan",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "InvestmentPlanLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestmentPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentPlanLogs", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlanLogs_Id",
                table: "InvestmentPlanLogs",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlanLogs_Id_InvestmentPlanId",
                table: "InvestmentPlanLogs",
                columns: new[] { "Id", "InvestmentPlanId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvestmentPlanLogs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "InvestmentPlan");

            migrationBuilder.DropColumn(
                name: "TotalCapitalMedGroup",
                table: "InvestmentPlan");

            migrationBuilder.DropColumn(
                name: "TotalExpectCostInvestment",
                table: "InvestmentPlan");

            migrationBuilder.DropColumn(
                name: "TotalExpenditureCapital",
                table: "InvestmentPlan");

            migrationBuilder.DropColumn(
                name: "TotalSpendingLoan",
                table: "InvestmentPlan");
        }
    }
}
