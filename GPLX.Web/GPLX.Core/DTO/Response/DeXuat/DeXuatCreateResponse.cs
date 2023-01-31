using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.DeXuat
{
    public class DeXuatCreateResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ItemError> ListError { get; set; }
        public DeXuatSearchResponseData Data { get; set; }
    }

    public class ItemError
    {
        public string Message { get; set; }
        public string FieldError { get; set; }

        public string FieldType { get; set; }
    }
}
