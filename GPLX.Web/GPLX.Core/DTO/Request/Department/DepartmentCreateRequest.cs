using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.Department
{
    public class DepartmentCreateRequest
    {
        public string Record { get; set; }
        public string RequestPage { get; set; }

        public int RawId => !string.IsNullOrEmpty(Record) && !string.IsNullOrEmpty(RequestPage)
            ? int.TryParse(Record.StringAesDecryption(RequestPage, true), out var i) ? i : 0
            : 0;
        public string Name { get; set; }
        public int Status { get; set; }


        public string CreatorName { get; set; }
        public int Creator { get; set; }
    }
}
