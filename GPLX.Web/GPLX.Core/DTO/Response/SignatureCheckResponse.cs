using GPLX.Core.DTO.Response.CostStatus;

namespace GPLX.Core.DTO.Response
{
    public class SignatureCheckResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool IsSignature { get; set; }
        public bool IsApproval { get; set; }
        public StatusesGranted Granted { get; set; }
        /// <summary>
        /// Người được duyệt
        /// </summary>
        public Database.Models.Groups Position { get; set; }
        /// <summary>
        /// Step cuối
        /// </summary>
        public bool IsMaxStep { get; set; }
    }
}
