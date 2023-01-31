using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _24012022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RotationProposalTue",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RotationProposalWe",
                table: "CostEstimate",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RotationProposalTue",
                table: "CostEstimate");

            migrationBuilder.DropColumn(
                name: "RotationProposalWe",
                table: "CostEstimate");
        }
    }
}
