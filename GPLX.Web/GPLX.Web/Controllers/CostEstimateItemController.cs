using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Response;
using GPLX.Database.Models;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aspose.Cells;
using GPLX.Core.Contants;
using GPLX.Core.Contracts.CostEstimateItem;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Payment;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.User;
using GPLX.Core.DTO.Request.CostEstimateItem;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models.Export;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Web.Controllers
{
    public class CostEstimateItemController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICostEstimateItemRepository _costEstimateRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly ICostEstimateItemRepository _costEstimateItemRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICostEstimateItemLogsRepository _costEstimateItemLogsRepository;
        private readonly IMapper _mapper;
        private readonly ISupplierRepository _supplier;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _fileHostView;
        private readonly string _defaultRootFolder;
        private readonly string[] extends;
        private readonly IList<SelectListItem> _weeks;


        public CostEstimateItemController(ILogger<HomeController> logger,
                                          ICostEstimateItemRepository costEstimateRepository,
                                          IConfiguration configuration,
                                          ISupplierRepository supplier,
                                          IMapper mapper,
                                          IWebHostEnvironment webHostEnvironment,
                                          ICostEstimateItemLogsRepository costEstimateItemLogsRepository,
                                          ICostStatusesRepository costStatusesRepository,
                                          ICostEstimateItemRepository costEstimateItemRepository,
                                          IPaymentRepository paymentRepository,
                                          IGroupsRepository groupsRepository,
                                          IUserRepository userRepository
                                          )
        {
            _logger = logger;
            _costEstimateRepository = costEstimateRepository;
            _costStatusesRepository = costStatusesRepository;
            _costEstimateItemRepository = costEstimateItemRepository;
            _paymentRepository = paymentRepository;
            _groupsRepository = groupsRepository;
            _userRepository = userRepository;
            _costEstimateItemLogsRepository = costEstimateItemLogsRepository;
            _mapper = mapper;
            _supplier = supplier;
            _webHostEnvironment = webHostEnvironment;
            _fileHostView = configuration.GetValue<string>("FileHosting");
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");

            var weekInYear = DateTime.Now.Year.WeekInYear();
            var currentWeekInYear = weekInYear.Where(x => x.weekStart >= DateTime.Now);
            _weeks = currentWeekInYear.Select(a => new SelectListItem
            {
                Value = a.weekNum.ToString(),
                Text = $"Tuần {a.weekNum} ({a.weekStart:dd/MM/yy} - {a.weekFinish:dd/MM/yy})"
            }).ToList();

            extends = new[] { ".png", ".gif", ".jpg", ".jpeg" };
        }

        /// <summary>
        /// Tạo yêu cầu của phòng ban
        /// </summary>
        /// <param name="record"></param>
        /// <param name="partial"></param>
        /// <param name="viewMode"></param>
        /// <returns></returns>

        [AuthorizeUser(Module = Functions.CostElementItemCreate, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Create(string record = default, bool partial = default, string viewMode = default)
        {
            try
            {
                var success = Guid.TryParse(record.StringAesDecryption(CostElementItemConst.PublicKey), out Guid rawId);
                var typesSelect = new List<SelectListItem>
                {
                    new SelectListItem {
                        Text = "Chọn nhóm chi phí",
                        Selected = string.IsNullOrEmpty(record),
                        Value = ""
                    }
                };
                typesSelect.AddRange((await _paymentRepository.AllTypes(IsSubUnit() ? "sub" : GetUserUnit().UnitType)).Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }));
                var weeks = new List<SelectListItem> { new SelectListItem {
                       Text = "Chọn thời gian đề xuất",
                       Selected =  string.IsNullOrEmpty(record),
                       Value = ""
                   }
                };

                ViewBag.Type = typesSelect;

                ViewBag.ViewMode = viewMode;
                ViewBag.Key = CostElementItemConst.PublicKey;
                ViewBag.Partial = partial;
                ViewBag.PayForms = new List<SelectListItem>
                {
                    new SelectListItem { Value = GlobalEnums.PayFormOwnCapital,Text = GlobalEnums.PayFormOwnCapital },
                    new SelectListItem { Value = GlobalEnums.PayFormLoan, Text = GlobalEnums.PayFormLoan },
                    new SelectListItem { Value = "", Text = "Chọn hình thức chi", Selected = string.IsNullOrEmpty(record)}
                };

                ViewBag.Id = record;
                if (!success && !string.IsNullOrEmpty(record))
                {
                    ModelState.AddModelError(string.Empty, "Lưu thông tin không thành công, vui lòng thử lại!");
                    return View();
                }

                CostEstimateItemCreateRequest model;
                if (rawId != Guid.Empty)
                {
                    var oCostEstimateItem = await _costEstimateRepository.GetByIdAsync(rawId);
                    model = _mapper.Map<CostEstimateItem, CostEstimateItemCreateRequest>(oCostEstimateItem);
                    if (model == null)
                    {
                        ModelState.AddModelError(string.Empty, "Không tìm thấy dữ liệu yêu cầu!");
                        return View();
                    }

                    var weekInYear = DateTime.Now.Year.WeekInYear();
                    var currentWeekInYear = weekInYear.Where(x => x.weekNum >= oCostEstimateItem.PayWeek);

                    weeks.AddRange(currentWeekInYear.Select(a => new SelectListItem
                    {
                        Value = a.weekNum.ToString(),
                        Text = $"Tuần {a.weekNum} ({a.weekStart:dd/MM/yy} - {a.weekFinish:dd/MM/yy})"
                    }).ToList());

                    model.Record = record;
                    if (!string.IsNullOrEmpty(model.RequestImage))
                        model.FileView = $"{_fileHostView}/{model.RequestImage}";
                }
                else
                {
                    weeks.AddRange(_weeks);
                    model = new CostEstimateItemCreateRequest
                    {
                        UnitName = GetUserUnit().UnitName,
                        DepartmentName = GetUserDepartment().DepartmentName
                    };
                }

                ViewBag.WeekinYear = weeks;

                return View(model);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                ModelState.AddModelError(string.Empty, "Lỗi hệ thống, vui lòng thử lại sau!");
                return View();
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AuthorizeUser(Module = Functions.CostElementItemCreate, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> OnCreate([FromForm] CostEstimateItemCreateRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.BillDateCreate))
                    request.BillDate = request.BillDateCreate.ToDateTime("dd/M/yyyy", new CultureInfo("vi-VN"));
                CostEstimateItemCreateRequestValidator validator = new CostEstimateItemCreateRequestValidator();
                var exValidate = validator.Validate(request);

                var user = GetUsersSessionModel();
                var allTypes = await _paymentRepository.AllTypes(IsSubUnit() ? "sub" : GetUserUnit().UnitType);
                var allPayments = await _paymentRepository.AllPayments();

                if (string.IsNullOrEmpty(request.Record))
                {
                    request.UnitName = user.Unit.UnitName;
                    request.DepartmentName = user.Department.DepartmentName;
                }

                if (!exValidate.IsValid)
                {
                    return Json(new CostEstimateItemApprovalResponse
                    {
                        Message = exValidate.Errors[0].ErrorMessage,
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error
                    });
                }
                var rawId = Guid.Empty;
                if (!string.IsNullOrEmpty(request.Record))
                {
                    var success = Guid.TryParse(request.Record.StringAesDecryption(CostElementItemConst.PublicKey), out rawId);
                    if (!success)
                    {
                        return Json(new CostEstimateItemApprovalResponse
                        {
                            Message = "Không tìm thấy thông tin yêu cầu!",
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error
                        });
                    }
                }

                var item = _mapper.Map<CostEstimateItemCreateRequest, CostEstimateItem>(request);

                #region bổ sung các thông tin
                var costEstimateType = allTypes.FirstOrDefault(x => x.Id == request.CostEstimateItemTypeId);

                item.RequestContentNonUnicode = item.RequestContent.StringToNonUnicode();
                item.CostEstimateItemTypeName = costEstimateType?.Name;

                if (costEstimateType == null)
                    return Json(new CostEstimateItemApprovalResponse
                    {
                        Message = "Nhóm chi phí không tồn tại!",
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error
                    });

                var payment = allPayments.FirstOrDefault(x => x.Id == costEstimateType.PaymentType);
                item.CostEstimatePaymentType = payment?.Id ?? 0;
                item.CostEstimateGroupName = payment?.Name;
                item.Type = (int)GlobalEnums.StatusDefaultType.Weekly;
                item.TypeName = GlobalEnums.DefaultTypeNames[(int)GlobalEnums.StatusDefaultType.Weekly];

                if (rawId == Guid.Empty)
                {
                    item.DepartmentId = user.Department.DepartmentId;
                    item.DepartmentName = user.Department.DepartmentName;
                    item.UnitName = user.Unit.UnitName;
                    item.UnitId = user.Unit.UnitId;
                    item.CreatorId = user.UserId;
                    item.CreatorName = user.UserName;
                    item.RequesterId = user.UserId;
                    item.RequesterName = user.UserName;
                }

                item.PayWeekName = $"Tuần {item.PayWeek} ({DateTime.Now.Year.FirstDateOfWeekISO8601(item.PayWeek):dd-MM-yy} - {DateTime.Now.Year.FirstDateOfWeekISO8601(item.PayWeek).AddDays(7):dd-MM-yy})";

                if (!string.IsNullOrEmpty(request.SupplierName))
                {
                    var supplier = await _supplier.GetIdByName(request.SupplierName);
                    item.SupplierName = supplier.SupplierName;
                    item.SupplierId = supplier.Id;
                }

                #endregion
               
                #region Lưu ảnh
                if (Request.Form.Files.Count > 0)
                {
                    var billFile = Request.Form.Files[0];
                    request.Image = billFile;
                    string ownerFolder = GetUserSyncId().CreateDatePhysicalPathFileToUser();
                    string ext = Path.GetExtension(billFile.FileName);

                    if (extends.All(c => !c.Equals(ext, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        request.UnitName = user.Unit.UnitName;
                        request.DepartmentName = user.Department.DepartmentName;

                        return Json(new CostEstimateItemApprovalResponse
                        {
                            Message = "Định dạng hình ảnh chứng từ không hợp lệ!",
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error
                        });
                    }

                    var newFileName = $"{Path.GetRandomFileName()}{ext}";
                    var fileFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder, ownerFolder);

                    var file = Path.Combine(_webHostEnvironment.WebRootPath, fileFolder, newFileName);
                    if (!Directory.Exists(fileFolder))
                        Directory.CreateDirectory(fileFolder);
                    await using var fileStream = new FileStream(file, FileMode.Create);
                    await request.Image.CopyToAsync(fileStream);
                    string path = file.Replace(Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder),
                        string.Empty);
                    item.RequestImage = path;
                    item.AccountImage = path;
                }
                #endregion

                ModelState.Clear();
                if (rawId == Guid.Empty)
                {
                    request.UnitName = user.Unit.UnitName;
                    request.DepartmentName = user.Department.DepartmentName;

                    if (await _costEstimateRepository.CheckBillCodeExists(item.BillCode))
                        return Json(new CostEstimateItemApprovalResponse
                        {
                            Message = "Số hóa đơn/phiếu thu đã tồn tại!",
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error
                        });


                    var totalRequestUnit = await _costEstimateItemRepository.CreateCostEstimateItemCodeForUnit(GetUserUnit().UnitId);
                    if (totalRequestUnit == -1)
                    {
                        return Json(new CostEstimateItemApprovalResponse
                        {
                            Message = "Lỗi hệ thống, tạo mã dự trù không thành công!",
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error
                        });
                    }

                    totalRequestUnit++;
                    item.RequestCode = totalRequestUnit.CreateEstimateRequestCode(GetUserUnit().UnitCode);
                    item.CreatedDate = DateTime.Now;

                    var res = await _costEstimateRepository.CreateAsync(item);



                    //if (res.Id != Guid.Empty)
                    //    return RedirectToAction("List", new { type = GlobalEnums.DefaultTypeKeys[(int)GlobalEnums.StatusDefaultType.Weekly] });
                    return Json(new CostEstimateItemApprovalResponse
                    {
                        Message = res.Id == Guid.Empty ? "Lỗi hệ thống, vui lòng thử lại sau!" : "Lưu thông tin yêu cầu thành công!",
                        Code = res.Id == Guid.Empty ? (int)GlobalEnums.ResponseCodeEnum.Error : (int)GlobalEnums.ResponseCodeEnum.Success
                    });
                }
                else
                {
                    item.Id = rawId;
                    var res = await _costEstimateRepository.UpdateAsync(rawId, item);
                    if (res?.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                    {
                        return Json(new CostEstimateItemApprovalResponse
                        {
                            Message = "Cập nhật yêu cầu thành công!",
                            Code = (int)GlobalEnums.ResponseCodeEnum.Success
                        });
                    }

                    if (res != null)
                    {
                        return Json(new CostEstimateItemApprovalResponse
                        {
                            Message = res.Msg,
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error
                        });
                    }

                    return Json(new CostEstimateItemApprovalResponse
                    {
                        Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error
                    });
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);

                return Json(new CostEstimateItemApprovalResponse
                {
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                });
            }
        }

        // rewrite trong bundle
        // default param type
        // ví dụ:
        // ../yeu-cau/don-vi => mặc định type = unit

        /// <summary>
        /// Trang danh sách
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fOps"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostElementItemView, Permission = PermissionConstant.VIEW)]

        public IActionResult List(string type, bool fOps)
        {
            var model = new CostEstimateItemListModel();
            try
            {
                // remove hard-code default status
                model = new CostEstimateItemListModel
                {
                    Stats = GlobalEnums.DefaultStatusSearchList,
                    DefaultStats = Globs.DefaultSearchAllStats,
                    DefaultStatsName = Globs.DefaultSearchAllStatsName,
                    RequestType = type,
                    StatsFilterVisible = fOps
                };
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
            }
            return View(model);
        }

        /// <summary>
        /// Ajax tìm kiếm danh sách
        /// </summary>
        /// <param name="length"></param>
        /// <param name="start"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostElementItemView, Permission = PermissionConstant.VIEW)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(int length, int start, CostEstimateItemSearchRequest @base)
        {
            CostEstimateItemSearchResponse data;
            try
            {
                var sessionUser = GetUsersSessionModel();

                @base.KeywordsNonUnicode = @base.Keywords.StringToNonUnicode();
                @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();
                @base.PermissionDelete = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostElementItemView, PermissionConstant.DELETE);
                @base.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostElementItemView, PermissionConstant.EDIT);
                @base.PermissionApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostElementItemView, PermissionConstant.APPROVE);
                @base.IsSub = IsSubUnit();
                @base.UserUnitsManages = await _userRepository.GetUserUnitManages(sessionUser.UserId);
                @base.UserId = sessionUser.UserId;

                @base.UserUnit = GetUserUnit().UnitId;

                // nếu là >= kết toán thì set UserDepartmentId = -100
                // vì kế toán có thể nhìn thấy hết các yêu cầu đã duyệt của các phòng ban trong đơn vị
                @base.UserDepartmentId = !ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfStaffAccountant))?.Order ?? 0, ComparingMode.GatherThan)
                        ? GetUserDepartment().DepartmentId
                        : (int)GlobalEnums.StatusDefaultEnum.All;

                // nếu là cán bộ HC --> chỉ nhìn thấy của cá nhân mình
                @base.EqualUser = !ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfDepartmentStaff))?.Order ?? 0, ComparingMode.GatherThan)
                    ? sessionUser.UserId
                    : 0;

                @base.PageRequest = CostElementItemConst.PublicKey;

                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.CostStatusesElementItem,
                    CostEstimateType = @base.RequestType,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes(),
                };
                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                // Không được xem dữ liệu nào
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new CostEstimateItemSearchResponse
                    {
                        Code = (int)HttpStatusCode.Forbidden,
                        Draw = @base.Draw,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });

                @base.StatusAllowsSeen = listStatsForDepartmentGroup;

                @base.EstimateType = GlobalEnums.DefaultKeyToTypes.TryGetValue(@base.RequestType, out var i) ? i : -1;
                data = await _costEstimateRepository.SearchAsync(@base, start, length, sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new CostEstimateItemSearchResponse
                {
                    Code = (int)HttpStatusCode.NoContent,
                    Draw = @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32(),
                    Message = "Không tìm thấy dữ liệu yêu cầu!"
                });
            }

            return Json(data);
        }
        /// <summary>
        /// Xem lịch sử của yêu cầu
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostElementItemView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> ViewHistory(string record)
        {
            IList<CostEstimateItemLogResponse> data = null;
            try
            {
                var request = new ViewHistoryRequest
                {
                    CostEstimateId = record.StringAesFromParams(),
                    PageRequest = CostElementItemConst.PublicKey,
                };
                data = await _costEstimateItemLogsRepository.ViewHistories(request);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
            }
            return PartialView("_ViewHistory", data);
        }

        /// <summary>
        /// Duyệt / Từ chối yêu cầu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser(Module = Functions.CostElementItemView, Permission = PermissionConstant.APPROVE)]

        public async Task<IActionResult> Approval(CostEstimateItemApprovalRequest request)
        {
            CostEstimateItemApprovalResponse response;
            try
            {
                var sessionUser = GetUsersSessionModel();
                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.CostStatusesElementItem,
                    CostEstimateType = GlobalEnums.DefaultTypeKeys[request.Type],
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };

                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new CostEstimateItemApprovalResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });

                request.StatusAllowsSeen = listStatsForDepartmentGroup;

                request.PageRequest = CostElementItemConst.PublicKey;
                request.Positions = sessionUser.Positions;
                request.UserId = sessionUser.UserId;
                request.UserName = sessionUser.UserName;
                request.UnitId = sessionUser.Unit.UnitId;
                request.IsSub = IsSubUnit();
                request.UnitType = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut;
                request.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostElementItemView, PermissionConstant.EDIT);

                response = await _costEstimateRepository.ApprovalOrDecline(request);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response = new CostEstimateItemApprovalResponse
                {
                    Message = "Lỗi hê thống, vui lòng thử lại sau!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                };
            }
            return Json(response);
        }

        /// <summary>
        /// Hàm chung dùng để tải các file mẫu
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="s"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadTemplate(string opts, string s, int year)
        {
            try
            {
                year = year == 0 ? DateTime.Now.Year + 1 : year;
                var sessionUser = GetUsersSessionModel();
                if (!string.IsNullOrEmpty(opts))
                {
                    var sepFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();
                    bool isSpecialUnit = sepFollowConfigs.Any(c =>
                        c.UnitCode.Equals(sessionUser.Unit.UnitCode, StringComparison.CurrentCultureIgnoreCase));

                    string defaultPosition = "TỔNG GIÁM ĐỐC";
                    if (isSpecialUnit)
                        defaultPosition = "PHÓ TỔNG GIÁM ĐỐC";

                    string template;
                    opts = opts.ToLower();
                    switch (opts)
                    {
                        #region Kế toán

                        case "xlsx;kt":
                            template = @"./Resources/KTExport-Template.xlsx";
                            Workbook workbook = new Workbook(template);
                            WorkbookDesigner designer = new WorkbookDesigner(workbook);
                            var ws = workbook.Worksheets[0];
                            var paymentForms = new List<PayFormExport>
                            {
                                new PayFormExport{Number = 1,Value = GlobalEnums.PayFormOwnCapital},
                                new PayFormExport{Number = 2,Value = GlobalEnums.PayFormLoan}
                            };

                            var allPayments = (await _paymentRepository.AllPayments()).OrderBy(x => x.ComparingType).ToList();
                            var allTypes = await _paymentRepository.AllTypes(IsSubUnit() ? "sub" : GetUserUnit().UnitType);

                            var paymentExports = new List<PaymentExport>();
                            foreach (var costEstimateItemType in allTypes)
                            {
                                paymentExports.Add(new PaymentExport
                                {
                                    CostEstimateTypeName = costEstimateItemType.Name,
                                    PaymentName = allPayments.FirstOrDefault(x => x.Id == costEstimateItemType.PaymentType)?.Name,
                                    ComparingType = costEstimateItemType.ComparingType
                                });
                            }

                            CellArea ca = CellArea.CreateCellArea("L", "L");
                            workbook.Worksheets[1].Protect(ProtectionType.All, "MED@2021", null);
                            workbook.Worksheets[2].Protect(ProtectionType.All, "MED@2021", null);
                            Style style;

                            // Define the styleflag object
                            StyleFlag styleflag;

                            // Loop through all the columns in the worksheet and unlock them.
                            for (int i = 0; i <= 255; i++)
                            {

                                style = ws.Cells.Columns[(byte)i].Style;
                                style.IsLocked = false;
                                styleflag = new StyleFlag { Locked = true };
                                ws.Cells.Columns[(byte)i].ApplyStyle(style, styleflag);
                            }
                            ValidationCollection validations = workbook.Worksheets[0].Validations;

                            Validation validation = validations[validations.Add(ca)];
                            var formulaM2 = ws.Cells["M2"].GetFormula(false, false);
                            for (int i = 2; i < 1000; i++)
                            {
                                string fml = formulaM2.Replace("L2", $"L{i}");
                                ws.Cells[$"M{i}"].SetFormula(fml, null);
                            }

                            // Define the styleflag object.
                            StyleFlag flag;

                            // Loop through all the columns in the worksheet and unlock them.
                            for (int i = 0; i <= 255; i++)
                            {
                                style = ws.Cells.Columns[(byte)i].Style;
                                style.IsLocked = false;
                                flag = new StyleFlag { Locked = true };
                                ws.Cells.Columns[(byte)i].ApplyStyle(style, flag);

                            }
                            int colIndexProtect = CellsHelper.ColumnNameToIndex("M");

                            // Get the first column style.
                            style = ws.Cells.Columns[colIndexProtect].Style;

                            // Lock it.
                            style.IsLocked = true;

                            // Instantiate the flag.
                            flag = new StyleFlag();

                            // Set the lock setting.
                            flag.Locked = true;
                            // Apply the style to the column.
                            ws.Cells.Columns[colIndexProtect].ApplyStyle(style, flag);
                            ws.Protect(ProtectionType.All);

                            validation.Type = ValidationType.List;
                            validation.IgnoreBlank = false;
                            validation.Formula1 = $"=FormulatedData!$C$2:$C${allTypes.Count + 1}";
                            validation.Operator = OperatorType.None;


                            validation.AddArea(ca);

                            designer.SetDataSource("FormulatedData", paymentExports);
                            designer.SetDataSource("Payments", allPayments);
                            designer.SetDataSource("PayFormExport", paymentForms);
                            workbook.CalculateFormula(true);
                            designer.Process();
                            MemoryStream mem = new MemoryStream();

                            workbook.Save(mem, SaveFormat.Xlsx);
                            mem.Position = 0;
                            return File(mem, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                $"BM_Ketoan_{DateTime.Now:ddMMyyyy}.xlsx");
                        #endregion

                        #region Thực chi
                        case "xlsx;actually":
                            template = @"./Resources/KTExport-Actually-Template.xlsx";
                            Workbook workbookAct = new Workbook(template);
                            WorkbookDesigner designerActually = new WorkbookDesigner(workbookAct);
                            designerActually.SetDataSource("Year", $"Năm {DateTime.Now.Year}");
                            designerActually.SetDataSource("UnitName", GetUserUnit().UnitName);
                            designerActually.Process();
                            MemoryStream memAct = new MemoryStream();

                            workbookAct.Save(memAct, SaveFormat.Xlsx);
                            memAct.Position = 0;
                            return File(memAct, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                $"BM_Thuc_chi_{DateTime.Now:ddMMyyyy}.xlsx");
                        #endregion

                        #region Dòng tiền
                        case "xlsx;cash-follow":
                            template = $@"./Resources/{s}/CashFollow-Template.xlsx";
                            Workbook workbookCashFollow = new Workbook(template);
                            MemoryStream memCashFollow = new MemoryStream();
                            WorkbookDesigner designerCashFollow = new WorkbookDesigner(workbookCashFollow);
                            designerCashFollow.SetDataSource("Year", year);
                            designerCashFollow.SetDataSource("UnitName", GetUserUnit().UnitName);
                            designerCashFollow.SetDataSource("Position", defaultPosition);


                            var allPaymentCashFollow = await _paymentRepository.AllPayments();
                            var allTypeCashFollow = await _paymentRepository.AllTypes(IsSubUnit() ? "sub" : GetUserUnit().UnitType);
                            var finance = allPaymentCashFollow.Where(x =>
                                x.Name.Equals("Chi hoạt động thường quy", StringComparison.CurrentCultureIgnoreCase)
                                || x.Name.Equals("Chi hoạt động không thường quy",
                                    StringComparison.CurrentCultureIgnoreCase)).ToList();
                            var listFinance = new List<CostEstimateItemType>();
                            var listInvestment = new List<CostEstimateItemType>();
                            if (finance.Any())
                                listFinance = allTypeCashFollow.Where(x => finance.Any(p => p.Id == x.PaymentType))
                                    .ToList();

                            var investment = allPaymentCashFollow.Where(x =>
                                x.Name.Equals("Chi hoạt động đầu tư", StringComparison.CurrentCultureIgnoreCase)).ToList();
                            if (finance.Any())
                                listInvestment = allTypeCashFollow.Where(x => investment.Any(p => p.Id == x.PaymentType))
                                    .ToList();

                            designerCashFollow.SetDataSource("Finals", listFinance);
                            designerCashFollow.SetDataSource("Investment", listInvestment);

                            designerCashFollow.Process();

                            workbookCashFollow.Save(memCashFollow, SaveFormat.Xlsx);
                            memCashFollow.Position = 0;
                            return File(memCashFollow, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                $"BM_KeHoachDongTien_{(s.Equals("in") ? "DV_Yte" : s.Equals("sub") ? "SUB" : "DV_NgoaiYTe")}_{DateTime.Now:ddMMyyyy}.xlsx");
                        #endregion

                        #region BM Đầu tư
                        case "xlsx;investment-plan":
                            if (isSpecialUnit) s = "spec";
                            template = $@"./Resources/{s}/Investment-Template.xlsx";

                            Workbook wbInvestmentIn = new Workbook(template);
                            MemoryStream memInvestmentIn = new MemoryStream();
                            WorkbookDesigner designerInvestmentIn = new WorkbookDesigner(wbInvestmentIn);
                            designerInvestmentIn.SetDataSource("Year", year);
                            designerInvestmentIn.SetDataSource("UnitName", GetUserUnit().UnitName);
                            designerInvestmentIn.SetDataSource("Position", defaultPosition);

                            designerInvestmentIn.Process();
                            wbInvestmentIn.Save(memInvestmentIn, SaveFormat.Xlsx);
                            memInvestmentIn.Position = 0;
                            return File(memInvestmentIn, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                $"BM_KeHoachDautu_{(s.Equals("in") ? "DV_Yte" : s.Equals("sub") ? "SUB" : "DV_NgoaiYTe")}_{DateTime.Now:ddMMyyyy}.xlsx");

                        #endregion

                        #region BM Doanh thu KH
                        case "xlsx;revenue-plan":
                            template = $@"./Resources/{s}/Revenue-Template.xlsx";

                            Workbook wbRevenueIn = new Workbook(template);
                            MemoryStream memRevenueIn = new MemoryStream();
                            WorkbookDesigner designerRevenueIn = new WorkbookDesigner(wbRevenueIn);
                            designerRevenueIn.SetDataSource("Year", year);
                            designerRevenueIn.SetDataSource("UnitName", GetUserUnit().UnitName);
                            designerRevenueIn.SetDataSource("Position", defaultPosition);
                            designerRevenueIn.Process();
                            wbRevenueIn.Save(memRevenueIn, SaveFormat.Xlsx);
                            memRevenueIn.Position = 0;
                            return File(memRevenueIn, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                $"BM_KeHoachDoanhThu_{(s.Equals("in") ? "DV_Yte" : s.Equals("sub") ? "SUB" : "DV_NgoaiYTe")}_{DateTime.Now:ddMMyyyy}.xlsx");

                        #endregion

                        #region BM Lợi nhuận
                        case "xlsx;profit-plan":
                            template = $@"./Resources/{s}/Profit-Template.xlsx";

                            Workbook wbProfit = new Workbook(template);
                            MemoryStream memProfit = new MemoryStream();
                            WorkbookDesigner designerProfit = new WorkbookDesigner(wbProfit);
                            designerProfit.SetDataSource("Year", year);
                            designerProfit.SetDataSource("UnitName", GetUserUnit().UnitName);
                            designerProfit.SetDataSource("Position", defaultPosition);
                            designerProfit.Process();
                            wbProfit.Save(memProfit, SaveFormat.Xlsx);
                            memProfit.Position = 0;
                            return File(memProfit, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                $"BM_KeHoachLoiNhuan_{(s.Equals("in") ? "DV_Yte" : s.Equals("sub") ? "SUB" : "DV_NgoaiYTe")}_{DateTime.Now:ddMMyyyy}.xlsx");

                        #endregion

                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }
            return new EmptyResult();
        }
        [AuthorizeUser(Module = Functions.CostElementItemView, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> Delete(CostEstimateItemApprovalRequest request)
        {
            CostEstimateItemApprovalResponse response;
            try
            {
                var sessionUser = GetUsersSessionModel();
                request.PageRequest = CostElementItemConst.PublicKey;
                request.StatusChange = request.IsApproval ? (int)GlobalEnums.StatusDefaultEnum.Active : (int)GlobalEnums.StatusDefaultEnum.Decline;
                request.StatusChangeName = GlobalEnums.DefaultStatusNames[request.StatusChange];
                request.Positions = GetUserPosition();
                request.UserId = sessionUser.UserId;
                request.UserName = sessionUser.UserName;
                request.UnitId = sessionUser.Unit.UnitId;
                response = await _costEstimateItemRepository.Delete(request);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response = new CostEstimateItemApprovalResponse
                {
                    Message = "Lỗi hê thống, vui lòng thử lại sau!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                };
            }
            return Json(response);
        }

        public async Task<IActionResult> SupplierSuggestion(string search)
        {
            IList<Suppliers> response;
            try
            {
                response = await _supplier.Searching(search);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response = new List<Suppliers>();
            }
            return Json(response);
        }
    }
}
