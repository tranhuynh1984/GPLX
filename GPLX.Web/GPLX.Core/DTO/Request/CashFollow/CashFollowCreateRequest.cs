using System.Collections.Generic;
using FluentValidation;
using GPLX.Core.DTO.Response.CashFollow;

namespace GPLX.Core.DTO.Request.CashFollow
{
    public class CashFollowCreateRequest
    {
        public IList<CashFollowItemExcel> CashFollowItemExcels { get; set; }
        public IList<CashFollowAggregateExcel> CashFollowAggregateExcels { get; set; }

        public Database.Models.CashFollow CashFollow { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string CreatorName { get; set; }
        public int Creator { get; set; }
        public bool IsSub { get; set; }
        public string RequestPage { get; set; }
    }

    public class CashFollowCreateValidator : AbstractValidator<CashFollowCreateRequest>
    {
        public CashFollowCreateValidator()
        {
            RuleFor(x => x.CashFollow.Year).GreaterThan(2000).WithMessage("Năm lập ngân sách không hợp lệ!");
            RuleFor(x => x.CashFollowItemExcels).NotEmpty().WithMessage("Không tìm thấy dữ liệu chi tiết!");
            RuleFor(x => x.CashFollowAggregateExcels).NotEmpty().WithMessage("Không tìm thấy dữ liệu tổng hợp!");
            RuleFor(x => x.CashFollow.PathExcel).NotEmpty().WithMessage("Không tìm được tệp dữ liệu gốc!");
            RuleForEach(x => x.CashFollowItemExcels).ChildRules(p =>
            {
                p.RuleFor(g => g.CashFollowGroupId).GreaterThan(0).WithMessage("Nội dung không hợp lệ!");
            });
            //RuleForEach(x => x.CashFollowAggregateExcels).ChildRules(p =>
            //{
            //    p.RuleFor(g => g.CashFollowGroupId).GreaterThan(0).WithMessage("Nội dung không hợp lệ!");
            //});
        }
    }

}
