using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _20220425 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMBangGiaChiNhanh",
                columns: table => new
                {
                    MaBangGiaChiNhanh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaCP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DG = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMBangGiaChiNhanh", x => new { x.MaBangGiaChiNhanh, x.MaCP })
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMBangGiaChiNhanh_MaBangGiaChiNhanh",
                table: "DMBangGiaChiNhanh",
                column: "MaBangGiaChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_DMBangGiaChiNhanh_MaCP",
                table: "DMBangGiaChiNhanh",
                column: "MaCP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMBangGiaChiNhanh");
        }
    }
}
