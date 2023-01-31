using System;

namespace GPLX.Database.Models
{
    // Dùng để đếm số lượng tệp đã đc export sang pdf
    public class FilePdfCreateLogs
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public string Type { get; set; }
        public int UnitId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
