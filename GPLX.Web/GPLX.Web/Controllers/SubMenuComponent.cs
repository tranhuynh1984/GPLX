using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Core.Enum;
using GPLX.Database.Models;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace GPLX.Web.Controllers
{
    public class SubMenuViewComponent : ViewComponent
    {
        public UsersSessionModel GetUsersSessionModel()
        {
            var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var unitRepository = HttpContext.RequestServices.GetRequiredService<IUnitRepository>();
            var groupRepository = HttpContext.RequestServices.GetRequiredService<IGroupsRepository>();

            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToInt32() ?? 0;
                if (userid == 0)
                    return null;

                var user = userRepository.GetUserByIdAsync(userid).Result;
                if (user != null)
                {
                    IList<Groups> oGroups = new List<Groups>();
                    var oUnit = unitRepository.GetByIdAsync(user.UnitId ?? 0).Result;
                    var userConcurrent = userRepository.GetUserConcurrently(userid).Result;
                    if (userConcurrent?.Count > 0)
                    {
                        var oConcurrent = userConcurrent.FirstOrDefault(c => c.Selected);
                        // trường hợp user đang chuyển sang vai trò kiêm nhiệm ở đơn vị khác
                        if (oConcurrent != null)
                        {
                            var oGroup = groupRepository.GetByIdAsync(oConcurrent.GroupId).Result;
                            oGroups.Add(oGroup);
                        }
                        else
                            oGroups = groupRepository.GetGroupsOurUser(user.Id).Result;
                    }
                    else
                        oGroups = groupRepository.GetGroupsOurUser(user.Id).Result;
                    var positions = new List<PositionModel>();

                    if (oGroups != null)
                    {
                        foreach (var oGroup in oGroups)
                        {
                            positions.Add(new PositionModel
                            {
                                PositionCode = oGroup.GroupCode,
                                PositionId = oGroup.Id,
                                PositionLevel = oGroup.Order,
                                PositionName = oGroup.Name
                            });
                        }
                    }

                    return new UsersSessionModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        SyncUserId = user.UserId,
                        PathSignature = user.PathSignature,
                        Department = new DepartmentModel
                        {
                            DepartmentId = user.DepartmentId ?? 0,
                            DepartmentName = user.DepartmentName,
                        },
                        Positions = positions,
                        Unit = new UnitModel
                        {
                            UnitId = user.UnitId ?? 0,
                            UnitName = oUnit?.OfficesShortName ?? oUnit?.OfficesName,
                            UnitType = (oUnit?.OfficesSub?.Equals("yt", StringComparison.OrdinalIgnoreCase) ?? false) ? GlobalEnums.UnitIn : GlobalEnums.UnitOut,
                            UnitCode = oUnit?.OfficesCode,
                            IsSub = oUnit?.OfficesSub?.Equals("sub", StringComparison.OrdinalIgnoreCase) ?? false
                        }
                    };
                }
            }

            return new UsersSessionModel();
        }

        public async Task<IViewComponentResult> InvokeAsync(MenuBuilder subBuilder)
        {
            return View("_SubMenuPartial", subBuilder);
        }
    }
}
