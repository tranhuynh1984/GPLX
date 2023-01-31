using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _20220516_deleteHDCTV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HDCTV");

            migrationBuilder.AddColumn<int>(
                name: "CtvStatus",
                table: "DMCTV",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CtvStatus",
                table: "DMCTV");

            migrationBuilder.CreateTable(
                name: "HDCTV",
                columns: table => new
                {
                    MaBS = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BH_Ma_Khoa = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Bank = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true),
                    CMND = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChiNhanh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChuKy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChungChi_NgayCap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChungChi_NoiCap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChungChi_So = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChuyenKhoa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CtvStatus = table.Column<int>(type: "int", nullable: false),
                    DC1 = table.Column<string>(type: "nvarchar(190)", maxLength: 190, nullable: true),
                    DC2 = table.Column<string>(type: "nvarchar(190)", maxLength: 190, nullable: true),
                    DaHoanThienHoSo = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    GT = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    KetThucHD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDoIsActive = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MaChucDanh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaDTCTV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaHuyen = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MaHuyen2 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MaSap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaSoThue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MaTinh2 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Mobi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapCMND = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThucPL = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKyHD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKyPL = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiDaiDien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NoiCapCMND = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PassWeb = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SoHD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoPhuLuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoTK = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TTDeXuat = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    TenBS = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TenChuTK = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TenVietTat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserWeb = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HDCTV", x => x.MaBS)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HDCTV_MaBS",
                table: "HDCTV",
                column: "MaBS");

            migrationBuilder.CreateIndex(
                name: "IX_HDCTV_TenBS",
                table: "HDCTV",
                column: "TenBS");
        }
    }
}
