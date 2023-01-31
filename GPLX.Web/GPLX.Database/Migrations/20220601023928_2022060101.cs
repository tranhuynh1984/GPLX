using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _2022060101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreateByCode",
                table: "DeXuatGhiChu",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateByName",
                table: "DeXuatGhiChu",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "DeXuatGhiChu",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateByCode",
                table: "DeXuatGhiChu");

            migrationBuilder.DropColumn(
                name: "CreateByName",
                table: "DeXuatGhiChu");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "DeXuatGhiChu");
        }
    }
}
