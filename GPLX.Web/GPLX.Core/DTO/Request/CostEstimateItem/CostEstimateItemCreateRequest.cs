using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace GPLX.Core.DTO.Request.CostEstimateItem
{
    public class CostEstimateItemCreateRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string Record { get; set; }

        public string RequestCode { get; set; }

        public string RequestContent { get; set; }
        /// <summary>
        /// Loại y/c chi phí
        /// </summary>
        public int CostEstimateItemTypeId { get; set; }
        /// <summary>
        /// Giá tiền
        /// </summary>
        [DisplayFormat(DataFormatString = "0:N0")]

        public long Cost { get; set; }

        public string CostDisplay => Cost.ToString("N0");
        /// <summary>
        /// Tuần đề xuất thanh toán
        /// </summary>
        public int PayWeek { get; set; }
        /// <summary>
        /// Hình thức chi
        /// </summary>
        public string PayForm { get; set; }
        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// Mã hóa đơn
        /// </summary>
        public string BillCode { get; set; }

        /// <summary>
        /// Ngày của hóa đơn
        /// </summary>
        /// 
        public DateTime? BillDate { get; set; }

        public string BillDateCreate { get; set; }

        /// <summary>
        /// Giá trị hóa đơn
        /// </summary>


        [DisplayFormat(DataFormatString = "0:N0")]
        public long? BillCost { get; set; }

        public string BillCostDisplay =>
            BillCost == null || BillCost == 0 ? string.Empty : BillCost?.ToString("N0");


        /// <summary>
        /// Thời gian theo format
        /// </summary>
        public string BillDateDisplay => BillDate == null || BillDate == DateTime.MinValue
            ? string.Empty
            : BillDate?.ToString("dd/M/yyyy");

        /// <summary>
        /// Giải trình với các khoản phát sính không có chứng từ
        /// </summary>
        public string Explanation { get; set; }

        /// <summary>
        /// Hình ảnh chứng từ
        /// </summary>
        public IFormFile Image { get; set; }
        /// <summary>
        /// Đường dẫn đến file upload
        /// </summary>
        public string FileView { get; set; }

        public string RequestImage { get; set; }

        public string UnitName { get; set; }
        public string DepartmentName { get; set; }
    }

    public class CostEstimateItemCreateRequestValidator : AbstractValidator<CostEstimateItemCreateRequest>
    {
        public CostEstimateItemCreateRequestValidator()
        {
            RuleFor(m => m.RequestContent).NotEmpty().WithMessage("Nội dung yêu cầu là bắt buộc");
            RuleFor(m => m.CostEstimateItemTypeId).NotEmpty().WithMessage("Loại yêu cầu chi phí là bắt buộc");
            RuleFor(m => m.Cost).NotEmpty().GreaterThanOrEqualTo(0).WithMessage("Giá tiền là bắt buộc");

            RuleFor(m => m.PayWeek).NotEmpty().WithMessage("Tuần đề xuất thanh toán là bắt buộc");

            RuleFor(m => m.BillDate).Must(date => date != default(DateTime)).When(a => !string.IsNullOrEmpty(a.BillDateCreate)).WithMessage("Ngày hóa đơn không đúng định dạng");

            RuleFor(x => x.Image).Custom((a, b) =>
            {
                if (a.Length > 10 * 1024 * 1024)
                    b.AddFailure("Vượt quá giới hạn cho phép là 10MB");

                if (!_allowImg.Contains(a.ContentType))
                    b.AddFailure("Không đúng định dạng cho phép");

            }).When(a => a.Image != null && a.Image.Length > 0);

            RuleFor(m => m).Custom((m, a) =>
            {
                if (!string.IsNullOrEmpty(m.BillCode) || m.BillDate != null || m.BillCost > 0)
                {
                    if (string.IsNullOrEmpty(m.BillCode) || m.BillDate == null || m.BillCost == null || m.BillCost <= 0)
                        a.AddFailure("Phải cung cấp đủ thông tin hóa đơn");
                }
                if ((m.Image == null || m.Image.Length <= 0) && !string.IsNullOrEmpty(m.RequestImage) && string.IsNullOrEmpty(m.Explanation))
                    a.AddFailure("Phải giải trình khi không có hình ảnh chứng từ");
            });
        }

        private readonly string[] _allowImg = {
            "image/jpeg"
            ,"image/jpg"
            ,"image/png"
            ,"image/gif"
            ,"image/webp"
            ,"application/pdf"
            ,"application/vnd.ms-excel"
            ,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            ,"application/msword"
            ,"application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };
    }


}
