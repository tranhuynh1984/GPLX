using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Aspose.Cells;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.CashFollow;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Investment;
using GPLX.Core.Contracts.PdfLogs;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Core.DTO.Entities;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.CashFollow;
using GPLX.Core.DTO.Request.Signature;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.CashFollow;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.Enum;
using GPLX.Database.Models;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using GPLX.Web.Signature;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Web.Controllers
{
    public class CashFollowController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ActuallySpentController> _logger;
        private readonly ICashFollowRepository _cashFollowRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IInvestmentPlanRepository _investmentPlanRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IPdfLogsRepository _pdfLogsRepository;

        private readonly string _defaultRootFolder;
        private readonly string _fileHostView;
        private readonly string _physicalFolder;


        public CashFollowController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, ILogger<ActuallySpentController> logger, ICashFollowRepository cashFollowRepository, ICostStatusesRepository costStatusesRepository, IGroupsRepository groupsRepository, IUserRepository userRepository, IUnitRepository unitRepository, IPdfLogsRepository pdfLogsRepository, IInvestmentPlanRepository investmentPlanRepository)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _cashFollowRepository = cashFollowRepository;
            _costStatusesRepository = costStatusesRepository;
            _groupsRepository = groupsRepository;
            _userRepository = userRepository;
            _investmentPlanRepository = investmentPlanRepository;
            _unitRepository = unitRepository;
            _pdfLogsRepository = pdfLogsRepository;
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");
            _fileHostView = configuration.GetValue<string>("FileHosting");

            _physicalFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder);
        }
        [AuthorizeUser(Module = Functions.CashFollowAdd, Permission = PermissionConstant.ADD)]
        public async Task<IActionResult> Create(string record = default)
        {
            var model = new CashFollowCreateModel
            {
                EnableCreate = string.IsNullOrEmpty(record),
                Record = record,
            };

            if (!string.IsNullOrEmpty(record))
            {
                var getByIdModel = new CashFollowGetByIdRequest
                {
                    RequestPage = CashFollowConst.PublicKey,
                    Record = record
                };

                var response = await _cashFollowRepository.GetViewCashFollow(getByIdModel.RawId);
                if (response != null)
                {
                    response.CashFollow.PathPdf = response.CashFollow.PathPdf.CreateHostFileView(_fileHostView);
                }

                model.DataView = response;
            }
            ViewBag.UnitType = IsSubUnit() ? "sub" : GetUserUnit().UnitType.ToLower();
            return View(model);
        }
        /// <summary>
        /// Đọc & xử lý dữ liệu từ biểu mẫu kế hoạch dòng tiền
        /// </summary>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CashFollowAdd, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnExcelUpload([FromQuery(Name = "year")] int selectedFinanceYear)
        {
            var response = new ExcelUploadResponse<CashFollowAggregateExcel>();

            if (selectedFinanceYear.IsLastDayOfYear())
            {
                response.AddError(new ExcelValidatorError { Message = $"Hết hạn năm tài chính {selectedFinanceYear}!" });
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                return Json(response);
            }

            if (selectedFinanceYear <= 0)
            {
                response.Message = "Cần chọn năm tài chính!";
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                return Json(response);
            }

            var listData = new CashFollowExcelResponse();
            try
            {
                var sessionUser = GetUsersSessionModel();
                var fileOnRequest = Request.Form.Files;
                if (fileOnRequest.Count > 0)
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
                    var listDetails = new List<CashFollowItemExcel>();
                    var listAggregates = new List<CashFollowAggregateExcel>();


                    if (workBook.Worksheets.Count == 0)
                    {
                        response.Message = "Không có sheet dữ liệu yêu cầu!";
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        return Json(response);
                    }

                    // lấy các sheet không ẩn
                    // và khác sheet hướng dẫn
                    var sheetNamesMatch = workBook.Worksheets.Cast<Worksheet>().Where(x => x.IsVisible).ToList();
                    if (sheetNamesMatch.FirstOrDefault(m => !m.Name.Contains("Hướng dẫn", StringComparison.OrdinalIgnoreCase)) == null)
                    {
                        response.Message = "Không có sheet dữ liệu yêu cầu!";
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        return Json(response);
                    }
                    string unitType = IsSubUnit() ? "sub" : sessionUser.Unit.UnitType;
                    Worksheet worksheet = workBook.Worksheets[0];
                    var allCashFollowTypes = await _cashFollowRepository.GetListCastFollowTypes(unitType);
                    var cells = worksheet.Cells;
                    // năm lập báo cáo
                    // mặc định cell D8
                    var year = cells["D8"].StringValue.ToInt32();
                    if (selectedFinanceYear != year)
                    {
                        response.Message =
                            $"Cần đẩy sheet dữ liệu theo đúng năm tài chính đã chọn {selectedFinanceYear}. Năm tài chính của sheet là {year}!";
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        return Json(response);
                    }

                    var listCellErrors = new List<ExcelValidatorError>();

                    var excelValidator = new ExcelFormValidation(_configuration);
                    excelValidator = excelValidator.Load("ExcelFormValidator:CashFollow");
                    Type oType = typeof(CashFollowItemExcel);

                    bool matchHeader = false;

                    bool matchColHeader = false;

                    string sMatchHeader = "II. Kế hoạch dòng tiền chi tiết";
                    int rowCounter = 0;

                    IList<ExcelRow> excelRows = new List<ExcelRow>();

                    foreach (Row cellsRow in cells.Rows)
                    {
                        if (cellsRow.IsHidden)
                            continue;
                        var cellsInRow = new List<ExcelCell>();

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
                            if (!header && xCellColumnValidator?.FieldNameMapper == "CashFollowGroupName" &&
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
                        var fCellCashGroup = xcw.Cells.FirstOrDefault(x => x.FieldMapper == "CashFollowGroupName");
                        var cashFollowGroup = allCashFollowTypes.FirstOrDefault(c =>
                            c.Name.Equals(fCellCashGroup?.ReaderVal?.ToString().Trim(), StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrEmpty(c.GroupFor));

                        if (cashFollowGroup == null && fCellCashGroup != null)
                            // lỗi nhóm dữ liệu không hợp lệ
                            listCellErrors.Add(new ExcelValidatorError { Message = $"Nhóm dữ liệu <b>{fCellCashGroup.ReaderVal}</b> không hợp lệ" });
                        else if (cashFollowGroup != null)
                        {
                            xcw.GroupId = cashFollowGroup.Id;
                            xcw.GroupName = cashFollowGroup.Name;
                            xcw.Style = cashFollowGroup.Style;
                            xcw.Rules = cashFollowGroup.RulesCellOnRow;
                            xcw.SkipCellAts = cashFollowGroup.SkipCellAts;
                        }
                    }

                    RowCellsValidate rcValidate = new RowCellsValidate
                    {
                        AllRows = excelRows
                    };

                    bool isValid = rcValidate.ValidateRows();

                    if (isValid && rcValidate.AllRows.All(c => c.Cells.All(m => !m.IsNotValidCell)))
                    {
                        Type oCellType = typeof(CashFollowItemExcel);

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

                            var oExcelRow = (CashFollowItemExcel)instanceOf;
                            oExcelRow.CashFollowGroupId = excelRow.GroupId;
                            oExcelRow.CashFollowGroupName = excelRow.GroupName;
                            oExcelRow.Style = excelRow.Style;

                            listDetails.Add(oExcelRow);
                        }
                    }
                    else
                    {
                        string errorFile = rcValidate.CreateErrorFile(workBook, folder, excelFile.FileName);
                        if (string.IsNullOrEmpty(errorFile))
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            response.Message = GlobalEnums.ErrorMessage;
                            return Json(response);
                        }

                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = "Tệp tải lên không đúng định dạng, tải xuống tệp lỗi để xem chi tiết";
                        response.ExcelFileError = $"{randomFolderName}/{errorFile}".CreateHostFileView(_fileHostView);
                        return Json(response);
                    }


                    var investmentData = await _investmentPlanRepository.GetInvestmentPlanAggregateByYearAsync(year, sessionUser.Unit.UnitId);

                    if (investmentData == null || investmentData.Count == 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = $"Không tìm thấy dữ liệu kế hoạch đầu tư năm {year}";
                        return Json(response);
                    }

                    #region Validate rules

                    var allRequireGroup = allCashFollowTypes.Where(c => c.IsRequire && string.IsNullOrEmpty(c.GroupFor)).ToList();
                    allRequireGroup.ForEach(c =>
                    {
                        if (listDetails.All(x => x.CashFollowGroupId != c.Id))
                            listCellErrors.Add(new ExcelValidatorError { Message = $"Không tìm thấy nhóm dữ liệu <b>{c.Name}</b> trong biểu mẫu" });
                    });


                    var investDataGroup = await _investmentPlanRepository.GetAllInvestmentPlanContentsForSubject(unitType);
                    string[] cellTotalCompareWithInvestment = { };
                    switch (unitType.ToLower())
                    {
                        case "sub":
                            cellTotalCompareWithInvestment = new[] { "Xây dựng, sửa chữa lớn", "Mua sắm TSCĐ", "Mua sắm CCDC", "Đầu tư BĐS", "Tiền cho vay", "Đầu tư vốn vào ĐVTV" };
                            break;
                        case "in":
                            cellTotalCompareWithInvestment = new[] { "Xây dựng, sửa chữa lớn", "Mua sắm tài sản cố định", "Mua sắm công cụ, dụng cụ", "Đầu tư khác", "Tiền cho vay" };
                            break;
                        case "out":
                            cellTotalCompareWithInvestment = new[] { "Xây dựng, sửa chữa lớn", "Mua sắm TSCĐ", "Mua sắm CCDC", "Đầu tư khác", "Tiền cho vay" };
                            break;
                    }

                    var fInvestGroup = allCashFollowTypes.FirstOrDefault(c =>
                        c.Name.Equals("Chi cho hoạt động đầu tư", StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrEmpty(c.GroupFor));

                    // so sánh với kế hoạch đầu tư
                    foreach (var cellTotal in cellTotalCompareWithInvestment)
                    {
                        var fGroupData = allCashFollowTypes.FirstOrDefault(x =>
                            x.Name.StartsWith(cellTotal, StringComparison.CurrentCultureIgnoreCase) && x.ParentId == fInvestGroup?.Id);

                        var fromCashFollow = listDetails.FirstOrDefault(x => x.CashFollowGroupId == fGroupData?.Id);
                        var fInvestGroupData = investDataGroup.FirstOrDefault(x =>
                            x.InvestmentPlanContentName.StartsWith(cellTotal, StringComparison.CurrentCultureIgnoreCase));

                        if (fromCashFollow != null)
                        {
                            var fromInvestment = investmentData.FirstOrDefault(x =>
                                x.InvestmentPlanContentId == fInvestGroupData?.InvestmentPlanContentId);

                            if (fromInvestment == null)
                                listCellErrors.Add(new ExcelValidatorError { Message = $"Không tìm thấy dữ liệu <b>{cellTotal}</b> trong kế hoạch đầu tư" });
                            else if (fromInvestment.ExpectCostInvestment < fromCashFollow.Total)
                                listCellErrors.Add(new ExcelValidatorError { Message = $"Dữ liệu <b>{cellTotal}</b> của KH dòng tiền không được lớn hơn dữ liệu trên KH đầu tư" });
                        }
                    }

                    #endregion


                    string errorSignerPos = await _costStatusesRepository.ValidateSignerPositionInExcel(worksheet, GlobalEnums.CashFollow,
                        GlobalEnums.Year, unitType, sessionUser.Unit.UnitId);
                    if (!string.IsNullOrEmpty(errorSignerPos))
                        listCellErrors.Add(new ExcelValidatorError { Message = errorSignerPos });

                    response.Errors = listCellErrors;

                    var cFollow = new CashFollow
                    {
                        PathExcel = file.Replace(_physicalFolder, string.Empty).NormalizePath(),
                        Year = year
                    };

                    if (!response.IsValid)
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
                        var aggregateExcelErrors = _readAggregateExcels(cells, allCashFollowTypes, listAggregates);
                        if (aggregateExcelErrors?.Count == 0)
                            listData.CashFollowAggregateExcels = listAggregates;
                        else
                            listCellErrors.AddRange(aggregateExcelErrors);

                        #region code remmed

                        //else
                        //{
                        //    #region Tổng tiền thu trong kỳ

                        //    var gTotalRevenue = allCashFollowTypes.FirstOrDefault(x =>
                        //        x.Name.StartsWith("Tiền thu được trong kỳ", StringComparison.OrdinalIgnoreCase));

                        //    if (gTotalRevenue != null)
                        //    {
                        //        var oTotalRevenue =
                        //            listDetails.FirstOrDefault(x => x.CashFollowGroupId == gTotalRevenue.Id);

                        //        var aggRevenue = new CashFollowAggregateExcel
                        //        {
                        //            Q1 = Math.Round(oTotalRevenue?.M1 + oTotalRevenue?.M2 + oTotalRevenue?.M3 ?? 0),
                        //            Q2 = Math.Round(oTotalRevenue?.M4 + oTotalRevenue?.M5 + oTotalRevenue?.M6 ?? 0),
                        //            Q3 = Math.Round(oTotalRevenue?.M7 + oTotalRevenue?.M8 + oTotalRevenue?.M9 ?? 0),
                        //            Q4 = Math.Round(oTotalRevenue?.M10 + oTotalRevenue?.M11 + oTotalRevenue?.M12 ?? 0),
                        //            CashFollowGroupId = gTotalRevenue.Id,
                        //            CashFollowGroupName = gTotalRevenue.Name,
                        //            No = "1"
                        //        };

                        //        aggRevenue.Total = Math.Round(aggRevenue.Q1 + aggRevenue.Q2 + aggRevenue.Q3 + aggRevenue.Q4);
                        //        cFollow.TotalRevenue = aggRevenue.Total;
                        //        listAggregates.Add(aggRevenue);
                        //    }
                        //    else
                        //        listCellErrors.Add(new ExcelValidatorError { Message = "Không tìm thấy danh mục <b>Tiền thu được trong kỳ</b>!" });

                        //    #endregion

                        //    #region Tổng tiền chi trong kỳ
                        //    var gTotalSpending = allCashFollowTypes.FirstOrDefault(x =>
                        //        x.Name.StartsWith("Tiền chi trong kỳ", StringComparison.OrdinalIgnoreCase));

                        //    if (gTotalSpending != null)
                        //    {
                        //        var oTotalSpending =
                        //            listDetails.FirstOrDefault(x => x.CashFollowGroupId == gTotalSpending.Id);

                        //        var aggSpending = new CashFollowAggregateExcel
                        //        {
                        //            Q1 = Math.Round(oTotalSpending?.M1 + oTotalSpending?.M2 + oTotalSpending?.M3 ?? 0),
                        //            Q2 = Math.Round(oTotalSpending?.M4 + oTotalSpending?.M5 + oTotalSpending?.M6 ?? 0),
                        //            Q3 = Math.Round(oTotalSpending?.M7 + oTotalSpending?.M8 + oTotalSpending?.M9 ?? 0),
                        //            Q4 = Math.Round(oTotalSpending?.M10 + oTotalSpending?.M11 + oTotalSpending?.M12 ?? 0),
                        //            CashFollowGroupId = gTotalSpending.Id,
                        //            CashFollowGroupName = gTotalSpending.Name,
                        //            No = "2"
                        //        };

                        //        aggSpending.Total = Math.Round(aggSpending.Q1 + aggSpending.Q2 + aggSpending.Q3 + aggSpending.Q4);

                        //        listAggregates.Add(aggSpending);

                        //        var subsSpending = allCashFollowTypes.Where(x => x.ParentId == gTotalSpending.Id)
                        //            .ToList();
                        //        for (int i = 0; i < subsSpending.Count; i++)
                        //        {
                        //            var oTotalSubSpending = listDetails.FirstOrDefault(x => x.CashFollowGroupId == subsSpending[i].Id);
                        //            var aggSubSpending = new CashFollowAggregateExcel
                        //            {
                        //                Q1 = oTotalSubSpending?.M1 + oTotalSubSpending?.M2 + oTotalSubSpending?.M3 ?? 0,
                        //                Q2 = oTotalSubSpending?.M4 + oTotalSubSpending?.M5 + oTotalSubSpending?.M6 ?? 0,
                        //                Q3 = oTotalSubSpending?.M7 + oTotalSubSpending?.M8 + oTotalSubSpending?.M9 ?? 0,
                        //                Q4 = oTotalSubSpending?.M10 + oTotalSubSpending?.M11 + oTotalSubSpending?.M12 ?? 0,
                        //                CashFollowGroupId = subsSpending[i].Id,
                        //                CashFollowGroupName = subsSpending[i].Name,
                        //                No = $"2.{i + 1}"
                        //            };
                        //            aggSubSpending.Total = aggSubSpending.Q1 + aggSubSpending.Q2 + aggSubSpending.Q3 + aggSubSpending.Q4;
                        //            listAggregates.Add(aggSubSpending);
                        //        }

                        //        cFollow.TotalSpending = aggSpending.Total;
                        //    }
                        //    else
                        //        listCellErrors.Add(new ExcelValidatorError { Message = "Không tìm thấy danh mục <b>Tiền chi trong kỳ</b>!" });

                        //    #endregion

                        //    #region Lưu chuyển tiền thuần trong kỳ

                        //    var gTotalCirculation = allCashFollowTypes.FirstOrDefault(x =>
                        //        x.Name.StartsWith("Lưu chuyển tiền thuần trong kỳ", StringComparison.OrdinalIgnoreCase));

                        //    if (gTotalCirculation != null)
                        //    {
                        //        var oTotalCirculation =
                        //            listDetails.FirstOrDefault(x => x.CashFollowGroupId == gTotalCirculation.Id);

                        //        var aggCirculation = new CashFollowAggregateExcel
                        //        {
                        //            Q1 = Math.Round(oTotalCirculation?.M1 + oTotalCirculation?.M2 + oTotalCirculation?.M3 ?? 0),
                        //            Q2 = Math.Round(oTotalCirculation?.M4 + oTotalCirculation?.M5 + oTotalCirculation?.M6 ?? 0),
                        //            Q3 = Math.Round(oTotalCirculation?.M7 + oTotalCirculation?.M8 + oTotalCirculation?.M9 ?? 0),
                        //            Q4 = Math.Round(oTotalCirculation?.M10 + oTotalCirculation?.M11 + oTotalCirculation?.M12 ?? 0),
                        //            CashFollowGroupId = gTotalCirculation.Id,
                        //            CashFollowGroupName = gTotalCirculation.Name,
                        //            No = "3"
                        //        };

                        //        aggCirculation.Total = Math.Round(aggCirculation.Q1 + aggCirculation.Q2 + aggCirculation.Q3 + aggCirculation.Q4);

                        //        listAggregates.Add(aggCirculation);
                        //        cFollow.TotalCirculation = aggCirculation.Total;

                        //    }
                        //    else
                        //        listCellErrors.Add(new ExcelValidatorError { Message = "Không tìm thấy danh mục <b>Tiền thu được trong kỳ</b>!" });

                        //    #endregion

                        //    listData.CashFollowAggregateExcels = listAggregates;
                        //}

                        #endregion


                        listData.CashFollowItemExcels = listDetails;
                        listData.CashFollow = cFollow;
                    }
                    response.SpecifyFieldValue = listData;
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                }
                else
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }
            return Json(response);
        }

        public IList<ExcelValidatorError> _readAggregateExcels(Cells cells, IList<CashFollowGroup> allCashFollowTypes, IList<CashFollowAggregateExcel> cashFollowAggregateExcels)
        {
            try
            {
                bool matchHeaderAggregate = false;
                bool matchColHeaderAggregate = false;
                string sMatchHeaderAggregate = "I. Tổng hợp Kế hoạch dòng tiền";
                string sBreak = "II. Kế hoạch dòng tiền chi tiết";
                int rowCounter = 0;

                var excelValidator = new ExcelFormValidation(_configuration);
                excelValidator = excelValidator.Load("ExcelFormValidator:CashFollowAgg");
                Type oType = typeof(CashFollowAggregateExcel);
                var listCellErrors = new List<ExcelValidatorError>();
                IList<ExcelRow> excelRows = new List<ExcelRow>();

                foreach (Row cellsRow in cells.Rows)
                {
                    if (cellsRow.IsHidden)
                        continue;

                    #region Find header

                    #endregion

                    var fCell = cellsRow.FirstCell.Row;
                    var cellB = cellsRow.GetCellOrNull(1);
                    // khi nào gặp cell này thì mới đọc dữ liệu
                    if (cellB?.StringValue.Contains(sMatchHeaderAggregate, StringComparison.OrdinalIgnoreCase) == true)
                        matchHeaderAggregate = true;
                    else if (cellB?.StringValue.Contains("STT", StringComparison.OrdinalIgnoreCase) == true && matchHeaderAggregate)
                        matchColHeaderAggregate = true;

                    if (cellB?.StringValue.Contains(sBreak, StringComparison.OrdinalIgnoreCase) == true)
                        break;

                    

                    if (fCell < excelValidator.StartAtRow) { continue; }

                    if (!matchHeaderAggregate || !matchColHeaderAggregate)
                        continue;

                    rowCounter++;
                    bool header = rowCounter == 1;
                    var cellsInRow = new List<ExcelCell>();

                    for (int j = 0; j <= excelValidator.ColumnConfigs.Max(x => x.Position); j++)
                    {
                        var dataAt = cellsRow[j];
                        var xCellColumnValidator = excelValidator.ColumnConfigs.FirstOrDefault(x => x.Position == j);
                        // nếu cột <Nội dung> trống thì bỏ qua luôn row
                        if (!header && xCellColumnValidator?.FieldNameMapper == "CashFollowGroupName" &&
                            string.IsNullOrEmpty(dataAt.StringValue))
                            break;
                        if (xCellColumnValidator != null)
                        {
                            var cell = excelValidator.ReadCellAt(xCellColumnValidator, dataAt, header);
                            cell.CellPosition = j;

                            cellsInRow.Add(cell);
                        }
                    }
                    string cellContent = cellsInRow.FirstOrDefault(x => x.CellPosition == 2)?.StringCellValue;
                    if (!string.IsNullOrEmpty(cellContent))
                        excelRows.Add(new ExcelRow { Cells = cellsInRow, RowIndex = fCell });
                }

                foreach (var xcw in excelRows)
                {
                    var fCellCashGroup = xcw.Cells.FirstOrDefault(x => x.FieldMapper == "CashFollowGroupName");
                    var cashFollowGroup = allCashFollowTypes.FirstOrDefault(c =>
                        c.Name.Equals(fCellCashGroup?.ReaderVal?.ToString().Trim(), StringComparison.CurrentCultureIgnoreCase) && c.GroupFor?.Equals("agg") == true);

                    if (cashFollowGroup == null && fCellCashGroup != null)
                        // lỗi nhóm dữ liệu không hợp lệ
                        listCellErrors.Add(new ExcelValidatorError { Message = $"Nhóm dữ liệu <b>{fCellCashGroup.ReaderVal}</b> không hợp lệ" });
                    else if (cashFollowGroup != null)
                    {
                        xcw.GroupId = cashFollowGroup.Id;
                        xcw.GroupName = cashFollowGroup.Name;
                        xcw.Rules = cashFollowGroup.RulesCellOnRow;
                        xcw.SkipCellAts = cashFollowGroup.SkipCellAts;
                    }
                }


                RowCellsValidate rcValidate = new RowCellsValidate
                {
                    AllRows = excelRows
                };

                //bool isValid = rcValidate.ValidateRows();

                //if (isValid)
                //{
                foreach (var excelRow in excelRows)
                {
                    var instanceOf = Activator.CreateInstance(oType);
                    foreach (var excelRowCell in excelRow.Cells)
                    {
                        if (!string.IsNullOrEmpty(excelRowCell.FieldMapper) && excelRowCell.ReaderVal != null)
                        {
                            var field = oType.GetProperty(excelRowCell.FieldMapper, BindingFlags.Instance | BindingFlags.Public);
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

                    var oExcelRow = (CashFollowAggregateExcel)instanceOf;
                    oExcelRow.CashFollowGroupId = excelRow.GroupId;
                    oExcelRow.CashFollowGroupName = excelRow.GroupName;

                    cashFollowAggregateExcels.Add(oExcelRow);
                }
                //}

                var allRequireGroup = allCashFollowTypes.Where(c => c.IsRequire && c.GroupFor?.Equals("agg") == true).ToList();
                allRequireGroup.ForEach(c =>
                {
                    if (cashFollowAggregateExcels.All(x => x.CashFollowGroupId != c.Id))
                        listCellErrors.Add(new ExcelValidatorError { Message = $"Không tìm thấy nhóm dữ liệu <b>{c.Name}</b> trong biểu mẫu" });
                });

                return listCellErrors;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        /// <summary>
        /// Lưu dữ liệu biểu mẫu kế hoạch dòng tiền
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CashFollowAdd, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> OnCreate(CashFollowCreateRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                request.Creator = GetUserId();
                request.CreatorName = GetUserName();
                request.UnitId = GetUserUnit().UnitId;
                request.UnitName = GetUserUnit().UnitName;
                request.RequestPage = CashFollowConst.PublicKey;

                var validator = new CashFollowCreateValidator();
                var validatorResult = validator.Validate(request);
                if (!validatorResult.IsValid)
                {
                    return Json(new CreateCashFollowResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = "Dữ liệu không đúng định dạng!",
                        Errors = validatorResult.Errors.Select(x => x.ErrorMessage).ToList()
                    });
                }

                //tổng thu
                // tổng chi
                // luân chuyển

                #region code remmeed

                var fTotalRevenue = request.CashFollowAggregateExcels.FirstOrDefault(x =>
                    x.CashFollowGroupName.StartsWith("Tổng tiền thu trong kỳ",
                        StringComparison.CurrentCultureIgnoreCase));

                var fTotalSpending = request.CashFollowAggregateExcels.FirstOrDefault(x =>
                    x.CashFollowGroupName.StartsWith("Tổng tiền chi trong kỳ",
                        StringComparison.CurrentCultureIgnoreCase));

                var fCirculation = request.CashFollowAggregateExcels.FirstOrDefault(x =>
                    x.CashFollowGroupName.StartsWith("Lưu chuyển tiền thuần trong kỳ",
                        StringComparison.CurrentCultureIgnoreCase));
                int code;
                string msg;
                if (fTotalRevenue == null)
                {
                    code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    msg = "Không tìm thấy dữ liệu tổng tiền thu trong kỳ";
                    return Json(new EmptySearchResponseModel
                    { Code = code, Message = msg });
                }

                if (fTotalSpending == null)
                {
                    code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    msg = "Không tìm thấy dữ liệu tổng tiền chi trong kỳ";
                    return Json(new EmptySearchResponseModel
                    { Code = code, Message = msg });
                }

                if (fCirculation == null)
                {
                    code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    msg = "Không tìm thấy dữ liệu lưu chuyển tiền thuần trong kỳ";
                    return Json(new EmptySearchResponseModel
                    { Code = code, Message = msg });
                }

                request.CashFollow.TotalRevenue = fTotalRevenue.Total;
                request.CashFollow.TotalSpending = fTotalSpending.Total;
                request.CashFollow.TotalCirculation = fCirculation.Total;

                #endregion


                #region Di chuyển từ file tạm sang thư mục backup & lưu dữ liệu
                var physicalFile = request.CashFollow.PathExcel;
                if (!string.IsNullOrEmpty(physicalFile))
                {
                    try
                    {
                        string fullTempPath = $"{_physicalFolder}{physicalFile}";
                        var fileInfo = new FileInfo(fullTempPath);
                        string ownerFolder = GetUserSyncId().CreateDatePhysicalPathFileToUser();
                        string fullOwnerPath = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder, ownerFolder);
                        if (!Directory.Exists(fullOwnerPath))
                            Directory.CreateDirectory(fullOwnerPath);
                        string fullFilePath = Path.Combine(fullOwnerPath, fileInfo.Name);

                        if (System.IO.File.Exists(fullFilePath))
                            System.IO.File.Delete(fullFilePath);

                        System.IO.File.Copy(fullTempPath, fullFilePath);

                        var sign = new SignatureConnect();

                        string templateName = await _createPdfName(request.CashFollow.Year, sessionUser.Unit);
                        string pdf = sign.CreatePdf(fullFilePath, templateName, _physicalFolder, sessionUser.UserName);

                        if (string.IsNullOrWhiteSpace(pdf))
                            return Json(new EmptySearchResponseModel
                            {
                                Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                Message = "Không tạo được PDF!"
                            });
                        request.CashFollow.PathExcel = fullFilePath.Replace(_physicalFolder, string.Empty).NormalizePath();
                        request.CashFollow.PathPdf = pdf;
                        request.CashFollow.CashFollowType = GetUserUnit().UnitType;
                        request.IsSub = IsSubUnit();

                        var response = await _cashFollowRepository.CreateAsync(request);
                        if (response.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                        {
                            await _pdfLogsRepository.CreateAsync(new FilePdfCreateLogs
                            {
                                Type = CashFollowConst.PublicKey,
                                UnitId = sessionUser.Unit.UnitId,
                                CreatedDate = DateTime.Now,
                                FilePath = pdf
                            });
                        }
                        return Json(response);
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogLevel.Error, e, e.Message);
                        return Json(new EmptySearchResponseModel
                        { Code = (int)GlobalEnums.ResponseCodeEnum.Error, Message = "Lỗi hệ thống, vui lòng thử lại sau!" });
                    }
                }
                // sai định dạng 
                return Json(new EmptySearchResponseModel
                { Code = (int)GlobalEnums.ResponseCodeEnum.Error, Message = "Dữ liệu không đúng định dạng!" });
                #endregion
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return Json(new EmptySearchResponseModel
                { Code = (int)GlobalEnums.ResponseCodeEnum.Error, Message = GlobalEnums.ErrorMessage });
            }
        }
        [AuthorizeUser(Module = Functions.CashFollowView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> List()
        {
            var model = new ActuallyListModel
            {
                Stats = GlobalEnums.DefaultStatusSearchList,
                DefaultStats = Globs.DefaultSearchAllStats,
                DefaultStatsName = Globs.DefaultSearchAllStatsName,
                EnableSearchUnit = ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0, ComparingMode.GatherThan),
                Units = new List<SelectListItem>()
            };

            try
            {
                ViewBag.AddPermission = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CashFollowAdd, PermissionConstant.ADD);
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
        /// <summary>
        /// Tìm kiếm
        /// </summary>
        /// <param name="length"></param>
        /// <param name="start"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CashFollowView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, CashFollowSearchRequest @base)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();

                bool searchAllUnit = ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0, ComparingMode.GatherThan);
                if (!searchAllUnit)
                    @base.UserUnit = GetUserUnit().UnitId;

                @base.PageRequest = CashFollowConst.PublicKey;
                @base.HostFileView = _fileHostView;

                @base.PermissionDelete = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CashFollowView, PermissionConstant.DELETE);
                @base.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CashFollowView, PermissionConstant.EDIT);
                @base.PermissionApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CashFollowView, PermissionConstant.APPROVE);
                @base.IsSub = IsSubUnit();
                @base.UserUnitsManages = await _userRepository.GetUserUnitManages(sessionUser.UserId);

                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.CashFollow,
                    CostEstimateType = GlobalEnums.Year,
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

                var response = await _cashFollowRepository.SearchAsync(@base, start, length, sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut);
                response.Draw = @base.Draw;
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new CashFollowResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.NoContentMessage,
                    Draw = Request.Query["draw"].ToString().ToInt32()
                });
            }
        }

        /// <summary>
        /// Duyệt / Từ chối kế hoạch dòng tiền
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser(Module = Functions.CashFollowView, Permission = PermissionConstant.APPROVE)]

        public async Task<IActionResult> Approval(CashFollowApproveRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                //kiểm tra phân quyền
                request.PageRequest = CashFollowConst.PublicKey;
                request.UserId = sessionUser.UserId;
                request.UserName = sessionUser.UserName;
                request.HostFileView = _fileHostView;
                request.Positions = sessionUser.Positions;

                bool searchAllUnit = ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0,
                        ComparingMode.GatherThan);
                // từ kế toán trưởng MG trở lên 
                // thì có xem tất cả đơn vị
                request.UnitId = !searchAllUnit ? sessionUser.Unit.UnitId : (int)GlobalEnums.StatusDefaultEnum.All;

                request.PermissionApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CashFollowView, PermissionConstant.APPROVE);
                request.IsSub = IsSubUnit();

                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.CashFollow,
                    CostEstimateType = GlobalEnums.Year,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };
                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                // Không được xem dữ liệu nào
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new CostEstimateItemSearchResponse
                    {
                        Code = (int)HttpStatusCode.Forbidden,
                        Message = "Bạn không có quyền thực hiện thao tác này!"
                    });

                request.StatusAllowsSeen = listStatsForDepartmentGroup;
                request.UnitType = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut;

                var record = await _cashFollowRepository.GetRawById(request.RawId);
                var check = await _cashFollowRepository.CheckPermApproval(request, record);
                CashFollowApproveResponse response = new CashFollowApproveResponse();
                var sign = new SignatureConnect();

                if (check.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                {
                    // ký số
                    if (check.IsSignature)
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
                            TextFilter = check.Position.Name
                        };

                        var sigRt = sign.Signature(signOp);

                        if (sigRt.IsError)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            response.Message =
                                "Lỗi ký số, phê duyệt không thành công!";

                            return Json(response);
                        }
                        #endregion
                        record.PathPdf = sigRt.SignaturePath.Replace(signOp.PhysicalPath, string.Empty);
                    }
                    else
                    {
                        // nếu từ chối
                        // --> tạo lại PDF
                        // không tạo lại
                        if (!check.IsApproval)
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
                                userSignature, request.UserName, check.Position.Name);
                            if (sigRt.IsError)
                            {
                                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                response.Message = sigRt.Message;
                                return Json(response);
                            }

                            record.PathPdf = sigRt.SignaturePath.Replace(_physicalFolder, string.Empty).NormalizePath();
                        }
                    }
                    response = await _cashFollowRepository.Approval(request, record, check);
                }
                else
                    response = new CashFollowApproveResponse
                    {
                        Code = check.Code,
                        Message = check.Message
                    };
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new CashFollowApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }

        }
        /// <summary>
        /// Xem lịch sử
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        //[AuthorizeUser(Module = Functions.CashFollowView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> ViewHistories(string record)
        {
            var request = new CashFollowApproveRequest
            {
                Record = record,
                PageRequest = CashFollowConst.PublicKey
            };
            var data = await _cashFollowRepository.ViewHistories(request);
            return PartialView("_ViewHistory", data);
        }

        /// <summary>
        /// So sánh dòng tiền & thực chi
        /// </summary>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CashFollowView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> CompareCashFollowActuallySpent(string record)
        {
            var model = new CompareCFAndActuallyModel();
            if (!string.IsNullOrEmpty(record))
            {
                var mo = new CashFollowGetByIdRequest { Record = record, RequestPage = CashFollowConst.PublicKey };
                var dataById = await _cashFollowRepository.GetRawById(mo.RawId);
                if (dataById != null)
                {
                    model.Data = dataById;
                    model.Record = record;
                }
            }
            return View(model);
        }

        [HttpPost]
        [AuthorizeUser(Module = Functions.CashFollowView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Comparing(CompareCFAndActuallyRequest request)
        {
            if (!string.IsNullOrEmpty(request.Record))
            {
                var sessionUser = GetUsersSessionModel();
                request.RequestPage = CashFollowConst.PublicKey;
                request.UnitType = GetUserUnit().UnitType;


                var allStats = await _costStatusesRepository.GetAll();
                var maxFollowValue = allStats.Where(x => x.IsApprove == 1 &&
                                                         x.StatusForCostEstimateType.Equals(GlobalEnums.Week)
                                                         && x.StatusForSubject.Equals(IsSubUnit()
                                                             ? GlobalEnums.ObjectSub
                                                             : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut) &&
                                                         x.Type.Equals(GlobalEnums.ActuallySpent)).Max(x => x.Value);
                request.MaxFollowValue = maxFollowValue;
                var response = await _cashFollowRepository.ComparingAsync(request);
                return Json(response);
            }
            return Json(new EmptySearchResponseModel { Code = (int)GlobalEnums.ResponseCodeEnum.NoContent, Message = "Dữ liệu không đúng định dạng!" });
        }

        [AuthorizeUser(Module = Functions.CashFollowView, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> OnDelete(CashFollowApproveRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                var position = GetUserPosition();
                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.CashFollow,
                    CostEstimateType = GlobalEnums.Year,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };

                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new CashFollowApproveResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });


                request.PageRequest = CashFollowConst.PublicKey;

                request.Positions = position;
                request.UserId = GetUserId();
                request.UserName = GetUserName();
                request.StatusAllowsSeen = listStatsForDepartmentGroup;
                request.IsSub = IsSubUnit();

                var response = await _cashFollowRepository.Delete(request);
                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return Json(new CashFollowApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }

        public async Task<string> _createPdfName(int year, UnitModel unit)
        {
            try
            {
                int fileCounter =
                    await _pdfLogsRepository.CounterDay(unit.UnitId, CashFollowConst.PublicKey);
                string templateName =
                    $"{DateTime.Now:yyyy-MM-dd}_{year}_{unit.UnitName.StringToNonUnicode().StringRemoveSpecial()}_Ke hoach dong tien_{fileCounter}.pdf";
                return templateName;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return string.Empty;
            }
        }
    }
}
