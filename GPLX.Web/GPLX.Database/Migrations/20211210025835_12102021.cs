using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _12102021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CashFollowTypeName",
                table: "CashFollowAggregates",
                newName: "CashFollowGroupName");

            migrationBuilder.RenameColumn(
                name: "CashFollowTypeId",
                table: "CashFollowAggregates",
                newName: "CashFollowGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CashFollowGroupName",
                table: "CashFollowAggregates",
                newName: "CashFollowTypeName");

            migrationBuilder.RenameColumn(
                name: "CashFollowGroupId",
                table: "CashFollowAggregates",
                newName: "CashFollowTypeId");
        }
    }
}
