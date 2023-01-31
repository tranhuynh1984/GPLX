using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.DTO.Request.CostStatus;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.DTO.Response.Department;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database.Models;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models.CostStatus;
using Microsoft.Extensions.Logging;

namespace GPLX.Web.Controllers
{
    public class CostStatusesController : BaseController
    {
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly ILogger<CostStatusesController> _logger;
        private readonly IGroupsRepository _groupsRepository;

        public CostStatusesController(ICostStatusesRepository costStatusesRepository, ILogger<CostStatusesController> logger, IGroupsRepository groupsRepository)
        {
            _costStatusesRepository = costStatusesRepository;
            _logger = logger;
            _groupsRepository = groupsRepository;
        }

        [AuthorizeUser(Module = Core.Contants.Functions.CostStatusesView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> List()
        {
            var model = new CostStatusListModel
            {
                Stats = new List<KeyPair>
                {
                    new KeyPair {Value = ((int) GlobalEnums.StatusDefaultEnum.Active).ToString(),Text = GlobalEnums.OtherStatusNames[(int) GlobalEnums.StatusDefaultEnum.Active]},
                    new KeyPair {Value = ((int) GlobalEnums.StatusDefaultEnum.InActive).ToString(),Text = GlobalEnums.OtherStatusNames[(int) GlobalEnums.StatusDefaultEnum.InActive]},
                    new KeyPair {Value = ((int) GlobalEnums.StatusDefaultEnum.All).ToString(),Text = GlobalEnums.OtherStatusNames[(int) GlobalEnums.StatusDefaultEnum.All]},
                },
                StatusForSubject = new List<KeyPair>
                {
                    new KeyPair{Text = "Chọn nhóm đơn vị", Value = ""},
                    new KeyPair{Text = GlobalEnums.ObjectNames[GlobalEnums.ObjectUnit], Value = GlobalEnums.ObjectUnit},
                    new KeyPair{Text = GlobalEnums.ObjectNames[GlobalEnums.ObjectSub], Value = GlobalEnums.ObjectSub},
                    new KeyPair{Text = GlobalEnums.UnitTypeNames[GlobalEnums.UnitIn], Value = GlobalEnums.UnitIn},
                    new KeyPair{Text = GlobalEnums.UnitTypeNames[GlobalEnums.UnitOut], Value = GlobalEnums.UnitOut}
                },
                StatusForCostEstimateType = new List<KeyPair>
                {
                    new KeyPair{Text = "Chọn nhóm thời gian", Value = ""},
                    new KeyPair{Text = GlobalEnums.CostEstimateTypeNames[GlobalEnums.Week], Value = GlobalEnums.Week},
                    new KeyPair{Text = GlobalEnums.CostEstimateTypeNames[GlobalEnums.Year], Value = GlobalEnums.Year},
                },
                Types = new List<KeyPair>
                {
                    new KeyPair { Text = "Chọn nhóm dữ liệu", Value = "" },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.CostStatusesElementItem], Value = GlobalEnums.CostStatusesElementItem },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.CostStatusesElement], Value = GlobalEnums.CostStatusesElement },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.ActuallySpent], Value = GlobalEnums.ActuallySpent },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.Investment], Value = GlobalEnums.Investment },

                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.Revenue], Value = GlobalEnums.Revenue },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.Profit], Value = GlobalEnums.Profit },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.CashFollow], Value = GlobalEnums.CashFollow }
                }
            };

            var listSpecial = await _costStatusesRepository.GetSpecialUnitFollowConfigs();
            foreach (var specialUnitFollowConfig in listSpecial)
            {
                model.StatusForSubject.Add(new KeyPair
                {
                    Text = specialUnitFollowConfig.UnitShortName,
                    Value = specialUnitFollowConfig.UnitCode
                });

            }

            return View(model);
        }

        public async Task<IActionResult> Create(string record = default, string viewMode = default)
        {
            var dropdownData = new CostStatusListModel
            {
                Stats = new List<KeyPair>
                {
                    new KeyPair {Value = ((int) GlobalEnums.StatusDefaultEnum.Active).ToString(),Text = GlobalEnums.OtherStatusNames[(int) GlobalEnums.StatusDefaultEnum.Active]},
                    new KeyPair {Value = ((int) GlobalEnums.StatusDefaultEnum.InActive).ToString(),Text = GlobalEnums.OtherStatusNames[(int) GlobalEnums.StatusDefaultEnum.InActive]},
                    new KeyPair {Value = ((int) GlobalEnums.StatusDefaultEnum.All).ToString(),Text = GlobalEnums.OtherStatusNames[(int) GlobalEnums.StatusDefaultEnum.All]},
                },
                StatusForSubject = new List<KeyPair>
                {
                    new KeyPair{Text = "Chọn nhóm đơn vị", Value = ""},
                    new KeyPair{Text = GlobalEnums.ObjectNames[GlobalEnums.ObjectUnit], Value = GlobalEnums.ObjectUnit},
                    new KeyPair{Text = GlobalEnums.ObjectNames[GlobalEnums.ObjectSub], Value = GlobalEnums.ObjectSub},
                    new KeyPair{Text = GlobalEnums.UnitTypeNames[GlobalEnums.UnitIn], Value = GlobalEnums.UnitIn},
                    new KeyPair{Text = GlobalEnums.UnitTypeNames[GlobalEnums.UnitOut], Value = GlobalEnums.UnitOut}
                },
                StatusForCostEstimateType = new List<KeyPair>
                {
                    new KeyPair{Text = "Chọn nhóm thời gian", Value = ""},
                    new KeyPair{Text = GlobalEnums.CostEstimateTypeNames[GlobalEnums.Week], Value = GlobalEnums.Week},
                    new KeyPair{Text = GlobalEnums.CostEstimateTypeNames[GlobalEnums.Year], Value = GlobalEnums.Year},
                },
                Types = new List<KeyPair>
                {
                    new KeyPair { Text = "Chọn nhóm dữ liệu", Value = "" },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.CostStatusesElementItem], Value = GlobalEnums.CostStatusesElementItem },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.CostStatusesElement], Value = GlobalEnums.CostStatusesElement },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.ActuallySpent], Value = GlobalEnums.ActuallySpent },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.Investment], Value = GlobalEnums.Investment },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.Revenue], Value = GlobalEnums.Revenue },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.Profit], Value = GlobalEnums.Profit },
                    new KeyPair { Text = GlobalEnums.TypeNames[GlobalEnums.CashFollow], Value = GlobalEnums.CashFollow }
                }
            };
           
            var listSpecial = await _costStatusesRepository.GetSpecialUnitFollowConfigs();

            foreach (var specialUnitFollowConfig in listSpecial)
            {
                dropdownData.StatusForSubject.Add(new KeyPair
                {
                    Text = specialUnitFollowConfig.UnitShortName,
                    Value = specialUnitFollowConfig.UnitCode
                });

            }

            CostStatuses model = null;
            if (!string.IsNullOrEmpty(record))
            {
                var rawId = Guid.TryParse(record.StringAesDecryption(StatusesConst.PublicKey, true), out var g) ? g : Guid.Empty;
                if (rawId != Guid.Empty)
                    model = await _costStatusesRepository.GetById(rawId);
                ViewBag.RawId = record;
            }
            ViewBag.ViewMode = viewMode;
            ViewBag.DropdownData = dropdownData;

            return View(model);
        }
        [AuthorizeUser(Module = Core.Contants.Functions.CostStatusesView, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> Grant(string record)
        {
            IList<Groups> oldData = null;
            if (!string.IsNullOrEmpty(record))
            {
                var rawId = Guid.TryParse(record.StringAesDecryption(StatusesConst.PublicKey, true), out var g) ? g : Guid.Empty;
                if (rawId != Guid.Empty)
                {
                    var oRecord = await _costStatusesRepository.GetById(rawId);
                    ViewBag.Name = oRecord.Name;
                    oldData = await _costStatusesRepository.GetGrantStatuses(rawId);
                    ViewBag.Record = record;
                    ViewBag.Used = await _costStatusesRepository.GetUsedByGroup(rawId);
                }
            }

            ViewBag.Granted = oldData;
            return PartialView((await _groupsRepository.GetAllActiveGroups()));
        }

        public async Task<IActionResult> OnGrant(CostStatusesGrantRequest request)
        {
            request.RequestPage = StatusesConst.PublicKey;
            var response = await _costStatusesRepository.SetGrant(request);
            return Json(response);
        }
        [AuthorizeUser(Module = Core.Contants.Functions.CostStatusesView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, CostStatusSearchRequest @base)
        {
            CostStatusSearchResponse data;
            try
            {
                @base.Draw = Request.Query["draw"].ToString().ToInt32();
                @base.RequestPage = StatusesConst.PublicKey;
                data = await _costStatusesRepository.Search(start, length, @base);
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
        [AuthorizeUser(Module = Core.Contants.Functions.CostStatusesView, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> OnCreate(CostStatusCreateRequest request)
        {
            try
            {
                request.RequestPage = StatusesConst.PublicKey;
                request.Creator = GetUserId();
                request.CreatorName = GetUserName();
                var response = await _costStatusesRepository.AddAsync(request);
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


        public async Task<IActionResult> FollowStep()
        {
            var getFollow = await _costStatusesRepository.GetFollow(new GetFollowRequest
            {
                StatusForSubject = "unit",
                StatusForCostEstimateType = "week",
                Type = "estimate"
            });
            return View(getFollow);
        }
        [AuthorizeUser(Module = Core.Contants.Functions.CostStatusesView, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> ChangeStats(CostStatusCreateRequest request)
        {
            try
            {
                request.RequestPage = StatusesConst.PublicKey;
                request.Creator = GetUserId();
                request.CreatorName = GetUserName();
                var response = await _costStatusesRepository.ChangeStatus(request);
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
