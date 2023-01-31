using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _061220211 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvestmentPlanAggregate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestmentPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestmentPlanContentId = table.Column<int>(type: "int", nullable: false),
                    InvestmentPlanContentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectCostInvestment = table.Column<double>(type: "float", nullable: false),
                    CapitalPlanAggregate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpenditureCapital = table.Column<double>(type: "float", nullable: false),
                    CapitalMedGroup = table.Column<double>(type: "float", nullable: false),
                    SpendingLoan = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentPlanAggregate", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlanAggregate_Id",
                table: "InvestmentPlanAggregate",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlanAggregate_Id_InvestmentPlanId",
                table: "InvestmentPlanAggregate",
                columns: new[] { "Id", "InvestmentPlanId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvestmentPlanAggregate");
        }
    }
}
