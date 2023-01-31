using GPLX.Core.DTO.Response.DM;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.LoaiDeXuat
{
    public class LoaiDeXuatCreateResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ItemError> ListError { get; set; }
        public LoaiDeXuatSearchResponseData Data { get; set; }
    }
}
