namespace GPLX.Core.DTO.Request.DMCP
{
    public class DMCPSearchRequest
    {
        //Trạng thái
        public int Status { get; set; } = 1;

        //...
        public int Draw { get; set; }

        //Mã dịch vụ
        public string MaCP { get; set; }

        // Tên dịch vụ
        public string Keywords { get; set; }

        // Mã nhóm dịch vụ
        public string MaNhCP { get; set; }

        //Mã bảng giá đơn vị
        public string BranchCode { get; set; }

        //Request
        public string RequestPage { get; set; }
    }
}