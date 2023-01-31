namespace GPLX.Core.DTO.Request.DMDV
{
    public class DMDVSearchRequest
    {
         public int Status { get; set; }

        public int Draw { get; set; }

        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public int PhapNhanId { get; set; }
        public string RequestPage { get; set; }
    }
}
