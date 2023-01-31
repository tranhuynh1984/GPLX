using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class AddApprovedFlow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovedFlow",
                columns: table => new
                {
                    FlowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlowName = table.Column<int>(type: "int", maxLength: 300, nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProcessId = table.Column<int>(type: "int", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovedFlow", x => x.FlowId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ApprovedFlowDetail",
                columns: table => new
                {
                    FlowDetailId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlowId = table.Column<int>(type: "int", nullable: false),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    ListUserAccept = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedUser = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsLastStep = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovedFlowDetail", x => x.FlowDetailId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Process",
                columns: table => new
                {
                    ProcessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessName = table.Column<int>(type: "int", maxLength: 250, nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Process", x => x.ProcessId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ProcessStep",
                columns: table => new
                {
                    StepId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProcessId = table.Column<int>(type: "int", nullable: false),
                    OrderStep = table.Column<int>(type: "int", nullable: false),
                    IsLastStep = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStep", x => x.StepId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedFlow_DoctorId",
                table: "ApprovedFlow",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedFlowDetail_FlowId",
                table: "ApprovedFlowDetail",
                column: "FlowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovedFlow");

            migrationBuilder.DropTable(
                name: "ApprovedFlowDetail");

            migrationBuilder.DropTable(
                name: "Process");

            migrationBuilder.DropTable(
                name: "ProcessStep");
        }
    }
}
