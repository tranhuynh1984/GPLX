using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _0712DB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RevenuePlan",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsSub",
                table: "RevenuePlan",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PathExcel",
                table: "RevenuePlan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PathPdf",
                table: "RevenuePlan",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RevenuePlan");

            migrationBuilder.DropColumn(
                name: "IsSub",
                table: "RevenuePlan");

            migrationBuilder.DropColumn(
                name: "PathExcel",
                table: "RevenuePlan");

            migrationBuilder.DropColumn(
                name: "PathPdf",
                table: "RevenuePlan");
        }
    }
}
