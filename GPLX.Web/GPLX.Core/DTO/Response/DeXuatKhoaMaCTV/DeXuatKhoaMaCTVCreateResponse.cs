using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.DeXuatKhoaMaCTV
{
    public class DeXuatKhoaMaCTVCreateResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ItemError> ListError { get; set; }
        public DeXuatKhoaMaCTVSearchResponseData Data { get; set; }
    }

    public class ItemError
    {
        public string Message { get; set; }
        public string FieldError { get; set; }
    }
}
