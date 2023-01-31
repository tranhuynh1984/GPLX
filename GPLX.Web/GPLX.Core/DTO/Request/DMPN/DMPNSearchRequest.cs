namespace GPLX.Core.DTO.Request.DMPN
{
    public class DMPNSearchRequest
    {
        public int? Status { get; set; }

        public int Draw { get; set; }

        public string MaPhapNhan { get; set; }
        public string TenPhapNhan { get; set; }
        public string RequestPage { get; set; }
    }
}
