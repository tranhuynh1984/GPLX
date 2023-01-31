using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPLX.Web.Filters
{
    public class MedSecurityStampValidator : ISecurityStampValidator
    {
        public async Task ValidateAsync(CookieValidatePrincipalContext context)
        {

        }
    }
}
