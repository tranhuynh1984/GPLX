using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class DB07122021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RevenuePlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevenuePlanType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenuePlan", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "RevenuePlanAggregate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevenuePlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevenuePlanContentId = table.Column<int>(type: "int", nullable: false),
                    RevenuePlanContentContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProportionCustomer = table.Column<double>(type: "float", nullable: false),
                    ExpectRevenue = table.Column<double>(type: "float", nullable: false),
                    ProportionRevenue = table.Column<double>(type: "float", nullable: false),
                    NumberCustomers = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenuePlanAggregate", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "RevenuePlanCashDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevenuePlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevenuePlanContentId = table.Column<int>(type: "int", nullable: false),
                    RevenuePlanContentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RevenuePlanCashDetails", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "RevenuePlanContents",
                columns: table => new
                {
                    RevenuePlanContentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RevenuePlanContentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenuePlanContents", x => x.RevenuePlanContentId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "RevenuePlanCustomerDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevenuePlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevenuePlanContentId = table.Column<int>(type: "int", nullable: false),
                    RevenuePlanContentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RevenuePlanCustomerDetails", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "RevenuePlanLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevenuePlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_RevenuePlanLogs", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlan_Id",
                table: "RevenuePlan",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlan_Id_UnitId_Status",
                table: "RevenuePlan",
                columns: new[] { "Id", "UnitId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanAggregate_Id",
                table: "RevenuePlanAggregate",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanAggregate_RevenuePlanContentId",
                table: "RevenuePlanAggregate",
                column: "RevenuePlanContentId");

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanCashDetails_RevenuePlanId",
                table: "RevenuePlanCashDetails",
                column: "RevenuePlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanContents_RevenuePlanContentId",
                table: "RevenuePlanContents",
                column: "RevenuePlanContentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanCustomerDetails_RevenuePlanId",
                table: "RevenuePlanCustomerDetails",
                column: "RevenuePlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanLogs_RevenuePlanId",
                table: "RevenuePlanLogs",
                column: "RevenuePlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanLogs_RevenuePlanId_FromStatus",
                table: "RevenuePlanLogs",
                columns: new[] { "RevenuePlanId", "FromStatus" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RevenuePlan");

            migrationBuilder.DropTable(
                name: "RevenuePlanAggregate");

            migrationBuilder.DropTable(
                name: "RevenuePlanCashDetails");

            migrationBuilder.DropTable(
                name: "RevenuePlanContents");

            migrationBuilder.DropTable(
                name: "RevenuePlanCustomerDetails");

            migrationBuilder.DropTable(
                name: "RevenuePlanLogs");
        }
    }
}
