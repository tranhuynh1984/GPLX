using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class _20220428 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMCTV",
                columns: table => new
                {
                    MaBS = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenBS = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NguoiDaiDien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaChucDanh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChuyenKhoa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DC1 = table.Column<string>(type: "nvarchar(190)", maxLength: 190, nullable: true),
                    MaTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MaHuyen = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DC2 = table.Column<string>(type: "nvarchar(190)", maxLength: 190, nullable: true),
                    MaTinh2 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MaHuyen2 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Mobi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    CK = table.Column<int>(type: "int", nullable: false),
                    HTCKDoiTuong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CMND = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NgayCapCMND = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoiCapCMND = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoTK = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Bank = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true),
                    TenChuTK = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoHD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaDVCT = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    LyDoIsActive = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TraSau = table.Column<int>(type: "int", nullable: false),
                    MaDTCTV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UserWeb = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PassWeb = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ChiNhanh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaSoThue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChungChi_So = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChungChi_NgayCap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChungChi_NoiCap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayKyHD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenVietTat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TTDeXuat = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GT = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    CKKH = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    KetThucHD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaHoanThienHoSo = table.Column<int>(type: "int", nullable: false),
                    BH_Ma_Khoa = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    ChuKy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaSap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SoPhuLuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayKyPL = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThucPL = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCTV", x => x.MaBS)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DMCTVExt",
                columns: table => new
                {
                    MaBS = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ExtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCTVExt", x => x.MaBS)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CTVGROUP",
                columns: table => new
                {
                    CTVGroupID = table.Column<int>(type: "int", nullable: false),
                    CTVGroupName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CTVGROUP", x => x.CTVGroupID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CTVGROUPSUB",
                columns: table => new
                {
                    SubId = table.Column<int>(type: "int", nullable: false),
                    CTVGroupID = table.Column<int>(type: "int", nullable: false),
                    SubName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    IsUse = table.Column<int>(type: "int", nullable: false),
                    WhyNotUse = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DateI = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserI = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DisCount = table.Column<float>(type: "real", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CTVGROUPSUB", x => x.SubId)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CTVGROUPSUB1_DETAIL",
                columns: table => new
                {
                    SubId = table.Column<int>(type: "int", nullable: false),
                    MaCP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BP1 = table.Column<float>(type: "real", nullable: false),
                    BP2 = table.Column<float>(type: "real", nullable: false),
                    BP3 = table.Column<float>(type: "real", nullable: false),
                    BP4 = table.Column<float>(type: "real", nullable: false),
                    BP5 = table.Column<float>(type: "real", nullable: false),
                    BP6 = table.Column<float>(type: "real", nullable: false),
                    BP7 = table.Column<float>(type: "real", nullable: false),
                    BP8 = table.Column<float>(type: "real", nullable: false),
                    BP9 = table.Column<float>(type: "real", nullable: false),
                    BP10 = table.Column<float>(type: "real", nullable: false),
                    BP11 = table.Column<float>(type: "real", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CTVGROUPSUB1_DETAIL", x => new { x.SubId, x.MaCP })
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CTVGROUPSUB2_DETAIL",
                columns: table => new
                {
                    SubId = table.Column<int>(type: "int", nullable: false),
                    MaCP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FixedPrice = table.Column<float>(type: "real", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CTVGROUPSUB2_DETAIL", x => new { x.SubId, x.MaCP })
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMCTV_MaBS",
                table: "DMCTV",
                column: "MaBS");

            migrationBuilder.CreateIndex(
                name: "IX_DMCTV_TenBS",
                table: "DMCTV",
                column: "TenBS");

            migrationBuilder.CreateIndex(
                name: "IX_DMCTVExt_MaBS",
                table: "DMCTVExt",
                column: "MaBS");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CTVGROUP_CTVGroupID",
                table: "TBL_CTVGROUP",
                column: "CTVGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CTVGROUP_CTVGroupName",
                table: "TBL_CTVGROUP",
                column: "CTVGroupName");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CTVGROUPSUB_SubId",
                table: "TBL_CTVGROUPSUB",
                column: "SubId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CTVGROUPSUB_SubName",
                table: "TBL_CTVGROUPSUB",
                column: "SubName");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CTVGROUPSUB1_DETAIL_MaCP",
                table: "TBL_CTVGROUPSUB1_DETAIL",
                column: "MaCP");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CTVGROUPSUB1_DETAIL_SubId",
                table: "TBL_CTVGROUPSUB1_DETAIL",
                column: "SubId");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CTVGROUPSUB2_DETAIL_MaCP",
                table: "TBL_CTVGROUPSUB2_DETAIL",
                column: "MaCP");

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CTVGROUPSUB2_DETAIL_SubId",
                table: "TBL_CTVGROUPSUB2_DETAIL",
                column: "SubId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMCTV");

            migrationBuilder.DropTable(
                name: "DMCTVExt");

            migrationBuilder.DropTable(
                name: "TBL_CTVGROUP");

            migrationBuilder.DropTable(
                name: "TBL_CTVGROUPSUB");

            migrationBuilder.DropTable(
                name: "TBL_CTVGROUPSUB1_DETAIL");

            migrationBuilder.DropTable(
                name: "TBL_CTVGROUPSUB2_DETAIL");

        }
    }
}
