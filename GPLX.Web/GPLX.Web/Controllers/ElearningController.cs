//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Threading.Tasks;
//using GPLX.Core.Contants;
//using GPLX.Core.Contracts.Department;
//using GPLX.Core.DTO.Request.Department;
//using GPLX.Core.DTO.Response.CostStatus;
//using GPLX.Core.DTO.Response.Department;
//using GPLX.Core.Enum;
//using GPLX.Core.Extensions;
//using GPLX.Database.Models;
//using GPLX.Infrastructure.Constants;
//using GPLX.Infrastructure.Extensions;
//using GPLX.Web.Filters;
//using GPLX.Web.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;

//namespace GPLX.Web.Controllers
//{
//    /// <summary>
//    /// GPLX
//    /// </summary>
//    public class GPLXController : BaseController
//    {
//        private readonly ILogger<DepartmentController> _logger;
//        private readonly IDepartmentRepository _departmentRepository;

//        public GPLXController(ILogger<DepartmentController> logger, IDepartmentRepository departmentRepository)
//        {
//            _logger = logger;
//            _departmentRepository = departmentRepository;
//        }

//        public IActionResult List()
//        {

//            var model = new Models.GPLX();
//            model.LstGPLX.Add(new ItemGPLX { TeacherId = 1, TeacherName = "Teacher1", StudentId = 1, StudentName = "SV1" });
//            model.LstGPLX.Add(new ItemGPLX { TeacherId = 1, TeacherName = "Teacher1", StudentId = 2, StudentName = "SV2" });
//            model.LstGPLX.Add(new ItemGPLX { TeacherId = 1, TeacherName = "Teacher1", StudentId = 3, StudentName = "SV3" });
//            model.LstGPLX.Add(new ItemGPLX { TeacherId = 2, TeacherName = "Teacher2", StudentId = 4, StudentName = "SV4" });
//            model.LstGPLX.Add(new ItemGPLX { TeacherId = 2, TeacherName = "Teacher2", StudentId = 5, StudentName = "SV5" });
//            model.LstGPLX.Add(new ItemGPLX { TeacherId = 2, TeacherName = "Teacher2", StudentId = 6, StudentName = "SV6" });
//            model.LstGPLX.Add(new ItemGPLX { TeacherId = 2, TeacherName = "Teacher2", StudentId = 7, StudentName = "SV7" });
//            return View(model);
//        }


//        public async Task<IActionResult> Create(string record = default, string viewMode = default)
//        {
//            ViewBag.ViewMode = viewMode;
//            DepartmentSearchResponseData model = null;
//            if (!string.IsNullOrEmpty(record))
//            {
//                model = await _departmentRepository.GetById(record.StringAesDecryption(DepartmentConst.PublicKey, true)
//                    .ToInt32());
//                model.Record = record;
//            }
//            return PartialView(model);
//        }

//        //public async Task<IActionResult> Search(int length, int start, DepartmentSearchRequest @base)
//        //{
//        //    DepartmentSearchResponse data;
//        //    try
//        //    {
//        //        @base.Draw = Request.Query["draw"].ToString().ToInt32();
//        //        @base.RequestPage = DepartmentConst.PublicKey;
//        //        data = await _departmentRepository.SearchAsync(start, length, @base);
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        _logger.Log(LogLevel.Error, e, e.Message);
//        //        return Json(new DepartmentSearchResponse
//        //        {
//        //            Code = (int)HttpStatusCode.NoContent,
//        //            Draw = @base.Draw,
//        //            Message = "Không tìm thấy dữ liệu yêu cầu!"
//        //        });
//        //    }

//        //    return Json(data);
//        //}

//        //public async Task<IActionResult> OnCreate(DepartmentCreateRequest request)
//        //{
//        //    try
//        //    {
//        //        request.RequestPage = DepartmentConst.PublicKey;
//        //        request.Creator = GetUserId();
//        //        request.CreatorName = GetUserName();
//        //        var response = await _departmentRepository.Create(request);
//        //        return Json(response);
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        _logger.Log(LogLevel.Error, e, e.Message);
//        //        return Json(new DepartmentCreateResponse
//        //        {
//        //            Message = GlobalEnums.ErrorMessage,
//        //            Code = (int)GlobalEnums.ResponseCodeEnum.Error
//        //        });
//        //    }
//        //}

//        //public async Task<IActionResult> OffDepartment(DepartmentCreateRequest request)
//        //{
//        //    try
//        //    {
//        //        request.RequestPage = DepartmentConst.PublicKey;
//        //        request.Creator = GetUserId();
//        //        request.CreatorName = GetUserName();
//        //        request.Status = (int)GlobalEnums.StatusDefaultEnum.InActive;
//        //        var response = await _departmentRepository.ChangeStatus(request);
//        //        return Json(response);
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        _logger.Log(LogLevel.Error, e, e.Message);
//        //        return Json(new DepartmentCreateResponse
//        //        {
//        //            Message = GlobalEnums.ErrorMessage,
//        //            Code = (int)GlobalEnums.ResponseCodeEnum.Error
//        //        });
//        //    }
//        //}
//    }
//}
