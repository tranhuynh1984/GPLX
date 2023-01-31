using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class AddColumnTableTBL_CTVGROUPSUB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenCP",
                table: "TBL_CTVGROUPSUB2_DETAIL",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenCP",
                table: "TBL_CTVGROUPSUB1_DETAIL",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenCP",
                table: "TBL_CTVGROUPSUB2_DETAIL");

            migrationBuilder.DropColumn(
                name: "TenCP",
                table: "TBL_CTVGROUPSUB1_DETAIL");
        }
    }
}
