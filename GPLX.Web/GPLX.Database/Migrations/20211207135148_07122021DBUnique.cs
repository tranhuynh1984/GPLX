using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _07122021DBUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RevenuePlanLogs_RevenuePlanId",
                table: "RevenuePlanLogs");

            migrationBuilder.DropIndex(
                name: "IX_RevenuePlanCustomerDetails_RevenuePlanId",
                table: "RevenuePlanCustomerDetails");

            migrationBuilder.DropIndex(
                name: "IX_RevenuePlanCashDetails_RevenuePlanId",
                table: "RevenuePlanCashDetails");

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanLogs_RevenuePlanId",
                table: "RevenuePlanLogs",
                column: "RevenuePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanCustomerDetails_RevenuePlanId",
                table: "RevenuePlanCustomerDetails",
                column: "RevenuePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanCashDetails_RevenuePlanId",
                table: "RevenuePlanCashDetails",
                column: "RevenuePlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RevenuePlanLogs_RevenuePlanId",
                table: "RevenuePlanLogs");

            migrationBuilder.DropIndex(
                name: "IX_RevenuePlanCustomerDetails_RevenuePlanId",
                table: "RevenuePlanCustomerDetails");

            migrationBuilder.DropIndex(
                name: "IX_RevenuePlanCashDetails_RevenuePlanId",
                table: "RevenuePlanCashDetails");

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanLogs_RevenuePlanId",
                table: "RevenuePlanLogs",
                column: "RevenuePlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanCustomerDetails_RevenuePlanId",
                table: "RevenuePlanCustomerDetails",
                column: "RevenuePlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevenuePlanCashDetails_RevenuePlanId",
                table: "RevenuePlanCashDetails",
                column: "RevenuePlanId",
                unique: true);
        }
    }
}
