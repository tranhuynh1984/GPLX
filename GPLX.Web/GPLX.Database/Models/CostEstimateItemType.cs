namespace GPLX.Database.Models
{
    public class CostEstimateItemType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PaymentType { get; set; }

        /// <summary>
        /// Phân loại chi phí để xác định vượt ngân sách:
        /// A1 - Chi phí tỷ lệ theo doanh thu - Vượt ngân sách nếu tỉ lệ chi/thu thực tế lớn hơn tỷ lệ chi/thu ngân sách
        /// A2 - Chi cố định, có kế hoạch - Vượt ngân sách nếu số tiền thực chi lớn hơn số tiền ngân sách
        ///
        /// </summary>
        public string ComparingType { get; set; }
        /// <summary>
        /// Phân loại nhóm chi phí
        /// Thuộc dạng đơn vị nào
        /// Trong y tế - ngoài y tế
        /// </summary>
        public string ForUnitType { get; set; }
    }
}
