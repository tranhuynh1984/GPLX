using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _091221 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashFollowType");

            migrationBuilder.DropColumn(
                name: "CashFollowTypeId",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "CashFollowTypeName",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "Cell",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "Col",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "CostEstimateItemTypeId",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "CostEstimateItemTypeName",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "IsBold",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "MonthOfYear",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "SubCashFollowTypeId",
                table: "CashFollowItem");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "CashFollowItem",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "SubCashFollowTypeName",
                table: "CashFollowItem",
                newName: "Migrate");

            migrationBuilder.RenameColumn(
                name: "Row",
                table: "CashFollowItem",
                newName: "CashFollowGroupId");

            migrationBuilder.RenameColumn(
                name: "PaymentName",
                table: "CashFollowItem",
                newName: "CashFollowGroupName");

            migrationBuilder.RenameColumn(
                name: "PathFilePdf",
                table: "CashFollow",
                newName: "PathPdf");

            migrationBuilder.RenameColumn(
                name: "PathFile",
                table: "CashFollow",
                newName: "PathExcel");

            migrationBuilder.AddColumn<double>(
                name: "M1",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M10",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M11",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M12",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M2",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M3",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M4",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M5",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M6",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M7",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M8",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "M9",
                table: "CashFollowItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "CashFollowType",
                table: "CashFollow",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Migrate",
                table: "CashFollow",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalCirculation",
                table: "CashFollow",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalRevenue",
                table: "CashFollow",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalSpending",
                table: "CashFollow",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "CashFollowAggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFollowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFollowTypeId = table.Column<int>(type: "int", nullable: false),
                    CashFollowTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Q1 = table.Column<double>(type: "float", nullable: false),
                    Q2 = table.Column<double>(type: "float", nullable: false),
                    Q3 = table.Column<double>(type: "float", nullable: false),
                    Q4 = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFollowAggregates", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CashFollowGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    RefPaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ForSubject = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFollowGroups", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashFollowAggregates_CashFollowId",
                table: "CashFollowAggregates",
                column: "CashFollowId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFollowAggregates_Id",
                table: "CashFollowAggregates",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CashFollowGroups_ForSubject",
                table: "CashFollowGroups",
                column: "ForSubject");

            migrationBuilder.CreateIndex(
                name: "IX_CashFollowGroups_Id",
                table: "CashFollowGroups",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashFollowAggregates");

            migrationBuilder.DropTable(
                name: "CashFollowGroups");

            migrationBuilder.DropColumn(
                name: "M1",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M10",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M11",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M12",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M2",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M3",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M4",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M5",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M6",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M7",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M8",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "M9",
                table: "CashFollowItem");

            migrationBuilder.DropColumn(
                name: "CashFollowType",
                table: "CashFollow");

            migrationBuilder.DropColumn(
                name: "Migrate",
                table: "CashFollow");

            migrationBuilder.DropColumn(
                name: "TotalCirculation",
                table: "CashFollow");

            migrationBuilder.DropColumn(
                name: "TotalRevenue",
                table: "CashFollow");

            migrationBuilder.DropColumn(
                name: "TotalSpending",
                table: "CashFollow");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "CashFollowItem",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Migrate",
                table: "CashFollowItem",
                newName: "SubCashFollowTypeName");

            migrationBuilder.RenameColumn(
                name: "CashFollowGroupName",
                table: "CashFollowItem",
                newName: "PaymentName");

            migrationBuilder.RenameColumn(
                name: "CashFollowGroupId",
                table: "CashFollowItem",
                newName: "Row");

            migrationBuilder.RenameColumn(
                name: "PathPdf",
                table: "CashFollow",
                newName: "PathFilePdf");

            migrationBuilder.RenameColumn(
                name: "PathExcel",
                table: "CashFollow",
                newName: "PathFile");

            migrationBuilder.AddColumn<Guid>(
                name: "CashFollowTypeId",
                table: "CashFollowItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CashFollowTypeName",
                table: "CashFollowItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cell",
                table: "CashFollowItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Col",
                table: "CashFollowItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CostEstimateItemTypeId",
                table: "CashFollowItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CostEstimateItemTypeName",
                table: "CashFollowItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsBold",
                table: "CashFollowItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthOfYear",
                table: "CashFollowItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "CashFollowItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SubCashFollowTypeId",
                table: "CashFollowItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CashFollowType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRoot = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parent = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    RefPaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFollowType", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashFollowType_Id_IsRoot",
                table: "CashFollowType",
                columns: new[] { "Id", "IsRoot" });
        }
    }
}
