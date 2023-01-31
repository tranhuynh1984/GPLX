using System;

namespace GPLX.Database.Models
{
    public class CashFollowGroup
    {
        /// <summary>
        /// Id của nhóm 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Tên của nhóm
        /// Tương đương các chỉ mục lớn trong BM kế hoạch dòng tiền
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ID của nhóm cha nếu có
        /// </summary>
        public int ParentId { get; set; }
       
        /// <summary>
        /// Thứ tự
        /// </summary>
        public int Position { get; set; }

        public string RefPaymentId { get; set; }

        public string ForSubject { get; set; }
        
        /// <summary>
        /// Các rule áp dụng cho cell trong row
        /// </summary>
        public string RulesCellOnRow { get; set; }
        /// <summary>
        /// Bỏ qua validate với các cell tại vị trí trong row
        /// </summary>
        public string SkipCellAts { get; set; }
        /// <summary>
        /// Bắt buộc phải có trong biểu mẫu
        /// </summary>
        public bool IsRequire { get; set; }

        public string Style { get; set; }
        /// <summary>
        /// Dùng cho bảng chi tiết hay bảng tổng hợp
        /// </summary>
        public string GroupFor { get; set; }
    }
}
