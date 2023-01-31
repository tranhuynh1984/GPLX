using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _2022061101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Createby",
                table: "GiaKCB");

            migrationBuilder.DropColumn(
                name: "Createdate",
                table: "GiaKCB");

            migrationBuilder.DropColumn(
                name: "Updateby",
                table: "GiaKCB");

            migrationBuilder.DropColumn(
                name: "Updatedate",
                table: "GiaKCB");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayAuto",
                table: "GiaKCB",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "TenBacsi",
                table: "DeXuat",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenBacsi",
                table: "DeXuat");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayAuto",
                table: "GiaKCB",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Createby",
                table: "GiaKCB",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Createdate",
                table: "GiaKCB",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Updateby",
                table: "GiaKCB",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updatedate",
                table: "GiaKCB",
                type: "datetime2",
                nullable: true);
        }
    }
}
