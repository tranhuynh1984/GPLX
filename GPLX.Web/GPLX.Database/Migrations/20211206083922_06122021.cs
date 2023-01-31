using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _06122021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvestmentPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSub = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentPlan", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentPlanContents",
                columns: table => new
                {
                    InvestmentPlanContentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvestmentPlanContentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ForSubject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentPlanContents", x => x.InvestmentPlanContentId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentPlanDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestmentPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvestmentPlanContentId = table.Column<int>(type: "int", nullable: false),
                    InvestmentPlanContentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CostExpected = table.Column<double>(type: "float", nullable: false),
                    CapitalPlanId = table.Column<int>(type: "int", nullable: false),
                    CapitalPlanName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpendingLoanPercent = table.Column<double>(type: "float", nullable: false),
                    SpendingLoan = table.Column<double>(type: "float", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentPlanDetails", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlan_Id",
                table: "InvestmentPlan",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlan_Id_Status_UnitId",
                table: "InvestmentPlan",
                columns: new[] { "Id", "Status", "UnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlanContents_InvestmentPlanContentId",
                table: "InvestmentPlanContents",
                column: "InvestmentPlanContentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlanDetails_Id",
                table: "InvestmentPlanDetails",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlanDetails_Id_InvestmentPlanId",
                table: "InvestmentPlanDetails",
                columns: new[] { "Id", "InvestmentPlanId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvestmentPlan");

            migrationBuilder.DropTable(
                name: "InvestmentPlanContents");

            migrationBuilder.DropTable(
                name: "InvestmentPlanDetails");
        }
    }
}
