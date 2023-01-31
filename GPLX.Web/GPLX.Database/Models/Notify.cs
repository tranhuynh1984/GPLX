using System;

namespace GPLX.Database.Models
{
    public class Notify
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Người nhận
        /// Danh sách phân tách bởi dấu ;
        /// </summary>
        public string Receiver { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Trạng thái xử lý
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
