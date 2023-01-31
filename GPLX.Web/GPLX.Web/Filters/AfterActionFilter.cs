using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.User;
using GPLX.Database.Models;
using GPLX.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GPLX.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AfterActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {


            GetPosition(context);
            base.OnActionExecuted(context);
        }

        public void GetPosition(ActionExecutedContext context)
        {
            try
            {
                var uPositions = GetUserPosition(context);
                if (uPositions == null || !uPositions.Any())
                    return;
                int max = uPositions.Max(x => x.PositionLevel);
                var uPos = uPositions.FirstOrDefault(x => x.PositionLevel == max);
                if (context.Controller is Controller controller)
                {
                    controller.ViewBag.NameJob = uPos?.PositionName;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }
        }

        public IList<PositionModel> GetUserPosition(ActionExecutedContext context)
        {
            var u = GetUsersSessionModel(context);
            return u?.Positions;
        }


        public UsersSessionModel GetUsersSessionModel(ActionExecutedContext context)
        {
            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            //var unitRepository = context.HttpContext.RequestServices.GetRequiredService<IUnitRepository>();
            var groupRepository = context.HttpContext.RequestServices.GetRequiredService<IGroupsRepository>();

            if (context.HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToInt32() ?? 0;
                if (userid == 0)
                    return null;

                var user = userRepository.GetUserByIdAsync(userid).Result;
                if (user != null)
                {
                    IList<Groups> oGroups = new List<Groups>();
                    //Units oUnit;
                    var userConcurrent = userRepository.GetUserConcurrently(userid).Result;
                    if (userConcurrent?.Count > 0)
                    {
                        var oConcurrent = userConcurrent.FirstOrDefault(c => c.Selected);
                        // trường hợp user đang chuyển sang vai trò kiêm nhiệm ở đơn vị khác
                        if (oConcurrent != null)
                        {
                            var oGroup = groupRepository.GetByIdAsync(oConcurrent.GroupId).Result;
                            oGroups.Add(oGroup);
                            //oUnit = unitRepository.GetByIdAsync(oConcurrent.UnitId).Result;
                        }
                        else
                        {
                            // oUnit = unitRepository.GetByIdAsync(user.UnitId ?? 0).Result;
                            oGroups = groupRepository.GetGroupsOurUser(user.Id).Result;
                        }
                    }
                    else
                    {
                        oGroups = groupRepository.GetGroupsOurUser(user.Id).Result;
                        //oUnit = unitRepository.GetByIdAsync(user.UnitId ?? 0).Result;
                    }
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
                        //PathSignature = user.PathSignature,
                        //Department = new DepartmentModel
                        //{
                        //    DepartmentId = user.DepartmentId ?? 0,
                        //    DepartmentName = user.DepartmentName,
                        //},
                        Positions = positions,
                        //Unit = new UnitModel
                        //{
                        //    UnitId = oUnit?.Id ?? 0,
                        //    UnitName = oUnit?.OfficesName,
                        //    UnitType = unitRepository.GetUnitType(oUnit?.OfficesCode).Result,
                        //    UnitCode = oUnit?.OfficesCode,
                        //    IsSub = !string.IsNullOrEmpty(oUnit?.OfficesSub)
                        //}
                    };
                }
            }

            return new UsersSessionModel();
        }
    }
}
