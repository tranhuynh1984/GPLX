using System;
using FluentValidation;
using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.CashFollow
{
    public class CompareCFAndActuallyRequest
    {
        public string Record { get; set; }

        public Guid RawId => !string.IsNullOrEmpty(Record) && !string.IsNullOrEmpty(RequestPage)
            ? Guid.TryParse(Record.StringAesDecryption(RequestPage, true), out var g) ? g : Guid.Empty
            : Guid.Empty;
        
        public int FromMonth { get; set; }
        public int ToMonth { get; set; }
        public int Year { get; set; }

        public string RequestPage { get; set; }
        public string UnitType { get; set; }
        public bool IsSub { get; set; }
        public int MaxFollowValue { get; set; }
    }

    public class CompareCFAndActuallyRequestValidator : AbstractValidator<CompareCFAndActuallyRequest>
    {
        public CompareCFAndActuallyRequestValidator()
        {
            //todo
            RuleFor(x => x.FromMonth).GreaterThan(0).WithMessage("Khoảng thời gian không hợp lệ!");
            RuleFor(x => x.ToMonth).GreaterThan(0).Must((m, n) => m.FromMonth < m.ToMonth).WithMessage("Khoảng thời gian không hợp lệ!");
            RuleFor(x => x.Year).GreaterThan(0).WithMessage("Khoảng thời gian không hợp lệ!");
            RuleFor(x => x.RawId).NotEmpty().WithMessage("Kế hoạch dòng tiền không hợp lệ!");
        }
    }
}
