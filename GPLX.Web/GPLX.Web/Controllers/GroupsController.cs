using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.DTO.Request.Groups;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using Microsoft.Extensions.Logging;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Web.Controllers
{
    public class GroupsController : BaseController
    {
        private readonly ILogger<DepartmentController> _logger;
        private readonly IGroupsRepository _groupsRepository;

        public GroupsController(ILogger<DepartmentController> logger, IGroupsRepository groupsRepository)
        {
            _logger = logger;
            _groupsRepository = groupsRepository;
        }
        [AuthorizeUser(Module = Functions.GroupsView, Permission = PermissionConstant.VIEW)]

        public IActionResult List()
        {
            var model = new CostEstimateItemListModel
            {
                Stats = new List<StatusesGranted>
                {
                    new StatusesGranted {Value = (int) GlobalEnums.StatusDefaultEnum.Active,Name = GlobalEnums.OtherStatusNames[(int) GlobalEnums.StatusDefaultEnum.Active]},
                    new StatusesGranted {Value = (int) GlobalEnums.StatusDefaultEnum.InActive,Name = GlobalEnums.OtherStatusNames[(int) GlobalEnums.StatusDefaultEnum.InActive]},
                    new StatusesGranted {Value = (int) GlobalEnums.StatusDefaultEnum.All,Name = GlobalEnums.OtherStatusNames[(int) GlobalEnums.StatusDefaultEnum.All]},
                },
                DefaultStats = (int)GlobalEnums.StatusDefaultEnum.All,
                DefaultStatsName = GlobalEnums.DefaultStatusNames[(int)GlobalEnums.StatusDefaultEnum.All]
            };
            return View(model);
        }
        [AuthorizeUser(Module = Functions.GroupsView, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> Create(string record = default, string viewMode = default)
        {
            ViewBag.ViewMode = viewMode;
            GroupsSearchResponseData model = null;
            if (!string.IsNullOrEmpty(record))
            {
                model = await _groupsRepository.GetByIdAsyncView(record.StringAesDecryption(GroupsConst.PublicKey, true)
                    .ToInt32());
                model.Record = record;
            }
            return PartialView(model);
        }
        [AuthorizeUser(Module = Functions.GroupsView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, GroupsSearchRequest @base)
        {
            GroupsSearchResponse data;
            try
            {
                @base.Draw = Request.Query["draw"].ToString().ToInt32();
                @base.RequestPage = GroupsConst.PublicKey;
                data = await _groupsRepository.SearchAsync(start, length, @base);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new GroupsSearchResponse
                {
                    Code = (int)HttpStatusCode.NoContent,
                    Draw = @base.Draw,
                    Message = "Không tìm thấy dữ liệu yêu cầu!"
                });
            }

            return Json(data);
        }
        [AuthorizeUser(Module = Functions.GroupsView, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> OnCreate(GroupsCreateRequest request)
        {
            try
            {
                request.RequestPage = GroupsConst.PublicKey;
                request.Creator = GetUserId();
                request.CreatorName = GetUserName();
                var response = await _groupsRepository.Create(request);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new GroupsCreateResponse
                {
                    Message = GlobalEnums.ErrorMessage,
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                });
            }
        }
        [AuthorizeUser(Module = Functions.GroupsView, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> OffGroups(GroupsCreateRequest request)
        {
            try
            {
                request.RequestPage = GroupsConst.PublicKey;
                request.Creator = GetUserId();
                request.CreatorName = GetUserName();
                var response = await _groupsRepository.ChangeStatus(request);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new GroupsCreateResponse
                {
                    Message = GlobalEnums.ErrorMessage,
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                });
            }
        }
    }
}
