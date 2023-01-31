namespace GPLX.Core.DTO.Request.Department
{
    public class DepartmentSearchRequest
    {
        public int Status { get; set; }

        public int Draw { get; set; }

        public string Keywords { get; set; }
        public string RequestPage { get; set; }

    }
}
