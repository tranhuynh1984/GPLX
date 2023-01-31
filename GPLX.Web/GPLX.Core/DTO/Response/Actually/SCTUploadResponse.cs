using System;

namespace GPLX.Core.DTO.Response.Actually
{
    public class SCTView
    {
        /// <summary>
        /// Ngày
        /// </summary>
        public DateTime Day { get; set; }
        /// <summary>
        /// Số chứng từ
        /// </summary>
        public string BillCode { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>

        public string RequestContent { get; set; }
        /// <summary>
        /// Số tài khoản đối ứng
        /// </summary>
        public int AccountReciprocalNumber { get; set; }
        /// <summary>
        /// Phát sinh nợ
        /// </summary>
        public long? IncurredDebt { get; set; }
        /// <summary>
        /// Phát sinh nợ có
        /// </summary>
        public long? IncurredHave { get; set; }
        /// <summary>
        /// Số dư nợ
        /// </summary>
        public long? SurplusDebt { get; set; }
        /// <summary>
        /// Số dư có
        /// </summary>
        public long? SurplusHave { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Đường dẫn file excel
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Tên file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Từ file 111 hay 112
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Mã dự trù
        /// </summary>
        public string RequestCode { get; set; }
    }
}
