using GPLX.Core.Contracts;
using GPLX.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.User;
using Serilog;

namespace GPLX.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeUser : Attribute, IAsyncAuthorizationFilter
    {
        public string Module { get; set; }
        public int Permission { get; set; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var services = context.HttpContext.RequestServices;
                var groupService = services.GetRequiredService<IGroupsRepository>();
                var userService = services.GetRequiredService<IUserRepository>();

                var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
                

                var claimIdentity = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToInt32() ?? 0;
                var claimGroups = claimsIdentity?.FindFirst(ClaimTypes.GroupSid)?.Value.StringToListInt(";");
                var user = await userService.GetUserByIdAsync(claimIdentity);
                //if (user == null)
                //{
                //    context.HttpContext.Response.Redirect("/Account/Login");
                //    return;
                //}
                //if (!string.IsNullOrEmpty(Module))
                //{
                //    var userConcurrent = await userService.GetUserConcurrently(claimIdentity);
                //    if (userConcurrent?.Count > 0)
                //    {
                //        var oConcurrent = userConcurrent.FirstOrDefault(c => c.Selected);
                //        // trường hợp user đang chuyển sang vai trò kiêm nhiệm ở đơn vị khác
                //        if (oConcurrent != null)
                //        {
                //            var isAuth = await groupService.IsAuthorize(new List<int> { oConcurrent.GroupId }, Module, Permission);
                //            if (!isAuth)
                //            {
                //                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //                context.HttpContext.Response.Redirect("/Error/403");
                //            }
                //        }
                //        else
                //        {
                //            var isAuth = await groupService.IsAuthorize(claimGroups, Module, Permission);
                //            if (!isAuth)
                //            {
                //                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //                context.HttpContext.Response.Redirect("/Error/403");
                //            }
                //        }
                //    }
                //    else
                //    {
                //        var isAuth = await groupService.IsAuthorize(claimGroups, Module, Permission);
                //        if (!isAuth)
                //        {
                //            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //            context.HttpContext.Response.Redirect("/Error/403");
                //        }
                //    }

                //}
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                context.HttpContext.Response.Redirect("/Error/500");
            }
        }
    }
}
