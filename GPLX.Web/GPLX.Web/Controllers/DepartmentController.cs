using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts.Department;
using GPLX.Core.DTO.Request.Department;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.DTO.Response.Department;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database.Models;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GPLX.Web.Controllers
{
    /// <summary>
    /// Quản lý phòng ban
    /// </summary>
    public class DepartmentController : BaseController
    {
        private readonly ILogger<DepartmentController> _logger;
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentController(ILogger<DepartmentController> logger, IDepartmentRepository departmentRepository)
        {
            _logger = logger;
            _departmentRepository = departmentRepository;
        }
        [AuthorizeUser(Module = Core.Contants.Functions.DepartmentView, Permission = PermissionConstant.VIEW)]

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
        [AuthorizeUser(Module = Core.Contants.Functions.DepartmentView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Create(string record = default, string viewMode = default)
        {
            ViewBag.ViewMode = viewMode;
            DepartmentSearchResponseData model = null;
            if (!string.IsNullOrEmpty(record))
            {
                model = await _departmentRepository.GetById(record.StringAesDecryption(DepartmentConst.PublicKey, true)
                    .ToInt32());
                model.Record = record;
            }
            return PartialView(model);
        }
        [AuthorizeUser(Module = Core.Contants.Functions.DepartmentView, Permission = PermissionConstant.VIEW)]
        public async Task<IActionResult> Search(int length, int start, DepartmentSearchRequest @base)
        {
            DepartmentSearchResponse data;
            try
            {
                @base.Draw = Request.Query["draw"].ToString().ToInt32();
                @base.RequestPage = DepartmentConst.PublicKey;
                data = await _departmentRepository.SearchAsync(start, length, @base);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new DepartmentSearchResponse
                {
                    Code = (int)HttpStatusCode.NoContent,
                    Draw = @base.Draw,
                    Message = "Không tìm thấy dữ liệu yêu cầu!"
                });
            }

            return Json(data);
        }
        [AuthorizeUser(Module = Core.Contants.Functions.DepartmentView, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnCreate(DepartmentCreateRequest request)
        {
            try
            {
                request.RequestPage = DepartmentConst.PublicKey;
                request.Creator = GetUserId();
                request.CreatorName = GetUserName();
                var response = await _departmentRepository.Create(request);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new DepartmentCreateResponse
                {
                    Message = GlobalEnums.ErrorMessage,
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                });
            }
        }
        [AuthorizeUser(Module = Core.Contants.Functions.DepartmentView, Permission = PermissionConstant.EDIT)]
        public async Task<IActionResult> OffDepartment(DepartmentCreateRequest request)
        {
            try
            {
                request.RequestPage = DepartmentConst.PublicKey;
                request.Creator = GetUserId();
                request.CreatorName = GetUserName();
                request.Status = (int)GlobalEnums.StatusDefaultEnum.InActive;
                var response = await _departmentRepository.ChangeStatus(request);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new DepartmentCreateResponse
                {
                    Message = GlobalEnums.ErrorMessage,
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                });
            }
        }
    }
}
