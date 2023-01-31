using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.Unit;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.Unit;
using GPLX.Core.Enum;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GPLX.Web.Controllers
{
    public class UnitController : BaseController
    {
        private readonly IUnitRepository _unitRepository;
        private readonly ILogger<UnitController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public UnitController(IUnitRepository unitRepository, ILogger<UnitController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _unitRepository = unitRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
        }
        [AuthorizeUser(Module = Functions.UnitView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, UnitSearchRequest @base)
        {
            UnitSearchResponse data;
            try
            {
                @base.Draw = Request.Query["draw"].ToString().ToInt32();
                @base.RequestPage = DepartmentConst.PublicKey;
                data = await _unitRepository.Search(start, length, @base);
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
        [AuthorizeUser(Module = Functions.UnitView, Permission = PermissionConstant.VIEW)]

        public IActionResult List()
        {
            return View();
        }
        [AuthorizeUser(Module = Functions.UnitView, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> OnSync()
        {
            var response = new EmptySearchResponseModel();
            try
            {
                var token = await _authenticateConnect.GetAccessTokenAsync(_configuration.GetSection("Authentication:AccessToken").Value, new MedAccesstokenRequest
                {
                    grant_type = _configuration.GetSection("AuthenSecret:grant_type").Value,
                    client_id = _configuration.GetSection("AuthenSecret:client_id").Value,
                    scope = _configuration.GetSection("AuthenSecret:scope").Value,
                    client_secret = _configuration.GetSection("AuthenSecret:client_secret").Value
                });

                var requestCall = await _authenticateConnect.GetUnitsAsync(_configuration.GetSection("Authentication:Unit").Value, token);
                if (requestCall.code == 999)
                {
                    var syncOk = await _unitRepository.AddRangeAsync(requestCall.items);
                    if (!syncOk)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = "Đồng bộ dữ liệu đơn vị không thành công!";
                    }
                    else
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Đồng bộ dữ liệu đơn vị thành công!";
                    }
                }
                else
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = "Đồng bộ dữ liệu đơn vị không thành công!";
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }
            return Json(response);
        }
        [AuthorizeUser(Module = Functions.UnitView, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> SetInOutView(string code)
        {
            var ops = new List<SelectListItem>
            {
                new SelectListItem{Text = "Chọn nhóm đơn vị",Value = ""},
                new SelectListItem{Text = GlobalEnums.UnitTypeNames[GlobalEnums.UnitIn],Value = GlobalEnums.UnitIn},
                new SelectListItem{Text = GlobalEnums.UnitTypeNames[GlobalEnums.UnitOut],Value = GlobalEnums.UnitOut}
                //Thêm option sub
            };
            if (!string.IsNullOrEmpty(code))
            {
                ViewBag.UnitType = await _unitRepository.GetUnitType(code);
                ViewBag.Code = code;
            }
            return PartialView("_SetTypeView", ops);
        }
        [AuthorizeUser(Module = Functions.UnitView, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> SetInOut(UnitSetTypeRequest request)
        {
            var response = new UnitSetTypeResponse();
            try
            {
                response = await _unitRepository.SetInOut(request);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }

            return Json(response);
        }


    }
}
