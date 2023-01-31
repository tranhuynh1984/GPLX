using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aspose.Cells;
using GPLX.Core.Contants;
using GPLX.Core.Contracts.Actually;
using GPLX.Core.Contracts.CostEstimateItem;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Payment;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Core.DTO.Entities;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.Actually;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.Actually;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.Enum;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ActuallySpent = GPLX.Infrastructure.Constants.ActuallySpent;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Web.Controllers
{
    public class ActuallySpentController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ActuallySpentController> _logger;
        private readonly ICostEstimateItemRepository _costEstimateItemRepository;
        private readonly IActuallySpentRepository _actuallySpentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly Regex _rgxMatchRequestCodeOnContent;
        private readonly string _defaultRootFolder;


        public ActuallySpentController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, ILogger<ActuallySpentController> logger, ICostEstimateItemRepository costEstimateItemRepository, IActuallySpentRepository actuallySpentRepository, IPaymentRepository paymentRepository, IGroupsRepository groupsRepository, ICostStatusesRepository costStatusesRepository, IUserRepository userRepository, IUnitRepository unitRepository)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _costEstimateItemRepository = costEstimateItemRepository;
            _actuallySpentRepository = actuallySpentRepository;
            _paymentRepository = paymentRepository;
            _groupsRepository = groupsRepository;
            _costStatusesRepository = costStatusesRepository;
            _userRepository = userRepository;
            _unitRepository = unitRepository;
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");
            _rgxMatchRequestCodeOnContent = new Regex("(^[_.]+)([a-zA-Z0-9-]+[_])", RegexOptions.IgnoreCase);
        }
        [AuthorizeUser(Module = Functions.ActuallySpentView, Permission = PermissionConstant.VIEW)]

        public IActionResult List()
        {
            var model = new ActuallyListModel
            {
                Stats = GlobalEnums.DefaultStatusSearchList,
                DefaultStats = Globs.DefaultSearchAllStats,
                DefaultStatsName = Globs.DefaultSearchAllStatsName,
            };

            return View(model);
        }
        [AuthorizeUser(Module = Functions.ActuallySpentView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, SearchActuallySpentRequest @base)
        {
            var response = new SearchActuallySpentResponse();
            try
            {
                var sessionUser = GetUsersSessionModel();
                @base.KeywordsNonUnicode = @base.Keywords.StringToNonUnicode();
                @base.Draw = Request.Query["draw"].ToString().ToInt32();
                @base.PageRequest = ActuallySpent.PublicKey;

                // nếu lớn hơn giám đốc đơn vị
                // có thể nhìn thấy hết các request của các đơn vị
                @base.UserUnit =
                    !ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0)
                        ? GetUserUnit().UnitId
                        : (int)GlobalEnums.StatusDefaultEnum.All;


                @base.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.ActuallySpentView, PermissionConstant.EDIT);

                @base.PermissionApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.ActuallySpentView, PermissionConstant.APPROVE);
                @base.IsSub = IsSubUnit();
                @base.UserUnitsManages = await _userRepository.GetUserUnitManages(sessionUser.UserId);


                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.ActuallySpent,
                    CostEstimateType = GlobalEnums.Week,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
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
                response = await _actuallySpentRepository.SearchAsync(@base, start, length, sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Draw = Request.Query["draw"].ToString().ToInt32();
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
            }

            return Json(response);
        }
        [AuthorizeUser(Module = Functions.ActuallySpentCreate, Permission = PermissionConstant.VIEW)]
        public async Task<IActionResult> Create(string record = default, string viewMode = default, bool partial = default)
        {

            var model = new ActuallyCreateModel
            {
                EnableCreate = viewMode != "view",
                DataView = null,
                Record = record,
                Partial = partial,
                Type = await _paymentRepository.AllTypes(IsSubUnit() ? "sub" : GetUserUnit().UnitType)
            };

            if (!string.IsNullOrEmpty(record))
            {
                var request = new GetActuallySpentByIdRequest
                {
                    Record = record,
                    PageRequest = ActuallySpent.PublicKey,
                    UnitId = !ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0)
                        ? GetUserUnit().UnitId
                        : (int)GlobalEnums.StatusDefaultEnum.All
                };
                var response = await _actuallySpentRepository.GetByIdAsync(request);
                if (response == null)
                {
                    model.IsError = true;
                    model.Message = "Không tìm thấy dữ liệu yêu cầu!";
                }
                else
                {
                    model.DataView = response.Data;
                    model.EnableCreate = (viewMode != "view" && !string.IsNullOrEmpty(viewMode));
                    model.ReportForWeek = response.ReportForWeek;
                    model.ReportForWeekName = response.ReportForWeekName;
                }
            }

            return View(model);
        }

        /// <summary>
        /// Đọc & xử lý dữ liệu từ sheet dữ liệu BM 111 và 112
        /// </summary>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.ActuallySpentCreate, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnSCTDataUpload()
        {
            var excelUploadResponse = new ExcelUploadResponse<SCTView>();

            try
            {
                var fileOnRequest = Request.Form.Files;
                if (fileOnRequest.Count == 0)
                {
                    excelUploadResponse.AddError(new ExcelValidatorError { Message = "Không có tệp nào được tải lên" });
                    excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                }
                else
                {
                    var sheets = SctDataConst.FileImportSheetNamesRequire;

                    var excelFile = fileOnRequest[0];
                    string randomFolderName = "temporary";
                    var fileCreateName = $"{Path.GetRandomFileName()}{Path.GetExtension(excelFile.FileName)}";
                    var folder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder, randomFolderName);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    var file = Path.Combine(folder, fileCreateName);

                    await using var fileStream = new FileStream(file, FileMode.Create);
                    await excelFile.CopyToAsync(fileStream);
                    await fileStream.DisposeAsync();
                    fileStream.Close();

                    var workBook = new Workbook(file);

                    var sheetNamesMatch = workBook.Worksheets.Cast<Worksheet>().Select(x =>
                        x.IsVisible && sheets.Any(m => x.Name.Contains(m, StringComparison.OrdinalIgnoreCase)));

                    if (sheets.Length > sheetNamesMatch.Count(x => x))
                    {
                        excelUploadResponse.AddError(new ExcelValidatorError { Message = "Không có sheet dữ liệu yêu cầu!" });
                        excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        return Json(excelUploadResponse);
                    }
                    foreach (var sheetName in SctDataConst.FileImportSheetNamesRequire)
                    {
                        IList<ExcelRow> excelRows = new List<ExcelRow>();
                        var numberCode = new Regex("[0-9]+");

                        var ws = workBook.Worksheets.Cast<Worksheet>().First(x => x.Name.Contains(sheetName) && x.IsVisible);
                        var excelValidator = new ExcelFormValidation(_configuration);
                        excelValidator = excelValidator.Load($"ExcelFormValidator:SctData_{numberCode.Match(sheetName).Value}");

                        bool matchHeader = false;
                        int rowCounter = -1;
                        foreach (Row cellsRow in ws.Cells.Rows)
                        {
                            if (cellsRow.IsHidden)
                                continue;
                            var fCell = cellsRow.FirstCell.Row;
                            var cellA = cellsRow.GetCellOrNull(0);
                            var cellC = cellsRow.GetCellOrNull(2);
                            // khi nào gặp cell này thì mới đọc dữ liệu
                            if (cellA?.StringValue.Contains("Chứng từ", StringComparison.OrdinalIgnoreCase) == true)
                            {
                                matchHeader = true;
                            }
                            if(!matchHeader)
                                continue;

                            rowCounter++;

                            if (rowCounter <= 2)
                                continue;
                            if(cellC?.StringValue.Equals("Tổng phát sinh") == true)
                                break;

                            var cellsInRow = new List<ExcelCell>();
                            for (int j = 0; j <= excelValidator.ColumnConfigs.Max(x => x.Position); j++)
                            {
                                var dataAt = cellsRow[j];
                                var xCellColumnValidator =
                                    excelValidator.ColumnConfigs.FirstOrDefault(x => x.Position == j);

                                if (xCellColumnValidator != null)
                                {
                                    var cell = excelValidator.ReadCellAt(xCellColumnValidator, dataAt);
                                    cell.CellPosition = j;
                                    cellsInRow.Add(cell);
                                }
                            }

                            string cellContent = cellsInRow.FirstOrDefault(x => x.CellPosition == 1)?.StringCellValue;
                            if (!string.IsNullOrEmpty(cellContent))
                                excelRows.Add(new ExcelRow { Cells = cellsInRow, RowIndex = fCell });
                           
                        }
                        Type oType = typeof(SCTView);
                        RowCellsValidate rcValidate = new RowCellsValidate
                        {
                            AllRows = excelRows
                        };
                        bool isValid = rcValidate.ValidateRows();
                        if (isValid && rcValidate.AllRows.All(c => c.Cells.All(m => !m.IsNotValidCell)))
                        {
                            Type oCellType = typeof(SCTView);

                            foreach (var excelRow in excelRows)
                            {
                                var instanceOf = Activator.CreateInstance(oType);
                                foreach (var excelRowCell in excelRow.Cells)
                                {
                                    if (!string.IsNullOrEmpty(excelRowCell.FieldMapper) && excelRowCell.ReaderVal != null)
                                    {
                                        var field = oCellType.GetProperty(excelRowCell.FieldMapper, BindingFlags.Instance | BindingFlags.Public);
                                        if (field != null)
                                        {
                                            Type t = Nullable.GetUnderlyingType(field.PropertyType) ?? field.PropertyType;
                                            object sfVal = excelRowCell.ReaderVal;
                                            switch (excelRowCell.ReaderVal)
                                            {
                                                case double d:
                                                    sfVal = Math.Round(d, MidpointRounding.AwayFromZero);
                                                    break;
                                            }
                                            object safeValue = Convert.ChangeType(sfVal, t);

                                            field.SetValue(instanceOf, safeValue);
                                        }
                                    }
                                }

                                var oExcelRow = (SCTView)instanceOf;
                                oExcelRow.Type = numberCode.Match(sheetName).Value;
                                oExcelRow.FileName = fileCreateName;
                                oExcelRow.Path = Path.Combine(_defaultRootFolder, randomFolderName, fileCreateName);
                                oExcelRow.RequestCode = _rgxMatchRequestCodeOnContent.IsMatch(oExcelRow.RequestContent)
                                    ? _rgxMatchRequestCodeOnContent.Match(oExcelRow.RequestContent).Groups[2].Value.TrimEnd('_')
                                    : string.Empty;
                                excelUploadResponse.Add(oExcelRow);
                            }
                        }
                        else
                        {
                            string errorFile = rcValidate.CreateErrorFile(workBook, folder, excelFile.FileName);
                            if (string.IsNullOrEmpty(errorFile))
                            {
                                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                excelUploadResponse.Message = GlobalEnums.ErrorMessage;
                                return Json(excelUploadResponse);
                            }
                        }
                    }

                    if (excelUploadResponse.IsValid)
                    {
                        // dự chi chỉ lấy các row
                        // có phát sinh 'Có' > 0
                        var listActuallyData = excelUploadResponse.Data.Where(x => x.IncurredHave > 0).ToList();
                        // Danh sách các thực chi có mã dự trù
                        var listInCostEstimate = listActuallyData.Where(x => !string.IsNullOrEmpty(x.RequestCode)).ToList();
                        var listIDsIn = listInCostEstimate.Select(x => x.RequestCode).ToList();
                        var listActuallySpent = new List<ActuallySpentItemResponse>();
                        if (listIDsIn.Count > 0)
                        {
                            var costItemsIn = await _costEstimateItemRepository.GetByListRequestCode(listIDsIn);
                            // có mã yêu cầu không hợp lệ
                            if (costItemsIn.Count != listIDsIn.Count)
                            {
                                var listNotIn = listIDsIn.Where(x => costItemsIn.All(p => p.RequestCode != x));
                                foreach (var not in listNotIn)
                                {
                                    excelUploadResponse.AddError(new ExcelValidatorError
                                    {
                                        Message = $"{not} Mã dự trù không tồn tại "
                                    });
                                }
                            }
                            else
                            {
                                // nếu thì thì lũy kế
                                // cộng dồn và so sánh với thực chi 
                                // xem còn có cần thanh toán không ?
                                listInCostEstimate.ForEach(x =>
                                {
                                    var estimate = costItemsIn.First(m =>
                                        m.RequestCode.Equals(x.RequestCode, StringComparison.OrdinalIgnoreCase));
                                    var week = x.Day.Year.WeekInYear().OrderBy(k => k.weekNum)
                                        .FirstOrDefault(k => k.weekStart >= x.Day);
                                    listActuallySpent.Add(new ActuallySpentItemResponse
                                    {
                                        AccountantCode = x.BillCode,
                                        ActualSpent = x.IncurredHave ?? 0,
                                        Cost = estimate.Cost,
                                        AmountLeft = estimate.Cost - (x.IncurredHave ?? 0) < 0 ? 0 : estimate.Cost - (x.IncurredHave ?? 0),
                                        ActualSpentType = "In",
                                        Creator = estimate.CreatorId,
                                        CreatorName = estimate.CreatorName,
                                        RequestCode = estimate.RequestCode,
                                        RequestContent = estimate.RequestContent,
                                        RequestPayWeek = estimate.PayWeek,
                                        RequestPayWeekName = estimate.PayWeekName,
                                        ActualPayWeek = week?.weekNum ?? -1,
                                        ActualPayWeekName = $"Tuần {week?.weekNum}",
                                        Record = Guid.NewGuid().ToString(),
                                        CostEstimateItemTypeId = estimate.CostEstimateItemTypeId,
                                        CostEstimateItemTypeName = estimate.CostEstimateItemTypeName,
                                        Explanation = estimate.Explanation
                                    });
                                });
                            }
                        }

                        // Phát sinh ngoài dự trù
                        var listOutCostEstimate = listActuallyData.Where(x => string.IsNullOrEmpty(x.RequestCode)).ToList();
                        listOutCostEstimate.ForEach(x =>
                        {
                            var week = x.Day.Year.WeekInYear().OrderBy(k => k.weekNum)
                                .FirstOrDefault(k => k.weekStart >= x.Day);
                            listActuallySpent.Add(new ActuallySpentItemResponse
                            {
                                AccountantCode = x.BillCode,
                                ActualSpent = x.IncurredHave ?? 0,
                                ActualSpentType = "Out",
                                Creator = GetUserId(),
                                CreatorName = GetUserName(),
                                RequestContent = x.RequestContent,
                                ActualPayWeek = week?.weekNum ?? -1,
                                ActualPayWeekName = $"Tuần {week?.weekNum}",
                                RequestPayWeekName = "Phát sinh ngoài dự trù",
                                //random UUID cho editor
                                Record = Guid.NewGuid().ToString(),
                            });
                        });

                        excelUploadResponse.SpecifyFieldValue = new ActuallyUploadModel
                        {
                            Data = listActuallySpent,
                            TotalActualSpentAtTime = listActuallySpent.Sum(x => x.ActualSpentAtTime),
                            TotalAmountLeft = listActuallySpent.Sum(x => x.AmountLeft),
                            TotalActualSpent = listActuallySpent.Sum(x => x.ActualSpent),
                            TotalCost = listActuallySpent.Sum(x => x.Cost)
                        };
                    }

                    excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

                    return Json(excelUploadResponse);
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                excelUploadResponse.AddError(new ExcelValidatorError { Message = "Có lỗi xảy ra, vui lòng thử lại sau!" });
                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
            }
            var serSettings = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
            };
            //JsonSerializerSettings jSettings = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented };
            return Json(excelUploadResponse, serSettings);
        }

        /// <summary>
        /// Tạo / chỉnh sửa chi tiết
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.ActuallySpentCreate, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> OnCreate(CreateActuallySpentRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                request.UnitId = sessionUser.Unit.UnitId;
                request.UnitName = sessionUser.Unit.UnitName;

                request.Creator = GetUserId();
                request.CreatorName = GetUserName();
                request.RequestPage = ActuallySpent.PublicKey;
                var allTypes = await _paymentRepository.AllTypes(request.IsSub ? "sub" : sessionUser.Unit.UnitType);
                var allPayments = await _paymentRepository.AllPayments();
                foreach (var actuallySpentItemResponse in request.Data)
                {
                    var costTypeFromName =
                        allTypes.FirstOrDefault(x => x.Name.Equals(actuallySpentItemResponse.CostEstimateItemTypeName));
                    actuallySpentItemResponse.CostEstimateItemTypeId = costTypeFromName?.Id ?? -1;
                    if (costTypeFromName != null)
                        actuallySpentItemResponse.CostEstimateGroupName =
                            allPayments.FirstOrDefault(x => x.Id == costTypeFromName.PaymentType)?.Name;
                }


                if (!string.IsNullOrEmpty(request.Record))
                {

                    var rqLoadStatusAllows = new DataSeenRequest
                    {
                        Type = GlobalEnums.ActuallySpent,
                        CostEstimateType = GlobalEnums.Week,
                        Subject = IsSubUnit() ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                        GroupCodes = GetUserPositionCodes()
                    };
                    var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                    // Không được xem dữ liệu nào
                    if (!listStatsForDepartmentGroup.Any())
                        return Json(new CostEstimateItemSearchResponse
                        {
                            Code = (int)HttpStatusCode.Forbidden,
                            Message = "Không có quyền thực hiện thao tác!"
                        });
                    request.StatsAllowSeen = listStatsForDepartmentGroup;
                    request.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                        Functions.ActuallySpentView, PermissionConstant.EDIT);
                    request.IsSub = IsSubUnit();
                    var responseEdit = await _actuallySpentRepository.EditAsync(request, sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut);

                    return Json(responseEdit);
                }

                #region Di chuyển từ file tạm sang thư mục backup
                var physicalFile = request.SctData?.FirstOrDefault();
                if (physicalFile != null)
                {
                    try
                    {
                        string fullTempPath = Path.Combine(_webHostEnvironment.WebRootPath, physicalFile.Path);
                        string ownerFolder = GetUserSyncId().CreateDatePhysicalPathFileToUser();
                        string fullOwnerPath = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder, ownerFolder);
                        if (!Directory.Exists(fullOwnerPath))
                            Directory.CreateDirectory(fullOwnerPath);
                        string fullFilePath = Path.Combine(fullOwnerPath, physicalFile.FileName);
                        System.IO.File.Move(fullTempPath, fullFilePath);
                        request.PathBackup = fullFilePath;
                        var response = await _actuallySpentRepository.CreateAsync(request);

                        if (response.Code != (int)GlobalEnums.ResponseCodeEnum.Success)
                            System.IO.File.Move(fullFilePath, fullTempPath);

                        return Json(response);
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogLevel.Error, e, e.Message);
                        return Json(new EmptySearchResponseModel
                        { Code = (int)GlobalEnums.ResponseCodeEnum.Error, Message = "Lưu dữ liệu không thành công, vui lòng liên hệ QTV!" });
                    }
                }
                else
                {
                    // sai định dạng 
                    return Json(new EmptySearchResponseModel
                    { Code = (int)GlobalEnums.ResponseCodeEnum.Error, Message = "Dữ liệu không đúng định dạng!" });
                }
                #endregion
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new EmptySearchResponseModel
                { Code = (int)GlobalEnums.ResponseCodeEnum.Error, Message = GlobalEnums.ErrorMessage });
            }

        }

        /// <summary>
        /// Duyệt / Từ chối báo cáo thực chi
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser(Module = Functions.ActuallySpentView, Permission = PermissionConstant.APPROVE)]

        public async Task<IActionResult> Approval(ActuallySpentApproveRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                request.PageRequest = ActuallySpent.PublicKey;
                request.UserId = GetUserId();
                request.UserName = GetUserName();
                request.UnitId = !ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0)
                    ? GetUserUnit().UnitId
                    : (int)GlobalEnums.StatusDefaultEnum.All;

                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.ActuallySpent,
                    CostEstimateType = GlobalEnums.Week,
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
                request.IsSub = IsSubUnit();
                request.UnitType = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut;
                request.Positions = sessionUser.Positions;
                var response = await _actuallySpentRepository.Approval(request);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new ActuallySpentApproveResponse
                {
                    Message = GlobalEnums.ErrorMessage,
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                });
            }
        }

        /// <summary>
        /// Xem lịch sử phê duyệt / từ chối
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.ActuallySpentView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> ViewHistories(string record)
        {
            var request = new ActuallyLogRequest
            {
                Record = record,
                RequestPage = ActuallySpent.PublicKey
            };
            IList<ActuallyLogResponse> data = null;
            try
            {
                data = await _actuallySpentRepository.ViewHistories(request);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
            }
            return PartialView("_ViewHistory", data);
        }


        //[AuthorizeUser(Module = Functions.CostEstimateView, Permission = PermissionConstant.VIEW)]
        //public async Task<IActionResult> Manage()
        //{
        //    var searchModel = new SearchModel
        //    {
        //        Stats = GlobalEnums.DefaultStatusSearchList.Select(cc => new SelectListItem { Text = cc.Name, Value = $"{cc.Value}" }).ToList(),
        //        DefaultStats = Globs.DefaultSearchAllStats,
        //        DefaultStatsName = Globs.DefaultSearchAllStatsName
        //    };
        //    try
        //    {
        //        var allUnits = (await _unitRepository.GetAllAsync(string.Empty, 0, 1000));
        //        var searchUnit = allUnits
        //            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"[{x.OfficesCode}] - {x.OfficesShortName ?? x.OfficesName}" }).ToList();
        //        var unitSearch = new List<SelectListItem>();
        //        unitSearch.AddRange(searchUnit);
        //        searchModel.Units = unitSearch;
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e, "Error");
        //    }

        //    return View(searchModel);
        //}
        //[AuthorizeUser(Module = Functions.CostEstimateView, Permission = PermissionConstant.VIEW)]
        //public async Task<IActionResult> SearchManage(int length, int start, SearchManageRequest @base)
        //{
        //    try
        //    {
        //        @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();
        //        @base.HostFileView = _fileHostView;
        //        var data = await _costEstimateRepository.SearchManage(@base, start, length);
        //        data.Draw = @base.Draw;
        //        return Json(data);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e, "Error");
        //        return Json(new SearchCostEstimateResponse
        //        {
        //            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
        //            Message = "Lỗi hệ thống, vui lòng thử lại sau!",
        //            Draw = @base.Draw
        //        });
        //    }
        //}

        //[AuthorizeUser(Module = Functions.CostEstimateView, Permission = PermissionConstant.VIEW)]
        //public async Task<IActionResult> ExportFile(List<DashboardExportRequest> data, string exportType)
        //{
        //    try
        //    {
        //        var excelPaths = new List<FileNPlanType>();
        //        var pdfPaths = new List<FileNPlanType>();
        //        var listUnits = new List<int>();
        //        var export = await _dashboardRepository.Export(data, excelPaths, pdfPaths, listUnits, _physicalFolder);
        //        if (export.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
        //        {
        //            var exporter = new Exporter();
        //            var rtExport = exporter.Export(excelPaths, pdfPaths, listUnits, exportType, _physicalFolder, _fileHostView);
        //            return Json(rtExport);

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e, "Error");
        //    }
        //    return Json(new DashboardListResponse
        //    {
        //        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
        //        Message = "Lỗi hệ thống, vui lòng thử lại sau!"
        //    });
        //}
    }
}
