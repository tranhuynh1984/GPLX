using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aspose.Cells;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.PdfLogs;
using GPLX.Core.Contracts.Profit;
using GPLX.Core.Contracts.Revenue;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Core.DTO.Entities;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.Profit;
using GPLX.Core.DTO.Request.Signature;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.CashFollow;
using GPLX.Core.DTO.Response.CostEstimate;
using GPLX.Core.DTO.Response.ProfitPlan;
using GPLX.Core.Enum;
using GPLX.Database.Models;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models.Revenue;
using GPLX.Web.Signature;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Web.Controllers
{
    public class ProfitPlanController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGroupsRepository _groupsRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IProfitPlanRepository _profitPlanRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPdfLogsRepository _pdfLogsRepository;
        private readonly IRevenuePlanRepository _revenuePlanRepository;

        private readonly string _defaultRootFolder;
        private readonly string _fileHostView;
        private readonly string _physicalFolder;

        public ProfitPlanController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IGroupsRepository groupsRepository, ICostStatusesRepository costStatusesRepository, IUnitRepository unitRepository, IProfitPlanRepository profitPlanRepository, IUserRepository userRepository, IPdfLogsRepository pdfLogsRepository, IRevenuePlanRepository revenuePlanRepository)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _groupsRepository = groupsRepository;
            _costStatusesRepository = costStatusesRepository;
            _unitRepository = unitRepository;
            _profitPlanRepository = profitPlanRepository;
            _userRepository = userRepository;
            _pdfLogsRepository = pdfLogsRepository;
            _revenuePlanRepository = revenuePlanRepository;
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");
            _fileHostView = configuration.GetValue<string>("FileHosting");
            _physicalFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder);
        }

        [AuthorizeUser(Module = Functions.ProfitPlanView, Permission = PermissionConstant.VIEW)]
        public IActionResult Create()
        {
            ViewBag.IsSub = IsSubUnit();
            ViewBag.UnitType = IsSubUnit() ? "sub" : GetUserUnit().UnitType.ToLower();
            return View();
        }
        [AuthorizeUser(Module = Functions.ProfitPlanView, Permission = PermissionConstant.VIEW)]
        public async Task<IActionResult> List()
        {
            var model = new RevenuePlanListModel
            {
                Stats = GlobalEnums.DefaultStatusSearchList,
                DefaultStats = Globs.DefaultSearchAllStats,
                DefaultStatsName = Globs.DefaultSearchAllStatsName,
                EnableSearchUnit = ComparePositionLevelUpper(((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0), ComparingMode.GatherThan),
                Units = new List<SelectListItem>()
            };
            try
            {
                ViewBag.AddPermission = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.ProfitPlanAdd, PermissionConstant.ADD);
                if (model.EnableSearchUnit)
                {
                    var unitManages = await _userRepository.GetUserUnitManages(GetUsersSessionModel().UserId);
                    var allUnits = (await _unitRepository.GetAllAsync(string.Empty, 0, 1000));
                    if (unitManages != null && unitManages.Count > 0)
                        allUnits = allUnits.Where(x => unitManages.Any(c => c.OfficeId == x.Id)).ToList();

                    var searchUnit = allUnits
                        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"[{x.OfficesCode}] - {x.OfficesShortName ?? x.OfficesName}" }).ToList();
                    var unitSearch = new List<SelectListItem> { new SelectListItem { Text = "Tất cả", Value = "-100" } };
                    unitSearch.AddRange(searchUnit);
                    model.Units = unitSearch;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }


            return View(model);
        }
        [AuthorizeUser(Module = Functions.ProfitPlanAdd, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnExcelUpload([FromQuery(Name = "year")] int selectedFinanceYear)
        {
            var excelUploadResponse = new ExcelUploadResponse<ProfitPlanAggregates>();
            if (selectedFinanceYear.IsLastDayOfYear())
            {
                excelUploadResponse.AddError(new ExcelValidatorError { Message = $"Hết hạn năm tài chính {selectedFinanceYear}!" });
                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                return Json(excelUploadResponse);
            }

            if (selectedFinanceYear <= 0)
            {
                excelUploadResponse.AddError(new ExcelValidatorError { Message = "Cần chọn năm tài chính!" });
                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                return Json(excelUploadResponse);
            }

            var listData = new ProfitPlanExcelUploadResponse();
            var sessionUser = GetUsersSessionModel();
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

                    if (workBook.Worksheets.Count == 0)
                    {
                        excelUploadResponse.AddError(new ExcelValidatorError { Message = "Không có sheet dữ liệu yêu cầu!" });
                        excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        return Json(excelUploadResponse);
                    }

                    // lấy các sheet không ẩn
                    // và khác sheet hướng dẫn
                    var sheetNamesMatch = workBook.Worksheets.Cast<Worksheet>().Where(x => x.IsVisible).ToList();
                    var listDetails = new List<ProfitPlanDetailExcel>();
                    if (sheetNamesMatch.FirstOrDefault(m => !m.Name.Contains("Hướng dẫn", StringComparison.OrdinalIgnoreCase)) == null)
                    {
                        excelUploadResponse.AddError(new ExcelValidatorError { Message = "Không có sheet dữ liệu yêu cầu!" });
                        excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        return Json(excelUploadResponse);
                    }

                    string unitType = IsSubUnit() ? "sub" : sessionUser.Unit.UnitType;

                    var profitPlanGroups = await _profitPlanRepository.ListGroups(unitType);
                    var listAggregates = new List<ProfitPlanAggregatesExcel>();

                    if (excelUploadResponse.IsValid)
                    {
                        var ws = sheetNamesMatch[0];
                        var cells = ws.Cells;
                        // năm lập báo cáo
                        var year = cells["D8"].StringValue.ToInt32();
                        if (selectedFinanceYear != year)
                        {
                            excelUploadResponse.AddError(new ExcelValidatorError
                            {
                                Message = $"Cần đẩy sheet dữ liệu theo đúng năm tài chính đã chọn {selectedFinanceYear}. Năm tài chính của sheet là {year}!"
                            });
                            excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            return Json(excelUploadResponse);
                        }
                        //
                        var excelValidator = new ExcelFormValidation(_configuration);
                        excelValidator = excelValidator.Load("ExcelFormValidator:ProfitPlan");
                        Type oType = typeof(ProfitPlanDetailExcel);

                        bool matchHeader = false;
                        bool matchColHeader = false;
                        string sMatchHeader = "II. Kế hoạch Lợi nhuận";

                        int profitPlanGroupBeforeId = 0;
                        int rowCounter = 0;
                        var root = profitPlanGroups.Where(x => x.ProfitPlanParentGroupId == 0).ToList();
                        var seconds = profitPlanGroups.Where(c =>
                            root.Any(x => x.ProfitPlanGroupId == c.ProfitPlanParentGroupId)).ToList();
                        ProfitPlanGroups profitBefore = null;

                        var listCellErrors = new List<ExcelValidatorError>();

                        var excelRows = new List<ExcelRow>();
                        foreach (Row cellsRow in cells.Rows)
                        {
                            if (cellsRow.IsHidden)
                                continue;

                            var cellsInRow = new List<ExcelCell>();
                            // bắt đầu từ dòng 7
                            var fCell = cellsRow.FirstCell.Row;
                            var cellB = cellsRow.GetCellOrNull(1);
                            // khi nào gặp cell này thì mới đọc dữ liệu
                            if (cellB?.StringValue.Contains(sMatchHeader, StringComparison.OrdinalIgnoreCase) == true)
                                matchHeader = true;
                            else if (cellB?.StringValue.Contains("STT", StringComparison.OrdinalIgnoreCase) == true && matchHeader)
                                matchColHeader = true;
                            if (fCell < excelValidator.StartAtRow) { continue; }

                            if (!matchHeader || !matchColHeader)
                                continue;

                            rowCounter++;
                            bool header = rowCounter == 1;


                            for (int col = 0; col <= excelValidator.ColumnConfigs.Max(c => c.Position); col++)
                            {
                                var dataAt = cellsRow[col];
                                var xCellColumnValidator = excelValidator.ColumnConfigs.FirstOrDefault(x => x.Position == col);
                                // nếu cột <Nội dung> trống thì bỏ qua luôn row
                                if (!header && xCellColumnValidator?.FieldNameMapper == "ProfitPlanGroupName" &&
                                    string.IsNullOrEmpty(dataAt.StringValue))
                                    break;


                                if (xCellColumnValidator != null)
                                {
                                    var cell = excelValidator.ReadCellAt(xCellColumnValidator, dataAt, header);
                                    cell.CellPosition = col;

                                    cellsInRow.Add(cell);
                                }
                            }

                            string cellContent = cellsInRow.FirstOrDefault(x => x.CellPosition == 2)?.StringCellValue;
                            if (!string.IsNullOrEmpty(cellContent))
                                excelRows.Add(new ExcelRow
                                {
                                    Cells = cellsInRow,
                                    RowIndex = fCell
                                });
                        }

                        foreach (var xcw in excelRows)
                        {
                            var fCellCashGroup = xcw.Cells.FirstOrDefault(x => x.FieldMapper == "ProfitPlanGroupName");
                            var oContent = profitPlanGroups.FirstOrDefault(x =>
                                fCellCashGroup?.ReaderVal?.ToString().StartsWith(x.ProfitPlanGroupName, StringComparison.OrdinalIgnoreCase) == true && x.ProfitPlanGroupId != profitPlanGroupBeforeId && string.IsNullOrEmpty(x.GroupFor));

                            if (oContent != null)
                            {
                                if (root.Any(c => c.ProfitPlanGroupId == oContent.ProfitPlanGroupId) ||
                                    seconds.Any(c => c.ProfitPlanGroupId == oContent.ProfitPlanGroupId))
                                    profitBefore = oContent;
                                else
                                {
                                    var reGet = profitPlanGroups.FirstOrDefault(x =>
                                        fCellCashGroup?.ReaderVal?.ToString().StartsWith(x.ProfitPlanGroupName, StringComparison.OrdinalIgnoreCase) == true && x.ProfitPlanGroupId != profitPlanGroupBeforeId && x.ProfitPlanParentGroupId == profitBefore?.ProfitPlanGroupId && string.IsNullOrEmpty(x.GroupFor));
                                    oContent = reGet ?? oContent;
                                }
                                profitPlanGroupBeforeId = oContent.ProfitPlanGroupId;
                            }


                            if (oContent == null && fCellCashGroup != null)
                                // lỗi nhóm dữ liệu không hợp lệ
                                listCellErrors.Add(new ExcelValidatorError { Message = $"Nhóm dữ liệu <b>{fCellCashGroup.ReaderVal}</b> không hợp lệ" });
                            else if (oContent != null)
                            {
                                xcw.GroupId = oContent.ProfitPlanGroupId;
                                xcw.GroupName = oContent.ProfitPlanGroupName;
                                xcw.Style = oContent.Style;
                                xcw.Rules = oContent.RulesCellOnRow;
                            }
                        }

                        RowCellsValidate rcValidate = new RowCellsValidate
                        {
                            AllRows = excelRows
                        };

                        bool isValid = rcValidate.ValidateRows();

                        if (isValid && rcValidate.AllRows.All(c => c.Cells.All(m => !m.IsNotValidCell)))
                        {
                            Type oCellType = typeof(ProfitPlanDetailExcel);

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

                                var oExcelRow = (ProfitPlanDetailExcel)instanceOf;
                                oExcelRow.ProfitPlanGroupId = excelRow.GroupId;
                                oExcelRow.ProfitPlanGroupName = excelRow.GroupName;
                                oExcelRow.Style = excelRow.Style;
                                oExcelRow.Row = excelRow.RowIndex;
                                listDetails.Add(oExcelRow);
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

                            excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            excelUploadResponse.Message = "Tệp tải lên không đúng định dạng, tải xuống tệp lỗi để xem chi tiết";
                            excelUploadResponse.ExcelFileError = $"{randomFolderName}/{errorFile}".CreateHostFileView(_fileHostView);
                            return Json(excelUploadResponse);
                        }

                        #region validate rules
                        var allRequireGroup = profitPlanGroups.Where(c => c.IsRequire && string.IsNullOrEmpty(c.GroupFor)).ToList();
                        allRequireGroup.ForEach(c =>
                        {
                            if (listDetails.All(x => x.ProfitPlanGroupId != c.ProfitPlanGroupId))
                                listCellErrors.Add(new ExcelValidatorError { Message = $"Không tìm thấy nhóm dữ liệu <b>{c.ProfitPlanGroupName}</b> trong biểu mẫu" });
                        });
                        var revenuePlanDetails =
                            await _revenuePlanRepository.GetLatestUnitRevenuePlanCashDetails(year, sessionUser.Unit.UnitId);
                        if (revenuePlanDetails == null || revenuePlanDetails.Count == 0)
                        {
                            excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            excelUploadResponse.Message = $"Không tìm thấy dữ liệu doanh thu khách hàng năm {year}";
                            return Json(excelUploadResponse);
                        }

                        var revenuePlanContent = await _revenuePlanRepository.ListRevenuePlanContents(unitType);

                        #region Tổng doanh thu

                        var revenueTotalGroup = revenuePlanContent.FirstOrDefault(c =>
                            c.RevenuePlanContentName.Equals("Tổng doanh thu",
                                StringComparison.CurrentCultureIgnoreCase));

                        var revenueTotal = revenuePlanDetails.FirstOrDefault(c =>
                            c.RevenuePlanContentId == revenueTotalGroup?.RevenuePlanContentId);

                        var profitTotalRevenueGroup = profitPlanGroups.FirstOrDefault(c =>
                            c.ProfitPlanGroupName.Equals("Doanh thu chỉ định",
                                StringComparison.CurrentCultureIgnoreCase));
                        string messageCompare = "Doanh thu chỉ định";
                        if (!unitType.Equals(GlobalEnums.UnitIn))
                        {
                            profitTotalRevenueGroup = profitPlanGroups.FirstOrDefault(c =>
                               c.ProfitPlanGroupName.Equals("Tổng doanh thu",
                                   StringComparison.CurrentCultureIgnoreCase));
                            messageCompare = "Tổng doanh thu";
                        }

                        var profitTotalRevenue = listDetails.FirstOrDefault(c =>
                            c.ProfitPlanGroupId == profitTotalRevenueGroup?.ProfitPlanGroupId);

                        if (profitTotalRevenue == null)
                        {
                            excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            excelUploadResponse.Message = "Không tìm thấy dữ liệu " + messageCompare;
                            return Json(excelUploadResponse);
                        }

                        string formatMsgCompareRevenue =
                            messageCompare + " <b>Tháng {0}</b> không khớp với doanh thu của Tháng {0} trong KH Doanh thu - Khách hàng";

                        if (revenueTotal.M1 != profitTotalRevenue.M1)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 1) });
                        if (revenueTotal.M2 != profitTotalRevenue.M2)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 2) });

                        if (revenueTotal.M3 != profitTotalRevenue.M3)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 3) });

                        if (revenueTotal.M4 != profitTotalRevenue.M4)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 4) });

                        if (revenueTotal.M5 != profitTotalRevenue.M5)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 5) });

                        if (revenueTotal.M6 != profitTotalRevenue.M6)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 6) });

                        if (revenueTotal.M7 != profitTotalRevenue.M7)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 7) });

                        if (revenueTotal.M8 != profitTotalRevenue.M8)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 8) });

                        if (revenueTotal.M9 != profitTotalRevenue.M9)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 9) });

                        if (revenueTotal.M10 != profitTotalRevenue.M10)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 10) });

                        if (revenueTotal.M11 != profitTotalRevenue.M11)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 11) });

                        if (revenueTotal.M12 != profitTotalRevenue.M12)
                            listCellErrors.Add(new ExcelValidatorError { Message = string.Format(formatMsgCompareRevenue, 12) });

                        if (revenueTotal.Total != profitTotalRevenue.Total)
                            listCellErrors.Add(new ExcelValidatorError { Message = "Doanh thu chỉ định cả năm không khớp với tổng doanh thu cả năm trong KH Doanh thu - Khách hàng" });

                        #endregion


                        #endregion

                        #region comment upload version old

                        //foreach (Row cellsRow in cells.Rows)
                        //{


                        //    var cellInRow = new List<ExcelCell>();
                        //    var fCell = cellsRow.FirstCell.Row;
                        //    var cellB = cellsRow.GetCellOrNull(1);
                        //    // khi nào gặp cell này thì mới đọc dữ liệu
                        //    if (cellB?.StringValue.Contains(sMatchHeader, StringComparison.OrdinalIgnoreCase) == true)
                        //        matchHeader = true;
                        //    else if (cellB?.StringValue.Contains("STT", StringComparison.OrdinalIgnoreCase) == true && matchHeader)
                        //        matchColHeader = true;

                        //    if (cellsRow.IsHidden)
                        //        continue;
                        //    var instanceOf = Activator.CreateInstance(oType);

                        //    if (fCell < excelValidator.StartAtRow) { continue; }
                        //    if (!matchHeader || !matchColHeader) { continue; }

                        //    rowCounter++;
                        //    bool header = rowCounter == 1;
                        //    for (int j = 0; j <= excelValidator.ColumnConfigs.Max(x => x.Position); j++)
                        //    {
                        //        var dataAt = cellsRow[j];
                        //        var xCellColumnValidator = excelValidator.ColumnConfigs.FirstOrDefault(x => x.Position == j);

                        //        if (xCellColumnValidator != null && header)
                        //        {
                        //            var cellValid = excelValidator.ValidCell(xCellColumnValidator, dataAt.StringValue, true, instanceOf,
                        //                typeof(ProfitPlanDetails), dataAt);
                        //            if (!cellValid.Item1)
                        //                excelValidator.Errors.Add(new ExcelValidatorError { Column = dataAt.Name, Message = cellValid.Item2 });
                        //        }
                        //        else if (xCellColumnValidator != null && fCell > excelValidator.StartAtRow)
                        //        {
                        //            if (instanceOf != null)
                        //            {
                        //                var cellValid = excelValidator.ValidCell(xCellColumnValidator, dataAt.StringValue, false, instanceOf, oType, dataAt);
                        //                if (!cellValid.Item1)
                        //                    excelValidator.Errors.Add(new ExcelValidatorError { Column = dataAt.Name, Message = cellValid.Item2 });
                        //            }
                        //        }
                        //    }

                        //    if (oType == typeof(ProfitPlanDetailExcel))
                        //    {
                        //        var oAdd = (ProfitPlanDetailExcel)instanceOf;

                        //        var oContent = profitPlanGroups.FirstOrDefault(x =>
                        //            oAdd?.ProfitPlanGroupName?.StartsWith(x.ProfitPlanGroupName, StringComparison.OrdinalIgnoreCase) == true && x.ProfitPlanGroupId != profitPlanGroupBeforeId);

                        //        if (oContent != null)
                        //        {
                        //            if (root.Any(c => c.ProfitPlanGroupId == oContent.ProfitPlanGroupId) ||
                        //                seconds.Any(c => c.ProfitPlanGroupId == oContent.ProfitPlanGroupId))
                        //                profitBefore = oContent;
                        //            else
                        //            {
                        //                var reGet = profitPlanGroups.FirstOrDefault(x =>
                        //                    oAdd?.ProfitPlanGroupName?.StartsWith(x.ProfitPlanGroupName, StringComparison.OrdinalIgnoreCase) == true && x.ProfitPlanGroupId != profitPlanGroupBeforeId && x.ProfitPlanParentGroupId == profitBefore?.ProfitPlanGroupId);
                        //                oContent = reGet ?? oContent;
                        //            }
                        //            profitPlanGroupBeforeId = oContent.ProfitPlanGroupId;
                        //        }

                        //        if (oAdd != null)
                        //        {
                        //            oAdd.ProfitPlanGroupId = oContent?.ProfitPlanGroupId ?? -1;
                        //            if (!string.IsNullOrEmpty(oAdd.ProfitPlanGroupName) && oContent != null)
                        //                listDetails.Add(oAdd);
                        //        }
                        //    }

                        //}

                        #endregion




                        string errorSignerPos = await _costStatusesRepository.ValidateSignerPositionInExcel(ws, GlobalEnums.Profit,
                            GlobalEnums.Year, unitType, sessionUser.Unit.UnitId);
                        if (!string.IsNullOrEmpty(errorSignerPos))
                            listCellErrors.Add(new ExcelValidatorError { Message = errorSignerPos });

                        excelUploadResponse.Errors = listCellErrors;

                        if (!excelUploadResponse.IsValid)
                        {
                            try
                            {
                                System.IO.File.Delete(file);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "exception {0}", e.Message);
                            }
                        }
                        else
                        {
                            #region Tổng doanh thu

                            var gTotalRevenue =
                                profitPlanGroups.FirstOrDefault(x => x.ProfitPlanGroupName.StartsWith("Tổng doanh thu"));
                            var gChildOfTotalRevenue = profitPlanGroups
                                .Where(x => x.ProfitPlanParentGroupId == (gTotalRevenue?.ProfitPlanGroupId ?? -100)).ToList();



                            var dTotalRevenue =
                                listDetails.FirstOrDefault(x => x.ProfitPlanGroupId == (gTotalRevenue?.ProfitPlanGroupId ?? -100));
                            var aggPlanRevenue = new ProfitPlanAggregatesExcel
                            {
                                Style = "normal",
                                ProfitPlanGroupName = gTotalRevenue?.ProfitPlanGroupName,
                                ProfitPlanGroupId = gTotalRevenue?.ProfitPlanGroupId ?? -1,
                                TotalCosh = dTotalRevenue?.Total ?? 0,
                                No = "1"
                            };
                            listAggregates.Add(aggPlanRevenue);

                            if (gChildOfTotalRevenue.Any() && unitType != GlobalEnums.UnitIn)
                            {
                                foreach (var gChild in gChildOfTotalRevenue)
                                {
                                    var dChildTotalRevenue = listDetails.FirstOrDefault(x => x.ProfitPlanGroupId == gChild.ProfitPlanGroupId);
                                    listAggregates.Add(new ProfitPlanAggregatesExcel
                                    {
                                        Style = "italic",
                                        ProfitPlanGroupName = gChild?.ProfitPlanGroupName,
                                        ProfitPlanGroupId = gChild?.ProfitPlanGroupId ?? -1,
                                        TotalCosh = dChildTotalRevenue?.Total ?? 0,
                                        Proportion = (dTotalRevenue?.Total ?? 0) > 0 ? Math.Round((dChildTotalRevenue?.Total ?? 0) / dTotalRevenue.Total * 100, MidpointRounding.AwayFromZero) : 0
                                    });
                                }
                            }

                            #endregion

                            #region Tổng chi phí
                            var gTotalExpense =
                                profitPlanGroups.FirstOrDefault(x => x.ProfitPlanGroupName.StartsWith("Tổng chi phí"));
                            // thuế
                            var gTax = profitPlanGroups.FirstOrDefault(x => x.ProfitPlanGroupName.StartsWith("Thuế TNDN"));

                            var dTotalExpense =
                                listDetails.FirstOrDefault(x => x.ProfitPlanGroupId == (gTotalExpense?.ProfitPlanGroupId ?? -100));
                            var dTax = listDetails.FirstOrDefault(x => x.ProfitPlanGroupId == (gTax?.ProfitPlanGroupId ?? -100));

                            var totalTax = (dTotalExpense?.Total ?? 0) + (dTax?.Total ?? 0);
                            var aggPlanExpense = new ProfitPlanAggregatesExcel
                            {
                                Style = "normal",
                                ProfitPlanGroupName = gTotalExpense.ProfitPlanGroupName,
                                ProfitPlanGroupId = gTotalExpense.ProfitPlanGroupId,
                                TotalCosh = Math.Round((dTotalExpense?.Total ?? 0) + (dTax?.Total ?? 0)),
                                Proportion = (dTotalRevenue?.Total ?? 0) > 0
                                    ? Math.Round(totalTax / dTotalRevenue.Total * 100, MidpointRounding.AwayFromZero)
                                    : 0,
                                No = "2"
                            };
                            listAggregates.Add(aggPlanExpense);



                            #endregion

                            #region Lợi nhuận sau thuế
                            var gTotalProfitTax =
                                profitPlanGroups.FirstOrDefault(x => x.ProfitPlanGroupName.StartsWith("Lợi nhuận sau thuế"));
                            var dTotalProfitTax = listDetails.FirstOrDefault(x => x.ProfitPlanGroupId == (gTotalProfitTax?.ProfitPlanGroupId ?? -100));
                            var totalProfitTax = (dTotalRevenue?.Total ?? 0) > 0
                                ? Math.Round((dTotalProfitTax?.Total ?? 0) / dTotalRevenue.Total * 100, MidpointRounding.AwayFromZero)
                                : 0;
                            var aggPlanTax = new ProfitPlanAggregatesExcel
                            {
                                Style = "normal",
                                ProfitPlanGroupName = gTotalProfitTax.ProfitPlanGroupName,
                                ProfitPlanGroupId = gTotalProfitTax.ProfitPlanGroupId,
                                TotalCosh = dTotalProfitTax?.Total ?? 0,
                                Proportion = totalProfitTax,
                                No = "3"
                            };
                            listAggregates.Add(aggPlanTax);

                            #endregion

                            if (dTotalProfitTax != null)
                            {
                                var indexOfProfitTax = listDetails.IndexOf(dTotalProfitTax);
                                dTotalProfitTax.ProPortion = (float)totalProfitTax;
                                listDetails[indexOfProfitTax] = dTotalProfitTax;


                                _setValueToSheet(cells, excelValidator.ColumnConfigs, "ProPortion", dTotalProfitTax.Row, $"{totalProfitTax}%");
                            }


                            var topGroups = profitPlanGroups.Where(x => x.ProfitPlanParentGroupId == 0).ToList();
                            var secondGroups = profitPlanGroups.Where(x =>
                                topGroups.Any(p => p.ProfitPlanGroupId == x.ProfitPlanParentGroupId)).ToList();

                            foreach (var profitPlanDetailExcel in listDetails)
                            {
                                if (!string.IsNullOrEmpty(profitPlanDetailExcel.No))
                                    profitPlanDetailExcel.Style = "bold";

                                if (topGroups.Any(p =>
                                        p.ProfitPlanGroupId == profitPlanDetailExcel.ProfitPlanGroupId) || secondGroups.Any(
                                        p => p.ProfitPlanGroupId == profitPlanDetailExcel.ProfitPlanGroupId) && !IsSubUnit())
                                {
                                    profitPlanDetailExcel.Style = "bold";
                                }

                            }

                            foreach (var sc in secondGroups)
                            {
                                var subOfSubs = profitPlanGroups.Where(x => x.ProfitPlanParentGroupId == sc.ProfitPlanGroupId).ToList();
                                if (subOfSubs.Count > 0)
                                {
                                    var details = listDetails.Where(x =>
                                        subOfSubs.Any(cx => x.ProfitPlanGroupId == cx.ProfitPlanGroupId)).ToList();
                                    var fSecond = listDetails.FirstOrDefault(x =>
                                        x.ProfitPlanGroupId == sc.ProfitPlanGroupId);

                                    if (fSecond != null)
                                    {
                                        int iOf = listDetails.IndexOf(fSecond);
                                        if (fSecond.M1 == 0)
                                        {
                                            fSecond.M1 = details.Sum(x => x.M1);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M1", fSecond.Row, fSecond.M1);
                                        }

                                        if (fSecond.M2 == 0)
                                        {
                                            fSecond.M2 = details.Sum(x => x.M2);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M2", fSecond.Row, fSecond.M2);
                                        }

                                        if (fSecond.M3 == 0)
                                        {
                                            fSecond.M3 = details.Sum(x => x.M3);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M3", fSecond.Row, fSecond.M3);
                                        }

                                        if (fSecond.M4 == 0)
                                        {
                                            fSecond.M4 = details.Sum(x => x.M4);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M4", fSecond.Row, fSecond.M4);
                                        }

                                        if (fSecond.M5 == 0)
                                        {
                                            fSecond.M5 = details.Sum(x => x.M5);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M5", fSecond.Row, fSecond.M5);
                                        }

                                        if (fSecond.M6 == 0)
                                        {
                                            fSecond.M6 = details.Sum(x => x.M6);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M6", fSecond.Row, fSecond.M6);
                                        }

                                        if (fSecond.M7 == 0)
                                        {
                                            fSecond.M7 = details.Sum(x => x.M7);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M7", fSecond.Row, fSecond.M7);
                                        }

                                        if (fSecond.M8 == 0)
                                        {
                                            fSecond.M8 = details.Sum(x => x.M8);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M8", fSecond.Row, fSecond.M8);
                                        }

                                        if (fSecond.M9 == 0)
                                        {
                                            fSecond.M9 = details.Sum(x => x.M9);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M9", fSecond.Row, fSecond.M9);
                                        }

                                        if (fSecond.M10 == 0)
                                        {
                                            fSecond.M10 = details.Sum(x => x.M10);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M10", fSecond.Row, fSecond.M10);
                                        }

                                        if (fSecond.M11 == 0)
                                        {
                                            fSecond.M11 = details.Sum(x => x.M11);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M11", fSecond.Row, fSecond.M11);
                                        }

                                        if (fSecond.M12 == 0)
                                        {
                                            fSecond.M12 = details.Sum(x => x.M12);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "M12", fSecond.Row, fSecond.M12);
                                        }

                                        if (fSecond.Total == 0)
                                        {
                                            fSecond.Total = details.Sum(x => x.Total);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "Total", fSecond.Row, fSecond.Total);
                                        }

                                        if (fSecond.ProPortion == 0)
                                        {
                                            fSecond.ProPortion = (float)((dTotalRevenue?.Total ?? 0) > 0
                                                ? Math.Round(fSecond.Total / dTotalRevenue.Total * 100, MidpointRounding.AwayFromZero)
                                                : 0);
                                            _setValueToSheet(cells, excelValidator.ColumnConfigs, "ProPortion", fSecond.Row, $"{fSecond.ProPortion}%");
                                        }

                                        listDetails[iOf] = fSecond;
                                    }
                                }
                            }

                            listData.ProfitPlanAggregates = listAggregates;
                            listData.ProfitPlanDetails = listDetails;

                            //save to new file excel
                            var calculateFile = file.Replace(".xlsx", "_calculate.xlsx");
                            workBook.Save(calculateFile, SaveFormat.Xlsx);

                            var profitPlan = new ProfitPlan
                            {
                                Year = year,
                                ProfitPlanType = IsSubUnit() ? "sub" : GetUserUnit().UnitType,
                                UnitName = sessionUser.Unit.UnitName,
                                PathExcel = calculateFile.Replace(_physicalFolder, string.Empty),
                                TotalRevenue = aggPlanRevenue.TotalCosh,
                                TotalExpense = aggPlanExpense.TotalCosh,
                                ProportionExpense = aggPlanExpense.Proportion,
                                TotalProfitTax = aggPlanTax.TotalCosh,
                                ProportionProfitTax = aggPlanTax.Proportion,
                            };
                            listData.ProfitPlan = profitPlan;
                        }
                    }



                    excelUploadResponse.SpecifyFieldValue = listData;
                    excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    return Json(excelUploadResponse);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "exception {0}", e.Message);
                excelUploadResponse.AddError(new ExcelValidatorError { Message = GlobalEnums.ErrorMessage });
                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
            }
            var serSettings = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
            };
            return Json(excelUploadResponse, serSettings);
        }
        [AuthorizeUser(Module = Functions.ProfitPlanAdd, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnCreate(ProfitPlanCreateRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                request.UnitName = sessionUser.Unit.UnitName;
                request.UnitId = sessionUser.Unit.UnitId;
                request.Creator = sessionUser.UserId;
                request.CreatorName = sessionUser.UserName;
                request.IsSub = IsSubUnit();

                #region Tạo PDF

                try
                {
                    var folder = _physicalFolder;
                    var fExcelFile = $"{folder}{request.ProfitPlan.PathExcel}";
                    if (System.IO.File.Exists(fExcelFile))
                    {
                        var ownerFolder = sessionUser.UserName.CreateDatePhysicalPathFileToUser();
                        var fullOwnerFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder,
                            ownerFolder);
                        if (!Directory.Exists(fullOwnerFolder))
                            Directory.CreateDirectory(fullOwnerFolder);
                        var fullOwnerFile = Path.Combine(fullOwnerFolder, $"{Path.GetRandomFileName()}.xlsx");

                        if (System.IO.File.Exists(fullOwnerFile))
                            System.IO.File.Delete(fullOwnerFile);

                        System.IO.File.Copy(fExcelFile, fullOwnerFile);
                        var sign = new SignatureConnect();
                        string template = await _createPdfName(request.ProfitPlan.Year, sessionUser.Unit);
                        string pdf = sign.CreatePdf(fullOwnerFile, template, _physicalFolder, sessionUser.UserName);

                        if (string.IsNullOrWhiteSpace(pdf))
                            return Json(new ProfitPlanCreateResponse
                            {
                                Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                Message = "Không tạo được PDF!"
                            });

                        request.ProfitPlan.PathPdf = pdf;
                        request.ProfitPlan.PathExcel = fullOwnerFile.Replace(_physicalFolder, string.Empty).NormalizePath();
                    }
                    else
                    {
                        return Json(new ProfitPlanCreateResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            Message = "Không tìm thấy dữ liệu gốc!"
                        });
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "{0}", e.Message);
                    return Json(new ProfitPlanCreateResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = GlobalEnums.ErrorMessage
                    });
                }
                #endregion

                var response = await _profitPlanRepository.Create(request);
                if (response.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                {
                    await _pdfLogsRepository.CreateAsync(new FilePdfCreateLogs
                    {
                        Type = ProfitPlanConst.PublicKey,
                        UnitId = sessionUser.Unit.UnitId,
                        CreatedDate = DateTime.Now,
                        FilePath = request.ProfitPlan.PathPdf
                    });
                }
                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "request {0}", request);
                return Json(new ProfitPlanCreateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }
        [AuthorizeUser(Module = Functions.ProfitPlanView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, SearchProfitPlanRequest @base)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                @base.PageRequest = ProfitPlanConst.PublicKey;
                @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();
                @base.PermissionApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.ProfitPlanView, PermissionConstant.APPROVE);
                @base.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.ProfitPlanView, PermissionConstant.EDIT);
                @base.PermissionDelete = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.ProfitPlanView, PermissionConstant.DELETE);
                @base.IsSub = IsSubUnit();
                @base.UserUnitsManages = await _userRepository.GetUserUnitManages(sessionUser.UserId);

                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.Profit,
                    CostEstimateType = GlobalEnums.Year,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes(),
                };
                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new SearchCostEstimateResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!",
                        Draw = @base.Draw
                    });

                @base.StatusAllowsSeen = listStatsForDepartmentGroup;

                bool searchAllUnit = ComparePositionLevelUpper(((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0), ComparingMode.GatherThan);
                // từ kế toán trưởng MG trở lên 
                // thì có xem tất cả đơn vị
                if (!searchAllUnit)
                    @base.UserUnit = GetUserUnit().UnitId;
                @base.HostFileView = _fileHostView;

                var data = await _profitPlanRepository.SearchAsync(@base, start, length, sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut);
                return Json(data);
            }
            catch (Exception e)
            {
                Log.Error(e, "request {0}", @base);
                return Json(new SearchCostEstimateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                    Draw = @base.Draw
                });
            }
        }
        [AuthorizeUser(Module = Functions.ProfitPlanView, Permission = PermissionConstant.APPROVE)]

        public async Task<IActionResult> OnApproval(ProfitPlanApproveRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.Profit,
                    CostEstimateType = GlobalEnums.Year,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };

                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new ProfitPlanApproveResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });


                request.PageRequest = ProfitPlanConst.PublicKey;

                request.Positions = sessionUser.Positions;
                request.UserId = GetUserId();
                request.UserName = GetUserName();
                request.StatusAllowsSeen = listStatsForDepartmentGroup;
                request.IsSub = IsSubUnit();


                bool searchAllUnit = ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0, ComparingMode.GatherThan);
                // từ kế toán trưởng MG trở lên 
                // thì có xem tất cả đơn vị
                request.UnitId = !searchAllUnit ? GetUserUnit().UnitId : (int)GlobalEnums.StatusDefaultEnum.All;

                request.HostFileView = _fileHostView;


                var record = await _profitPlanRepository.GetByIdAsync(request.RawId);

                ProfitPlanApproveResponse response = new ProfitPlanApproveResponse();
                var checkPermissionApprove = await _profitPlanRepository.CheckPermissionApprove(request, record);
                var sign = new SignatureConnect();
                if (checkPermissionApprove.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                {
                    // ký số
                    if (checkPermissionApprove.IsSignature)
                    {
                        #region Ký số
                        var signOp = new SignOpts
                        {
                            EndpointToken = _configuration.GetSection("Signature:SERVICE_GET_TOKENURL").Value,
                            EndpointQuery = _configuration.GetSection("Signature:SERVICE_URL").Value,
                            EnterpriseAcc = _configuration.GetSection("Signature:ENTERPRISE_ACC").Value,
                            //EnterpriseUser = _configuration.GetSection("Signature:USER_ACC").Value,
                            //EnterprisePass = _configuration.GetSection("Signature:USER_PASS").Value,

                            EnterpriseUser = sessionUser.AccountSignature,
                            EnterprisePass = sessionUser.PasswordSignature,

                            ClientId = _configuration.GetSection("Signature:APP_ID").Value,
                            ClientSec = _configuration.GetSection("Signature:APP_SECRET").Value,
                            PhysicalPath = _physicalFolder,
                            RelativePath = record.PathPdf,
                            Signer = request.UserName,
                            TextFilter = checkPermissionApprove.Position.Name
                        };

                        var signRt = sign.Signature(signOp);

                        if (signRt.IsError)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            response.Message =
                                "Lỗi ký số, phê duyệt không thành công!";

                            return Json(response);
                        }
                        #endregion
                        record.PathPdf = signRt.SignaturePath.Replace(signOp.PhysicalPath, string.Empty);
                    }
                    else
                    {
                        // nếu từ chối
                        // --> tạo lại PDF
                        if (!checkPermissionApprove.IsApproval)
                        {
                            //string templateName = Path.GetFileName(record.PathPdf);
                            //var newPdf = sign.CreatePdf(record.PathExcel, templateName, _physicalFolder, record.CreatorName);
                            //if (string.IsNullOrEmpty(newPdf))
                            //    return Json(new CashFollowApproveResponse
                            //    {
                            //        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            //        Message = GlobalEnums.ErrorMessage
                            //    });
                            //record.PathPdf = newPdf;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(sessionUser.PathSignature))
                                return Json(new CashFollowApproveResponse
                                {
                                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                    Message = "Không tìm thấy mẫu chữ ký, vui lòng liên hệ quản trị viên!"
                                });
                            byte[] userSignature = System.IO.File.ReadAllBytes(sessionUser.PathSignature.CreateAbsolutePath(_physicalFolder));

                            var sigRt = sign.SingNormal(
                                record.PathPdf.CreateAbsolutePath(_physicalFolder),
                                userSignature, request.UserName, checkPermissionApprove.Position.Name);
                            if (sigRt.IsError)
                            {
                                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                response.Message =
                                    "Lỗi ký điện tử, phê duyệt không thành công!";
                                return Json(response);
                            }

                            record.PathPdf = sigRt.SignaturePath.Replace(_physicalFolder, string.Empty).NormalizePath();
                        }
                    }

                    request.UnitType = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut;
                    response = await _profitPlanRepository.Approval(request, record, checkPermissionApprove);
                }
                else
                    response = new ProfitPlanApproveResponse
                    {
                        Code = checkPermissionApprove.Code,
                        Message = checkPermissionApprove.Message
                    };

                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return Json(new ProfitPlanApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }
        [AuthorizeUser(Module = Functions.ProfitPlanView, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> OnDelete(ProfitPlanApproveRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.Profit,
                    CostEstimateType = GlobalEnums.Year,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };

                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new ProfitPlanApproveResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });


                request.PageRequest = ProfitPlanConst.PublicKey;

                request.Positions = sessionUser.Positions;
                request.UserId = sessionUser.UserId;
                request.UserName = sessionUser.UserName;
                request.StatusAllowsSeen = listStatsForDepartmentGroup;
                request.IsSub = IsSubUnit();

                var response = await _profitPlanRepository.Delete(request);
                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return Json(new ProfitPlanApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }
        [AuthorizeUser(Module = Functions.ProfitPlanView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> ViewHistories(string record)
        {
            var request = new ProfitPlanViewHistoryRequest
            {
                Record = record,
                PageRequest = ProfitPlanConst.PublicKey
            };
            IList<ProfitPlanViewHistoryResponse> data = null;
            try
            {
                data = await _profitPlanRepository.ViewHistories(request);
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", record);
            }
            return PartialView("_ViewHistory", data);
        }

        public async Task<string> _createPdfName(int year, UnitModel unit)
        {
            try
            {
                int fileCounter =
                    await _pdfLogsRepository.CounterDay(unit.UnitId, ProfitPlanConst.PublicKey);
                string templateName =
                    $"{DateTime.Now:yyyy-MM-dd}_{year}_{unit.UnitName.StringToNonUnicode().StringRemoveSpecial()}_Ke hoach loi nhuan_{fileCounter}.pdf";
                return templateName;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return string.Empty;
            }
        }

        int _getFieldPost(List<ExcelFormValidator> columns, string fieldMapper)
        {
            try
            {
                return columns.First(c => c.FieldNameMapper.Equals(fieldMapper)).Position;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return -1;
            }
        }

        void _setValueToSheet(Cells cells, List<ExcelFormValidator> columns, string fieldMapper, int row, object value)
        {
            var pos = _getFieldPost(columns, fieldMapper);
            if (pos != -1)
            {
                var fCell = cells[row, pos];
                fCell.PutValue(value);

                if (!fieldMapper.Equals("Proportion"))
                {
                    var cellStyle = fCell.GetStyle();
                    cellStyle.Custom = @"_(* #,##0_);_(* (#,##0);_(* ""-""??_);_(@_)";
                    fCell.SetStyle(cellStyle);
                }
                else
                {
                    var cellStyle = fCell.GetStyle();
                    cellStyle.HorizontalAlignment = TextAlignmentType.Center;
                    fCell.SetStyle(cellStyle);
                }
            }
        }
    }
}
