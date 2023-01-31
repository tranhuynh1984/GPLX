using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentValidation;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.Extensions;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Request.CostEstimateItem
{
    /// <summary>
    /// Dùng để import màn hình kế toán viên
    /// Dùng để pick yêu cầu tạo dự trù
    /// </summary>
    public class CostEstimateItemBulkCreateRequest
    {
        public string Record { get; set; }
        public Guid RawId => Guid.TryParse(Record.StringAesDecryption(PageRequest, true), out var g) ? g : Guid.Empty;
        public string PageRequest { get; set; }
        public List<CostEstimateItemFromExcel> Data { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public IList<PositionModel> Positions { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string HostPath { get; set; }
        public string ReportForWeekName { get; set; }
        public int ReportForWeek { get; set; }

        public int CostEstimateType { get; set; }
        public string UnitType { get; set; }

        public int MaxFollowStats { get; set; }
        public bool IsSub { get; set; }
    }
    public class CostEstimateItemBulkCreateRequestValidator : AbstractValidator<CostEstimateItemBulkCreateRequest>
    {
        public CostEstimateItemBulkCreateRequestValidator()
        {
            RuleFor(x => x.Data).Must(x => x.Count > 0).WithMessage("Không có yêu cầu nào được chọn!");
            //todo: uncomment on production
            //RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Vui lòng đăng nhập lại để thực hiện thao tác");
            RuleFor(x => x.CostEstimateType).GreaterThan(-1).WithMessage("Loại yêu cầu không hợp lệ");
            RuleForEach(x => x.Data).ChildRules(data =>
            {
                data.RuleFor(x => x.RequestCode).NotEmpty().WithMessage("Mã yêu cầu không được trống!");
                data.RuleFor(x => x.RequestContent).NotEmpty().WithMessage("Nội dung yêu cầu không được trống!");
                data.RuleFor(x => x.CostEstimateItemTypeName).NotEmpty().WithMessage("Nhóm dự trù không được trống!");
                data.RuleFor(x => x.Cost).GreaterThan(0).WithMessage("Tổng số tiền không được nhỏ hơn 0!");
                data.RuleFor(x => x.RequesterName).NotEmpty().WithMessage("Tên người yêu cầu không được trống!");
                data.RuleFor(x => x.PayWeekName).NotEmpty().Custom((x, y) =>
                {
                    var rgxMatch = new Regex("(Tuần)+(?:\\s)+([0-9]){1,2}",RegexOptions.IgnoreCase);
                    if (!rgxMatch.IsMatch(x))
                        y.AddFailure("Thời gian đề xuất thanh toán không đúng!");
                });
                data.RuleFor(x => x.AccountImage).NotEmpty()
                    .When(x => string.IsNullOrEmpty(x.Explanation)).WithMessage("Vui lòng nhập ảnh chứng từ!");
                data.RuleFor(x => x.Explanation).NotEmpty()
                    .When(x => string.IsNullOrEmpty(x.AccountImage)).WithMessage("Giải trình không được trống!");
                data.RuleFor(x => x.PayForm).NotEmpty().WithMessage("Hình thức chi không được trống!");
                data.RuleFor(x => x.CostEstimateGroupName).NotEmpty().WithMessage("Nhóm chi phí không được để trống!");
            });
        }
    }
}
