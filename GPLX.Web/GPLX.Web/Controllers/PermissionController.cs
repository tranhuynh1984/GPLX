using GPLX.Core.Contracts;
using GPLX.Core.DTO.Entities;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.DTO.Response.Permission;
using GPLX.Core.Extensions;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;

namespace GPLX.Web.Controllers
{
    public class PermissionController : BaseController
    {
        private readonly IGroupsRepository _groupsRepository;

        public PermissionController(IGroupsRepository groupsRepository)
        {
            _groupsRepository = groupsRepository;
        }
        [AuthorizeUser(Module = Functions.Permission, Permission = PermissionConstant.DELETE)]
        public async Task<IActionResult> Grant(string record)
        {
            try
            {
                int gId = !string.IsNullOrEmpty(record)
                    ? record.StringAesDecryption(GroupsConst.PublicKey, true).ToInt32()
                    : -1;
                var funcs = (await _groupsRepository.GetFunctionsAsync()).Where(x => !string.IsNullOrEmpty(x.Unique)).ToList();
                var groupPer = await _groupsRepository.GetPermissionAsync(gId);
                var oGroup = await _groupsRepository.GetByIdAsync(gId);
                ViewBag.Id = record;
                ViewBag.Funcs = funcs;
                ViewBag.Per = groupPer;
                ViewBag.oGroup = oGroup;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }
            return View();
        }
        [AuthorizeUser(Module = Functions.Permission, Permission = PermissionConstant.DELETE)]
        public async Task<IActionResult> Save(string record, PermissionUpdate[] pers)
        {
            try
            {
                int gId = !string.IsNullOrEmpty(record)
                    ? record.StringAesDecryption(GroupsConst.PublicKey, true).ToInt32()
                    : -1;
                foreach (var item in pers)
                {
                    await _groupsRepository.EditPermission(gId, item.Permission, item.Id);
                }

                return Json(new
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Cập nhật thông tin phân quyền thành công!"
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return Json(new
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Cập nhật thông tin phân quyền không thành công!"
                });
            }
        }
        [AuthorizeUser(Module = Functions.Permission, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> OnGrant(string record, string[] data)
        {
            try
            {
                int gId = !string.IsNullOrEmpty(record)
                    ? record.StringAesDecryption(GroupsConst.PublicKey, true).ToInt32()
                    : -1;
                var allFuncActions = data.Where(x => x.StartsWith("action", StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
                var permission = new List<PermissionUpdate>();
                allFuncActions.ForEach(x =>
                {
                    var separators = x.Split(':');
                    if (separators.Length == 3)
                        permission.Add(new PermissionUpdate { Id = separators[1].ToInt32(), Permission = separators[2].ToInt32() });
                });

                var sumPerms = permission.GroupBy(x => x.Id, (x, y) => new
                {
                    id = x,
                    pers = y.Sum(g => g.Permission)
                }).Select(x => new PermissionUpdate {Id = x.id, Permission = x.pers}).ToList();

                foreach (var item in sumPerms)
                    await _groupsRepository.EditPermission(gId, item.Permission, item.Id);

                await _groupsRepository.DeleteFuncInPermission(sumPerms, gId);
                
                return Json(new
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Cập nhật thông tin phân quyền thành công!"
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return Json(new
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Cập nhật thông tin phân quyền không thành công!"
                });
            }
        }
        [AuthorizeUser(Module = Functions.Permission, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> GrantTree(string record)
        {
            try
            {
                int gId = !string.IsNullOrEmpty(record)
                    ? record.StringAesDecryption(GroupsConst.PublicKey, true).ToInt32()
                    : -1;
                var oGroup = await _groupsRepository.GetByIdAsync(gId);
                ViewBag.Id = record;
                ViewBag.oGroup = oGroup;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }
            return View();
        }
        [AuthorizeUser(Module = Functions.Permission, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> LoadPerm(string record)
        {
            try
            {
                int gId = !string.IsNullOrEmpty(record)
                    ? record.StringAesDecryption(GroupsConst.PublicKey, true).ToInt32()
                    : -1;

                var response = await _groupsRepository.GetGrants(gId);
                return Json(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
            }

            return Json(new List<PermissionGrantResponse>());
        }
    }
}
