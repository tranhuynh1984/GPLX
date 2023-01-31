using System.Collections.Generic;

namespace GPLX.Core.DTO.Entities
{
    public class ExcelCell
    {
        /// <summary>
        /// Tên cell
        /// </summary>
        public string CellName { get; set; }
        /// <summary>
        /// Dữ liệu map với property trong object
        /// </summary>
        public string FieldMapper { get; set; }
        /// <summary>
        /// Dữ liệu sau khi đọc
        /// </summary>
        public object ReaderVal { get; set; }
        /// <summary>
        /// Giá trị string của cell
        /// </summary>
        public string StringCellValue { get; set; }

        /// <summary>
        /// Cell dữ liệu này có lỗi không
        /// </summary>
        public bool IsNotValidCell { get; set; }
        /// <summary>
        /// Mã lỗi (nếu có)
        /// </summary>
        public string ErrorMessage { get; set; }

        public int CellPosition { get; set; }
        public bool IsHeader { get; set; }
    }

    public class ExcelRow
    {
        public int RowIndex { get; set; }
        /// <summary>
        /// ID nhóm dữ liệu
        /// </summary>
        public int GroupId { get; set; }
        /// <summary>
        /// Tên nhóm dữ liệu
        /// </summary>
        public string GroupName { get; set; }
        public string Style { get; set; }
        public List<ExcelCell> Cells { get; set; }
        public string Rules { get; set; }
        public string SkipCellAts { get; set; }
    }
}
