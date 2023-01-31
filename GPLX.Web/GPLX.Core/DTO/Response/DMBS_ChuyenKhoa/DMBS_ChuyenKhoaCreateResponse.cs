using GPLX.Core.DTO.Response.DM;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.DMBS_ChuyenKhoa
{
    public class DMBS_ChuyenKhoaCreateResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ItemError> ListError { get; set; }
        public DMBS_ChuyenKhoaSearchResponseData Data { get; set; }
    }
}
