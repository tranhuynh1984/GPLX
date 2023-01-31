using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _20220513 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsActive",
                table: "TBL_CTVGROUPSUB2_DETAIL",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IsActive",
                table: "TBL_CTVGROUPSUB1_DETAIL",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ProfileCK",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TBL_CTVGROUPSUB2_DETAIL");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TBL_CTVGROUPSUB1_DETAIL");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "ProfileCK");
        }
    }
}
