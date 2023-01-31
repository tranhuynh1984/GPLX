namespace GPLX.Infrastructure.Constants
{
    public class AppConstant
    {
        public static class TemplatePath
        {
            public static readonly string DE_XUAT_KHOA_MA_CTVBS = "/DeXuatKhoaMaCTVBS.html";
            public static readonly string DE_XUAT_LC_MA_CTVBS = "/DeXuatLuanChuyenMaCTVBS.html";
            public static readonly string DE_XUAT_MO_MA_CTVBS = "/DeXuatMoMaCTVBS.html";
            public static readonly string DE_XUAT_SUA_MA_CTVBS = "/DeXuatSuaMaCTVBS.html";
            public static readonly string DE_XUAT_TAO_MA_CTVBS = "/DeXuatTaoMaCTVBS.html";
        }

        public static class DeXuatCode
        {
            public static readonly string MO_MA = "DeXuatMoMa";
        }

        public static class DeXuatPermission
        {
            public static readonly int DELETE = -1; //Delete
            public static readonly int INIT = 0; //Tạo mới hoặc chỉnh sửa, chưa đẩy luồng duyệt
            public static readonly int DONE = 1; //Done
            public static readonly int VIEW = 2; //Không có quyền tương tác
            public static readonly int WAIT_APPROVE = 3; //Chờ duyệt
        }

        public static class Process
        {
            public static readonly int FIRST_STEP = 0;
        }
    }
}