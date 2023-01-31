using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace GPLX.Database.Migrations
{
    public partial class _20220427 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMPN",
                columns: table => new
                {
                    PhapNhanId = table.Column<int>(type: "int", nullable: false),
                    PhapNhanName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AddressCompany = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMPN", x => x.PhapNhanId)
                        .Annotation("SqlServer:Clustered", true);
                });
            migrationBuilder.CreateIndex(
                name: "IX_DMPN_PhapNhanId",
                table: "DMPN",
                column: "PhapNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_DMPN_PhapNhanName",
                table: "DMPN",
                column: "PhapNhanName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "DMPN");
        }
    }
}
