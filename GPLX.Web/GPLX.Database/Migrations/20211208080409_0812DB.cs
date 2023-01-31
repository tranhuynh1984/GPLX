using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _0812DB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfitPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RevenuePlanType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PathExcel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PathPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSub = table.Column<bool>(type: "bit", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    TotalRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalExpense = table.Column<double>(type: "float", nullable: false),
                    TotalProfitTax = table.Column<double>(type: "float", nullable: false),
                    ProportionExpense = table.Column<double>(type: "float", nullable: false),
                    ProportionProfitTax = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitPlan", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ProfitPlanAggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfitPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfitPlanGroupId = table.Column<int>(type: "int", nullable: false),
                    ProfitPlanGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalExpense = table.Column<double>(type: "float", nullable: false),
                    TotalProfitTax = table.Column<double>(type: "float", nullable: false),
                    ProportionExpense = table.Column<double>(type: "float", nullable: false),
                    ProportionProfitTax = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitPlanAggregates", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ProfitPlanDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfitPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfitPlanGroupId = table.Column<int>(type: "int", nullable: false),
                    ProfitPlanGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    M1 = table.Column<double>(type: "float", nullable: false),
                    M2 = table.Column<double>(type: "float", nullable: false),
                    M3 = table.Column<double>(type: "float", nullable: false),
                    M4 = table.Column<double>(type: "float", nullable: false),
                    M5 = table.Column<double>(type: "float", nullable: false),
                    M6 = table.Column<double>(type: "float", nullable: false),
                    M7 = table.Column<double>(type: "float", nullable: false),
                    M8 = table.Column<double>(type: "float", nullable: false),
                    M9 = table.Column<double>(type: "float", nullable: false),
                    M10 = table.Column<double>(type: "float", nullable: false),
                    M11 = table.Column<double>(type: "float", nullable: false),
                    M12 = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    ProPortion = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitPlanDetails", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ProfitPlanGroups",
                columns: table => new
                {
                    ProfitPlanGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfitPlanGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfitPlanParentGroupId = table.Column<int>(type: "int", nullable: false),
                    ForSubject = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitPlanGroups", x => x.ProfitPlanGroupId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ProfitPlanLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfitPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_ProfitPlanLogs", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfitPlan_Id",
                table: "ProfitPlan",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfitPlan_UnitId_Status_IsSub",
                table: "ProfitPlan",
                columns: new[] { "UnitId", "Status", "IsSub" });

            migrationBuilder.CreateIndex(
                name: "IX_ProfitPlanAggregates_Id",
                table: "ProfitPlanAggregates",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfitPlanAggregates_ProfitPlanId",
                table: "ProfitPlanAggregates",
                column: "ProfitPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitPlanDetails_ProfitPlanId",
                table: "ProfitPlanDetails",
                column: "ProfitPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitPlanGroups_ForSubject",
                table: "ProfitPlanGroups",
                column: "ForSubject");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitPlanGroups_ProfitPlanGroupId",
                table: "ProfitPlanGroups",
                column: "ProfitPlanGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfitPlanLogs_ProfitPlanId",
                table: "ProfitPlanLogs",
                column: "ProfitPlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfitPlan");

            migrationBuilder.DropTable(
                name: "ProfitPlanAggregates");

            migrationBuilder.DropTable(
                name: "ProfitPlanDetails");

            migrationBuilder.DropTable(
                name: "ProfitPlanGroups");

            migrationBuilder.DropTable(
                name: "ProfitPlanLogs");
        }
    }
}
