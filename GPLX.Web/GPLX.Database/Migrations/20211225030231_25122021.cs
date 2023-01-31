using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _25122021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccDigitalSignature",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordDigitalSignature",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FilePdfCreateLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilePdfCreateLogs", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilePdfCreateLogs_Id",
                table: "FilePdfCreateLogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FilePdfCreateLogs_UnitId_Type",
                table: "FilePdfCreateLogs",
                columns: new[] { "UnitId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilePdfCreateLogs");

            migrationBuilder.DropColumn(
                name: "AccDigitalSignature",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordDigitalSignature",
                table: "Users");
        }
    }
}
