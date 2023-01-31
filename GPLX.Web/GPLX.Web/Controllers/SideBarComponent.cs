using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GPLX.Core.Contants;
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
using Functions = GPLX.Database.Models.Functions;

namespace GPLX.Web.Controllers
{
    public class SideBarViewComponent : ViewComponent
    {
        private readonly IGroupsRepository _groupsRepository;

        public SideBarViewComponent(IGroupsRepository groupsRepository)
        {
            _groupsRepository = groupsRepository;
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
                            UnitName = oUnit.OfficesShortName ?? oUnit.OfficesName,
                            UnitType = (oUnit?.OfficesSub?.Equals("yt", StringComparison.OrdinalIgnoreCase) ?? false) ? GlobalEnums.UnitIn : GlobalEnums.UnitOut,
                            UnitCode = oUnit?.OfficesCode,
                            IsSub = oUnit?.OfficesSub?.Equals("sub", StringComparison.OrdinalIgnoreCase) ?? false
                        }
                    };
                }
            }

            return new UsersSessionModel();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                IList<MenuBuilder> listVerifies = new List<MenuBuilder>();
                var userSession = GetUsersSessionModel();
                if (userSession != null)
                {
                    // load tất cả các function
                    // chỉ lấy các function đc cấu hình hiển thị trên menu
                    var allFunctions = (await _groupsRepository.GetFunctionsAsync()).Where(x => x.DisplayOnMenu == 1).ToList();
                    var allPermissions = await _groupsRepository.GetAllPermission();
                    // các function cấp lớn nhất

                    var allParents = allFunctions.Where(x => x.ParentId == 0);
                    IList<MenuBuilder> listBuilder = new List<MenuBuilder>();
                    foreach (var func in allParents)
                    {
                        listBuilder.Add(new MenuBuilder
                        {
                            Parent = func,
                            ChildFunctions = _GetSubMenuBuilders(allFunctions, func)
                        });
                    }

                    var listFuncEmpty = listBuilder.Where(x => (x.ChildFunctions == null || x.ChildFunctions.Count == 0)
                                                               && string.IsNullOrEmpty(x.Parent.Url) && !string.IsNullOrEmpty(x.Parent.Unique)).ToList();
                    // xóa các nhóm cha không có 
                    listFuncEmpty.ForEach(x => listBuilder.Remove(x));

                    //required function format
                    //{Identify}@{Action}
                    listBuilder = listBuilder.OrderBy(x => x.Parent.Order).ToList();
                    listVerifies = _VerifiesMenu(listBuilder, allPermissions, allFunctions, userSession);

                    #region comment

                    //foreach (var menuBuilder in listBuilder)
                    //{
                    //    // menu không có menu cấp 2
                    //    if (!string.IsNullOrEmpty(menuBuilder.Parent.Url))
                    //    {
                    //        string uniqueFunc = menuBuilder.Parent.Unique;
                    //        string[] uniqueFuncSeparator = uniqueFunc.Split('@');
                    //        if (uniqueFuncSeparator.Length == 2)
                    //        {
                    //            string actInUnique = uniqueFuncSeparator[1].Trim().ToUpper();
                    //            int iAction = PermissionConstant.PermissionKeyToValue.TryGetValue(actInUnique, out var i) ? i : -1;
                    //            if (iAction != -1)
                    //            {
                    //                var bAuthorized = _groupsRepository.IsAuthorize(userSession.Position.PositionId,
                    //                     menuBuilder.Parent.Unique, iAction, allPermissions, allFunctions);
                    //                if (bAuthorized)
                    //                    listVerifies.Add(menuBuilder);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (string.IsNullOrEmpty(menuBuilder.Parent.Unique) && menuBuilder.ChildFunctions.Count == 0)
                    //        {
                    //            listVerifies.Add(menuBuilder);
                    //            continue;
                    //        }
                    //        var menuBuilderLevel = new MenuBuilder { Parent = menuBuilder.Parent, ChildFunctions = new List<Functions>() };
                    //        foreach (var fc in menuBuilder.ChildFunctions)
                    //        {
                    //            string[] uniqueFuncSeparator = fc.Unique.Split('@');
                    //            if (uniqueFuncSeparator.Length == 2)
                    //            {
                    //                string actInUnique = uniqueFuncSeparator[1].Trim().ToUpper();
                    //                int iAction = PermissionConstant.PermissionKeyToValue.TryGetValue(actInUnique, out var i) ? i : -1;
                    //                if (iAction != -1)
                    //                {
                    //                    var bAuthorized = _groupsRepository.IsAuthorize(userSession.Position.PositionId,
                    //                        fc.Unique, iAction, allPermissions, allFunctions);
                    //                    if (bAuthorized)
                    //                        menuBuilderLevel.ChildFunctions.Add(fc);
                    //                }
                    //            }
                    //        }

                    //        menuBuilderLevel.ChildFunctions =
                    //            menuBuilderLevel.ChildFunctions.OrderBy(x => x.Order).ToList();
                    //        if (menuBuilderLevel.ChildFunctions.Count > 0)
                    //            listVerifies.Add(menuBuilderLevel);
                    //    }
                    //}

                    #endregion

                }

                var systemConfigElem = listVerifies.FirstOrDefault(x => x.Parent.Name.Contains("Quản trị hệ thống"));
                if (systemConfigElem != null)
                {
                    if (!listVerifies.Any(x => x.Parent.Order > systemConfigElem.Parent.Order))
                        listVerifies.Remove(systemConfigElem);
                }

                listVerifies = listVerifies.OrderBy(x => x.Parent.Order).ToList();
                ViewBag.Loged = userSession != null;

                return View("_MenuPartial", listVerifies);

            }
            catch (Exception)
            {
                return View("_MenuPartial", new List<MenuBuilder>());
            }
        }

        public IList<MenuBuilder> _GetSubMenuBuilders(List<Functions> all, Functions f)
        {
            IList<MenuBuilder> response = new List<MenuBuilder>();
            var finds = all.Where(x => x.ParentId == f.Id).ToList();
            if (finds.Any())
            {
                finds.ForEach(p =>
                {
                    response.Add(new MenuBuilder
                    {
                        Parent = p,
                        ChildFunctions = _GetSubMenuBuilders(all, p)
                    });
                });
            }

            response = response.OrderBy(x => x.Parent.Order).ToList();
            return response;
        }

        public IList<MenuBuilder> _VerifiesMenu(IList<MenuBuilder> originals, IList<GroupsPermission> permissions, IList<Functions> all, UsersSessionModel session)
        {
            var verifiesResult = new List<MenuBuilder>();
            foreach (var org in originals)
            {
                // Trường hợp menu cha đc gắn link
                // thì không xóa hết các menu con

                if (!string.IsNullOrEmpty(org.Parent.Url))
                {
                    string uniqueFunc = org.Parent.Unique;
                    string[] uniqueFuncSeparator = uniqueFunc.Split('@');
                    if (uniqueFuncSeparator.Length == 2)
                    {
                        string actInUnique = uniqueFuncSeparator[1].Trim().ToUpper();
                        int iAction = PermissionConstant.PermissionKeyToValue.TryGetValue(actInUnique, out var i) ? i : -1;
                        if (iAction != -1)
                        {
                            var bAuthorized = _groupsRepository.IsAuthorizePath(session.Positions?.Select(x => x.PositionId).ToList(),
                                org.Parent.Id, iAction, permissions, all);
                            if (bAuthorized)
                            {
                                org.ChildFunctions = null;
                                verifiesResult.Add(org);
                            }
                        }
                    }
                }
                else
                {
                    // Các menu phân nhóm chức năng
                    if (string.IsNullOrEmpty(org.Parent.Unique) && org.ChildFunctions.Count == 0)
                    {
                        verifiesResult.Add(org);
                        continue;
                    }
                    var subMenuBuilder = new MenuBuilder
                    {
                        Parent = org.Parent,
                        ChildFunctions = new List<MenuBuilder>()
                    };
                    foreach (var fc in org.ChildFunctions)
                    {
                        string[] uniqueFuncSeparator = fc.Parent.Unique.Split('@');
                        if (uniqueFuncSeparator.Length == 2)
                        {
                            string actInUnique = uniqueFuncSeparator[1].Trim().ToUpper();
                            int iAction = PermissionConstant.PermissionKeyToValue.TryGetValue(actInUnique, out var i) ? i : -1;
                            if (iAction != -1)
                            {
                                var bAuthorized = _groupsRepository.IsAuthorizePath(session.Positions?.Select(x => x.PositionId).ToList(),
                                    fc.Parent.Id, iAction, permissions, all);
                                if (fc.ChildFunctions.Count > 0)
                                    fc.ChildFunctions = _VerifiesMenu(fc.ChildFunctions, permissions, all, session);

                                if (bAuthorized || fc.ChildFunctions.Count > 0)
                                    subMenuBuilder.ChildFunctions.Add(fc);
                            }
                        }
                    }
                    if (subMenuBuilder.ChildFunctions.Count > 0)
                        verifiesResult.Add(subMenuBuilder);
                }
            }
            return verifiesResult;
        }
    }
}
