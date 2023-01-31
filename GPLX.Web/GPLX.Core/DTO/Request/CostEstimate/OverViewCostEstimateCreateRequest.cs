using FluentValidation;

namespace GPLX.Core.DTO.Request.CostEstimate
{
    public class OverViewCostEstimateCreateRequest
    {
        public string[] Ids { get; set; }
    }

    public class COverViewCostEstimateCreateRequestValidator : AbstractValidator<OverViewCostEstimateCreateRequest>
    {
        public COverViewCostEstimateCreateRequestValidator()
        {
            RuleFor(m => m.Ids).NotEmpty().WithMessage("Danh sách yêu cầu là bắt buộc");
        }
    }
}
