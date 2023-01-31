using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class AddLogCTV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CK",
                table: "HDCTV");

            migrationBuilder.DropColumn(
                name: "CKKH",
                table: "HDCTV");

            migrationBuilder.DropColumn(
                name: "HTCKDoiTuong",
                table: "HDCTV");

            migrationBuilder.DropColumn(
                name: "MaDVCT",
                table: "HDCTV");

            migrationBuilder.RenameColumn(
                name: "TraSau",
                table: "HDCTV",
                newName: "CtvStatus");

            migrationBuilder.AddColumn<DateTime>(
                name: "SendDate",
                table: "ApprovedFlow",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StatusFlow",
                table: "ApprovedFlow",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TBL_LOGCTVGROUP",
                columns: table => new
                {
                    LogCtvId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CTVGroupID = table.Column<int>(type: "int", nullable: false),
                    SubId = table.Column<int>(type: "int", nullable: false),
                    BP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HTCK = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHetHan = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ThuSau = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LogCtvStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_LOGCTVGROUP", x => x.LogCtvId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_LOGCTVGROUP_DoctorId",
                table: "TBL_LOGCTVGROUP",
                column: "DoctorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_LOGCTVGROUP");

            migrationBuilder.DropColumn(
                name: "SendDate",
                table: "ApprovedFlow");

            migrationBuilder.DropColumn(
                name: "StatusFlow",
                table: "ApprovedFlow");

            migrationBuilder.RenameColumn(
                name: "CtvStatus",
                table: "HDCTV",
                newName: "TraSau");

            migrationBuilder.AddColumn<int>(
                name: "CK",
                table: "HDCTV",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CKKH",
                table: "HDCTV",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HTCKDoiTuong",
                table: "HDCTV",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaDVCT",
                table: "HDCTV",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
