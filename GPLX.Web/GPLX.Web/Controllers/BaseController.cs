using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Core.Enum;
using GPLX.Core.Exceptions;
using GPLX.Core.Extensions;
using GPLX.Database.Models;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;

namespace GPLX.Web.Controllers
{
    [AfterActionFilter]
    public class BaseController : Controller
    {
        public enum ComparingMode
        {
            GatherThanEqual = 0,
            GatherThan = 1
        }

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
                    Units oUnit;
                    var userConcurrent = userRepository.GetUserConcurrently(userid).Result;
                    if (userConcurrent?.Count > 0)
                    {
                        var oConcurrent = userConcurrent.FirstOrDefault(c => c.Selected);
                        // trường hợp user đang chuyển sang vai trò kiêm nhiệm ở đơn vị khác
                        if (oConcurrent != null)
                        {
                            var oGroup = groupRepository.GetByIdAsync(oConcurrent.GroupId).Result;
                            oGroups.Add(oGroup);
                            oUnit = unitRepository.GetByIdAsync(oConcurrent.UnitId).Result;
                        }
                        else
                        {
                            oUnit = unitRepository.GetByIdAsync(user.UnitId ?? 0).Result;
                            oGroups = groupRepository.GetGroupsOurUser(user.Id).Result;
                        }
                    }
                    else
                    {
                        oGroups = groupRepository.GetGroupsOurUser(user.Id).Result;
                        oUnit = unitRepository.GetByIdAsync(user.UnitId ?? 0).Result;
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

                    string passDigitalSignature = string.Empty;
                    if (!string.IsNullOrEmpty(user.PasswordDigitalSignature) &&
                        !string.IsNullOrEmpty(user.AccDigitalSignature))
                    {
                        passDigitalSignature = user.PasswordDigitalSignature.StringAesDecryption(UsersConst.PublicKey);
                    }

                    return new UsersSessionModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        SyncUserId = user.UserId,
                        PathSignature = user.PathSignature,
                        AccountSignature = user.AccDigitalSignature,
                        PasswordSignature = passDigitalSignature,
                        Department = new DepartmentModel
                        {
                            DepartmentId = user.DepartmentId ?? 0,
                            DepartmentName = user.DepartmentName,
                        },
                        Positions = positions,
                        Unit = new UnitModel
                        {
                            UnitId = oUnit?.Id ?? 0,
                            UnitName = oUnit?.OfficesShortName ?? oUnit?.OfficesName,
                            UnitType = oUnit?.OfficesSub?.Equals("yt", StringComparison.OrdinalIgnoreCase) ?? false ? GlobalEnums.UnitIn : GlobalEnums.UnitOut, 
                            UnitCode = oUnit?.OfficesCode,
                            IsSub =  oUnit?.OfficesSub?.Equals("sub", StringComparison.OrdinalIgnoreCase) ?? false
                        }
                    };
                }
            }

            return new UsersSessionModel();
        }

        public int GetProcessRoleSessionModel()
        {
            var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            int userid = GetUserId();
            return userRepository.GetProcessRoleByUserIdAsync(userid).Result;
        }

        public UnitModel GetUserUnit()
        {
            var u = GetUsersSessionModel();
            return u.Unit;
        }

        public DepartmentModel GetUserDepartment()
        {
            var u = GetUsersSessionModel();
            return u.Department;
        }

        public IList<PositionModel> GetUserPosition()
        {
            var u = GetUsersSessionModel();
            return u.Positions;
        }

        public List<int> GetUserPositionIds()
        {
            var u = GetUsersSessionModel();
            return u.Positions?.Select(x => x.PositionId).ToList();
        }
        public IList<string> GetUserPositionCodes()
        {
            var u = GetUsersSessionModel();
            return u.Positions?.Select(x => x.PositionCode).ToList();
        }
        /// <summary>
        /// ID của user
        /// </summary>
        /// <returns></returns>
        public int GetUserId()
        {
            var u = GetUsersSessionModel();
            //Return to login page
            if (u == null)
            {
                throw new AuthenticationException("Cần đăng nhập để sử dụng tính năng này!");
            }
            return u.UserId;
        }
        /// <summary>
        /// UserName của người đang đăng nhập
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            var u = GetUsersSessionModel();
            return u.UserName;
        }

        public string GetUserSyncId()
        {
            var u = GetUsersSessionModel();
            return u.SyncUserId;
        }

        public string GetIDRole()
        {
            var u = GetUsersSessionModel();
            return u.SyncUserId;
        }

        /// <summary>
        /// Lấy token từ request
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Người đang đăng nhập thuộc đơn vị SUB
        /// </summary>
        /// <returns></returns>
        public bool IsSubUnit()
        {
            return GetUserUnit().IsSub;
        }

        public bool IsSubUnit(UsersSessionModel session)
        {
            return session?.Unit?.IsSub ?? false;
        }

        /// <summary>
        /// So sánh level hiện tại của nhóm
        /// Với vị trí của nhóm được truyền vào
        /// </summary>
        /// <param name="levelCompare"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool ComparePositionLevelUpper(int levelCompare, ComparingMode mode = ComparingMode.GatherThanEqual)
        {
            var positions = GetUserPosition();
            if (positions == null)
                return false;
            foreach (var positionModel in positions)
            {
                if (positionModel.PositionLevel >= levelCompare && mode == ComparingMode.GatherThanEqual)
                    return true;
                else if (positionModel.PositionLevel > levelCompare && mode == ComparingMode.GatherThan)
                    return true;
            }

            return false;
        }

    }
}
