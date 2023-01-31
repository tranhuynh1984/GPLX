using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class RemoveComputeCol_IsHetHan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHetHan",
                table: "TBL_LOGCTVGROUP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHetHan",
                table: "TBL_LOGCTVGROUP",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
