using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPLX.Database.Migrations
{
    public partial class InitMisPharse2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLS",
                columns: table => new
                {
                    IDCLS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    S_ID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MaBN = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ngay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaDT = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgTru = table.Column<int>(type: "int", nullable: false),
                    NgayThu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserTC = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    QuaTC = table.Column<int>(type: "int", nullable: false),
                    DaTT = table.Column<int>(type: "int", nullable: false),
                    Del = table.Column<int>(type: "int", nullable: false),
                    TraTruoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienMat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThePOS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChKhoan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserCD = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TienGG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TTDiemPID = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KhoiDoanhThu = table.Column<int>(type: "int", nullable: false),
                    TienVoucher = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SIDFull = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsBSTraSau = table.Column<int>(type: "int", nullable: false),
                    SendLab = table.Column<int>(type: "int", nullable: false),
                    TienQR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tiendilai = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaDVQL = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLS", x => x.IDCLS)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CLSCT",
                columns: table => new
                {
                    IDCLSCT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDCLS = table.Column<int>(type: "int", nullable: false),
                    MaCP = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    SL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Del = table.Column<int>(type: "int", nullable: false),
                    SendLab = table.Column<int>(type: "int", nullable: false),
                    TongTienGG = table.Column<float>(type: "real", nullable: false),
                    TongTien = table.Column<float>(type: "real", nullable: false),
                    GGDiemPID = table.Column<float>(type: "real", nullable: false),
                    TongTienSau = table.Column<float>(type: "real", nullable: false),
                    CLS_ThanhToan = table.Column<int>(type: "int", nullable: false),
                    MaNhCP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLSCT", x => x.IDCLSCT)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DKK",
                columns: table => new
                {
                    MaBN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    P_ID = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Ngay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NS2 = table.Column<int>(type: "int", nullable: false),
                    GT = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DC = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MaTinh = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    MaHuyen = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaDT = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaLoaiDT = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    SoCM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IDHDKCB = table.Column<int>(type: "int", nullable: false),
                    Del = table.Column<int>(type: "int", nullable: false),
                    MaDV = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    KhoiDoanhThu = table.Column<int>(type: "int", nullable: false),
                    NgoaiKieu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhapNhanId = table.Column<int>(type: "int", nullable: false),
                    PhapNhanName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DKK", x => x.MaBN)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DM",
                columns: table => new
                {
                    IDTree = table.Column<int>(type: "int", nullable: false),
                    MaDM = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenDM = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    Stt = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DM", x => new { x.MaDM, x.IDTree })
                        .Annotation("SqlServer:Clustered", true);
                });

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
                    table.PrimaryKey("PK_DMBangGiaChiNhanh", x => x.MaBangGiaChiNhanh)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DMBS_ChuyenKhoa",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMBS_ChuyenKhoa", x => x.Ma)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DMCP",
                columns: table => new
                {
                    MaCP = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MaNN = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TenCP = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TenCPE = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DVT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DG = table.Column<float>(type: "real", nullable: false),
                    DGBH = table.Column<float>(type: "real", nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    MaNhCP = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    MaLoaiKT1 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaLoaiKT2 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MaLoaiKT3 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhanNhom = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TenRutGon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KhoaGGTrucTiep = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCP", x => x.MaCP)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DMDV",
                columns: table => new
                {
                    MaDV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenDV = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    PhapNhanId = table.Column<int>(type: "int", nullable: false),
                    MaSAP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaDVThanhVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaDVExSap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMDV", x => x.MaDV)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DMHuyen",
                columns: table => new
                {
                    MaHuyen = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenHuyen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMHuyen", x => x.MaHuyen)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "DMPN",
                columns: table => new
                {
                    PhapNhanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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

            migrationBuilder.CreateTable(
                name: "HDCTV",
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
                    table.PrimaryKey("PK_HDCTV", x => x.MaBS)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "HDCTVExt",
                columns: table => new
                {
                    MaBS = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_HDCTVExt", x => x.MaBS)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "HDKCB",
                columns: table => new
                {
                    IDHD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MaHD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenHD = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Del = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    ND = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HDKCB", x => x.IDHD)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "LoaiDeXuat",
                columns: table => new
                {
                    LoaiDeXuatCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LoaiDeXuatName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiDeXuat", x => x.LoaiDeXuatCode)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "NhCP",
                columns: table => new
                {
                    MaNhCP = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    TenNhCP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Stt = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhCP", x => x.MaNhCP)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ProfileCK",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileCKMa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProfileCKTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChuyenKhoaMa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileCK", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Relationship",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupRelationshipId = table.Column<int>(type: "int", nullable: false),
                    RelationshipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RelationshipName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    Createby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updateby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relationship", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CLS_IDCLS",
                table: "CLS",
                column: "IDCLS");

            migrationBuilder.CreateIndex(
                name: "IX_CLS_S_ID",
                table: "CLS",
                column: "S_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CLSCT_IDCLSCT",
                table: "CLSCT",
                column: "IDCLSCT");

            migrationBuilder.CreateIndex(
                name: "IX_DKK_MaBN",
                table: "DKK",
                column: "MaBN");

            migrationBuilder.CreateIndex(
                name: "IX_DKK_P_ID",
                table: "DKK",
                column: "P_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DM_IDTree",
                table: "DM",
                column: "IDTree");

            migrationBuilder.CreateIndex(
                name: "IX_DM_MaDM",
                table: "DM",
                column: "MaDM");

            migrationBuilder.CreateIndex(
                name: "IX_DMBangGiaChiNhanh_MaBangGiaChiNhanh",
                table: "DMBangGiaChiNhanh",
                column: "MaBangGiaChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_DMBangGiaChiNhanh_MaCP",
                table: "DMBangGiaChiNhanh",
                column: "MaCP");

            migrationBuilder.CreateIndex(
                name: "IX_DMBS_ChuyenKhoa_Ma",
                table: "DMBS_ChuyenKhoa",
                column: "Ma");

            migrationBuilder.CreateIndex(
                name: "IX_DMBS_ChuyenKhoa_Ten",
                table: "DMBS_ChuyenKhoa",
                column: "Ten");

            migrationBuilder.CreateIndex(
                name: "IX_DMCP_MaCP",
                table: "DMCP",
                column: "MaCP");

            migrationBuilder.CreateIndex(
                name: "IX_DMCP_TenCP",
                table: "DMCP",
                column: "TenCP");

            migrationBuilder.CreateIndex(
                name: "IX_DMDV_MaDV",
                table: "DMDV",
                column: "MaDV");

            migrationBuilder.CreateIndex(
                name: "IX_DMDV_TenDV",
                table: "DMDV",
                column: "TenDV");

            migrationBuilder.CreateIndex(
                name: "IX_DMHuyen_MaHuyen",
                table: "DMHuyen",
                column: "MaHuyen");

            migrationBuilder.CreateIndex(
                name: "IX_DMHuyen_TenHuyen",
                table: "DMHuyen",
                column: "TenHuyen");

            migrationBuilder.CreateIndex(
                name: "IX_DMPN_PhapNhanId",
                table: "DMPN",
                column: "PhapNhanId");

            migrationBuilder.CreateIndex(
                name: "IX_DMPN_PhapNhanName",
                table: "DMPN",
                column: "PhapNhanName");

            migrationBuilder.CreateIndex(
                name: "IX_HDCTV_MaBS",
                table: "HDCTV",
                column: "MaBS");

            migrationBuilder.CreateIndex(
                name: "IX_HDCTV_TenBS",
                table: "HDCTV",
                column: "TenBS");

            migrationBuilder.CreateIndex(
                name: "IX_HDCTVExt_MaBS",
                table: "HDCTVExt",
                column: "MaBS");

            migrationBuilder.CreateIndex(
                name: "IX_HDKCB_IDHD",
                table: "HDKCB",
                column: "IDHD");

            migrationBuilder.CreateIndex(
                name: "IX_HDKCB_MaHD",
                table: "HDKCB",
                column: "MaHD");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiDeXuat_LoaiDeXuatCode",
                table: "LoaiDeXuat",
                column: "LoaiDeXuatCode");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiDeXuat_LoaiDeXuatName",
                table: "LoaiDeXuat",
                column: "LoaiDeXuatName");

            migrationBuilder.CreateIndex(
                name: "IX_NhCP_MaNhCP",
                table: "NhCP",
                column: "MaNhCP");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileCK_Id",
                table: "ProfileCK",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileCK_ProfileCKMa",
                table: "ProfileCK",
                column: "ProfileCKMa");

            migrationBuilder.CreateIndex(
                name: "IX_Relationship_Id",
                table: "Relationship",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Relationship_RelationshipCode",
                table: "Relationship",
                column: "RelationshipCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CLS");

            migrationBuilder.DropTable(
                name: "CLSCT");

            migrationBuilder.DropTable(
                name: "DKK");

            migrationBuilder.DropTable(
                name: "DM");

            migrationBuilder.DropTable(
                name: "DMBangGiaChiNhanh");

            migrationBuilder.DropTable(
                name: "DMBS_ChuyenKhoa");

            migrationBuilder.DropTable(
                name: "DMCP");

            migrationBuilder.DropTable(
                name: "DMDV");

            migrationBuilder.DropTable(
                name: "DMHuyen");

            migrationBuilder.DropTable(
                name: "DMPN");

            migrationBuilder.DropTable(
                name: "HDCTV");

            migrationBuilder.DropTable(
                name: "HDCTVExt");

            migrationBuilder.DropTable(
                name: "HDKCB");

            migrationBuilder.DropTable(
                name: "LoaiDeXuat");

            migrationBuilder.DropTable(
                name: "NhCP");

            migrationBuilder.DropTable(
                name: "ProfileCK");

            migrationBuilder.DropTable(
                name: "Relationship");
        }
    }
}
