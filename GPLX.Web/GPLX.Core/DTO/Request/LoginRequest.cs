using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Request
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(m => m.UserName).NotEmpty();
            RuleFor(m => m.Password).NotEmpty();
        }
    }
}
