using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using GPLX.Core.DTO.Request.RevenuePlan;
using GPLX.Core.DTO.Request.Signature;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.CashFollow;
using GPLX.Core.DTO.Response.CostEstimate;
using GPLX.Core.DTO.Response.RevenuePlan;
using GPLX.Core.Enum;
using GPLX.Database.Models;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Signature;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;
using GPLX.Web.Models.Revenue;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GPLX.Web.Controllers
{
    public class RevenuePlanController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRevenuePlanRepository _revenuePlanRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPdfLogsRepository _pdfLogsRepository;
        private readonly IProfitPlanRepository _profitPlanRepository;
        private readonly string _defaultRootFolder;
        private readonly string _fileHostView;
        private readonly string _physicalFolder;
        public RevenuePlanController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IRevenuePlanRepository revenuePlanRepository, IGroupsRepository groupsRepository, ICostStatusesRepository costStatusesRepository, IUnitRepository unitRepository, IUserRepository userRepository, IPdfLogsRepository pdfLogsRepository, IProfitPlanRepository profitPlanRepository)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _revenuePlanRepository = revenuePlanRepository;
            _groupsRepository = groupsRepository;
            _costStatusesRepository = costStatusesRepository;
            _unitRepository = unitRepository;
            _userRepository = userRepository;
            _pdfLogsRepository = pdfLogsRepository;
            _profitPlanRepository = profitPlanRepository;
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");
            _fileHostView = configuration.GetValue<string>("FileHosting");

            _physicalFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder);
        }
        [AuthorizeUser(Module = Functions.RevenuePlanAdd, Permission = PermissionConstant.VIEW)]

        public IActionResult Create()
        {
            ViewBag.UnitType = IsSubUnit() ? "sub" : GetUserUnit().UnitType.ToLower();
            return View();
        }
        [AuthorizeUser(Module = Functions.RevenuePlanView, Permission = PermissionConstant.VIEW)]
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
                    Functions.RevenuePlanAdd, PermissionConstant.ADD);

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

        [AuthorizeUser(Module = Functions.RevenuePlanAdd, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnExcelUpload([FromQuery(Name = "year")] int selectedFinanceYear)
        {
            var excelUploadResponse = new ExcelUploadResponse<RevenuePlanAggregate>();

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

            var listData = new RevenuePlanExcelUploadResponse();
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

                    if (sheetNamesMatch.FirstOrDefault(m => !m.Name.Contains("Hướng dẫn", StringComparison.OrdinalIgnoreCase)) == null)
                    {
                        excelUploadResponse.AddError(new ExcelValidatorError { Message = "Không có sheet dữ liệu yêu cầu!" });
                        excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        return Json(excelUploadResponse);
                    }


                    if (excelUploadResponse.IsValid)
                    {
                        var ws = sheetNamesMatch[0];
                        var cells = ws.Cells;
                        // năm lập báo cáo
                        var year = cells["D8"].StringValue.ToInt32();
                        if (selectedFinanceYear != year)
                        {
                            excelUploadResponse.AddError(new ExcelValidatorError { Message = $"Cần đẩy sheet dữ liệu theo đúng năm tài chính đã chọn {selectedFinanceYear}. Năm tài chính của sheet là {year}!" });
                            excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            return Json(excelUploadResponse);
                        }
                        string unitType = IsSubUnit() ? "sub" : sessionUser.Unit.UnitType;
                        var revenueContents = await _revenuePlanRepository.ListRevenuePlanContents(unitType);
                        //
                        var excelValidator = new ExcelFormValidation(_configuration);
                        excelValidator = excelValidator.Load("ExcelFormValidator:RevenuePlanIn");
                        var listCustomer = new List<RevenuePlanCustomerDetails>();
                        var listCash = new List<RevenuePlanCashDetails>();
                        Type oType = null;
                        object instanceOf = null;

                        bool matchHeader = false;
                        bool matchColHeader = false;
                        string sMatchHeader = sessionUser.Unit.UnitType == GlobalEnums.UnitIn ? "II. Kế hoạch Doanh thu" : "II. Chi tiết Kế hoạch Doanh thu";
                        IList<ExcelRow> excelRows = new List<ExcelRow>();
                        int rowCounter = 0;

                        foreach (Row cellsRow in cells.Rows)
                        {
                            if (cellsRow.IsHidden)
                                continue;
                            var fCell = cellsRow.FirstCell.Row;
                            var cellB = cellsRow.GetCellOrNull(1);
                            // khi nào gặp cell này thì mới đọc dữ liệu
                            if (cellB?.StringValue.Contains(sMatchHeader, StringComparison.OrdinalIgnoreCase) == true)
                                matchHeader = true;
                            else if (cellB?.StringValue.Contains("STT", StringComparison.OrdinalIgnoreCase) == true && matchHeader)
                                matchColHeader = true;
                            if (oType != null)
                                instanceOf = Activator.CreateInstance(oType);
                            var cellsInRow = new List<ExcelCell>();
                            if (fCell < excelValidator.StartAtRow) { continue; }
                            if (!matchHeader || !matchColHeader) { continue; }

                            rowCounter++;
                            bool header = rowCounter == 1;

                            for (int j = 0; j <= excelValidator.ColumnConfigs.Max(x => x.Position); j++)
                            {
                                var dataAt = cellsRow[j];
                                var xCellColumnValidator = excelValidator.ColumnConfigs.FirstOrDefault(x => x.Position == j);

                                if (xCellColumnValidator != null && header)
                                {
                                    var cellValid = excelValidator.ValidCell(xCellColumnValidator, dataAt.StringValue, true, instanceOf, typeof(RevenuePlanCustomerDetails), dataAt);
                                    if (!cellValid.Item1)
                                        excelValidator.Errors.Add(new ExcelValidatorError { Column = dataAt.Name, Message = cellValid.Item2 });
                                }
                                else if (xCellColumnValidator != null && fCell > excelValidator.StartAtRow)
                                {
                                    // cột nội dung
                                    var contentCell = dataAt.Name;//.StartsWith("C", StringComparison.OrdinalIgnoreCase);
                                    if (contentCell.StartsWith("C", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (dataAt.StringValue.Equals("Số lượng khách hàng", StringComparison.OrdinalIgnoreCase))
                                        {
                                            oType = typeof(RevenuePlanCustomerDetails);
                                            instanceOf = Activator.CreateInstance(oType);
                                        }
                                        else if (dataAt.StringValue.Equals("Tổng doanh thu", StringComparison.OrdinalIgnoreCase))
                                        {
                                            oType = typeof(RevenuePlanCashDetails);
                                            instanceOf = Activator.CreateInstance(oType);
                                        }
                                        //else if (dataAt.StringValue.StartsWith("Doanh thu theo …", StringComparison.CurrentCultureIgnoreCase))
                                        //    break;
                                        else if (dataAt.StringValue.Equals("…", StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrWhiteSpace(dataAt.StringValue))
                                            break;
                                    }
                                    //bỏ qua dòng "<< Đơn vị điền phân loại nhóm doanh thu (VD: theo nhóm sản phẩm, nhóm dịch vụ, …)"
                                    else if (contentCell.StartsWith("D", StringComparison.OrdinalIgnoreCase))
                                        if (dataAt.StringValue.StartsWith("<<", StringComparison.CurrentCultureIgnoreCase))
                                            break;

                                    var cell = excelValidator.ReadCellAt(xCellColumnValidator, dataAt, header);
                                    cell.CellPosition = j;
                                    cellsInRow.Add(cell);


                                    if (instanceOf != null && oType != null)
                                    {
                                        var cellValid = excelValidator.ValidCell(xCellColumnValidator, dataAt.StringValue, false, instanceOf, oType, dataAt);
                                        if (!cellValid.Item1)
                                            excelValidator.Errors.Add(new ExcelValidatorError { Column = dataAt.Name, Message = cellValid.Item2 });
                                    }
                                }
                            }

                            var fCellCashGroup = cellsInRow.FirstOrDefault(x => x.FieldMapper == "RevenuePlanContentName");
                            var revenueGroup = revenueContents.FirstOrDefault(x =>
                                x.RevenuePlanContentName.Equals(fCellCashGroup?.ReaderVal?.ToString(),
                                    StringComparison.CurrentCultureIgnoreCase));
                            if (sessionUser.Unit.UnitType == GlobalEnums.UnitOut)
                            {
                                if (oType == typeof(RevenuePlanCustomerDetails))
                                {
                                    var oAdd = (RevenuePlanCustomerDetails)instanceOf;
                                    if (!string.IsNullOrEmpty(oAdd?.RevenuePlanContentName))
                                    {
                                        if (revenueGroup != null)
                                            oAdd.RevenuePlanContentId = revenueGroup.RevenuePlanContentId;

                                        oAdd.No = cellB.StringValue;
                                        listCustomer.Add(oAdd);
                                    }

                                }
                                else if (oType == typeof(RevenuePlanCashDetails))
                                {
                                    var oAdd = (RevenuePlanCashDetails)instanceOf;
                                    if (!string.IsNullOrEmpty(oAdd?.RevenuePlanContentName))
                                    {
                                        if (revenueGroup != null)
                                            oAdd.RevenuePlanContentId = revenueGroup.RevenuePlanContentId;
                                        oAdd.No = cellB.StringValue;
                                        listCash.Add(oAdd);
                                    }
                                }
                            }
                            else
                            {
                                if (oType == typeof(RevenuePlanCustomerDetails))
                                {
                                    var oAdd = (RevenuePlanCustomerDetails)instanceOf;

                                    oAdd.RevenuePlanContentId = revenueGroup?.RevenuePlanContentId ?? 0;
                                    if (!string.IsNullOrEmpty(oAdd?.RevenuePlanContentName))
                                    {
                                        oAdd.No = cellB.StringValue;
                                        listCustomer.Add(oAdd);
                                    }
                                }
                                else if (oType == typeof(RevenuePlanCashDetails))
                                {
                                    var oAdd = (RevenuePlanCashDetails)instanceOf;
                                    oAdd.RevenuePlanContentId = revenueGroup?.RevenuePlanContentId ?? 0;
                                    if (!string.IsNullOrEmpty(oAdd?.RevenuePlanContentName))
                                    {
                                        oAdd.No = cellB.StringValue;
                                        listCash.Add(oAdd);
                                    }
                                }
                            }
                            // Số lượng khách hàng
                            instanceOf = null;

                            string cellContent = cellsInRow.FirstOrDefault(x => x.CellPosition == 2)?.StringCellValue;
                            if (!string.IsNullOrEmpty(cellContent))
                                excelRows.Add(new ExcelRow
                                {
                                    Cells = cellsInRow,
                                    RowIndex = fCell,
                                    Rules = "require(3:15);minEqual(3:15[0])",
                                    GroupId = revenueGroup?.RevenuePlanContentId ?? 0,
                                    GroupName = revenueGroup?.RevenuePlanContentName
                                });
                        }

                        // kiểm tra sau dòng <Tổng doanh thu> bắt buộc phải có các mục nhỏ
                        if (IsSubUnit(sessionUser))
                        {
                            var listRevenueGroup = revenueContents.Where(c =>
                                c.GroupFor.Equals("cash", StringComparison.CurrentCultureIgnoreCase)).ToList();
                            if (listCash.All(c =>
                                listRevenueGroup.All(m => m.RevenuePlanContentId != c.RevenuePlanContentId)))
                            {
                                excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Không tìm thấy dữ liệu doanh thu chi tiết trong tệp đã chọn!" });
                                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                                return Json(excelUploadResponse);
                            }
                        }
                        else if (listCash.All(c => c.RevenuePlanContentId != 0))
                        {
                            excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Không tìm thấy dữ liệu doanh thu chi tiết trong tệp đã chọn!" });
                            excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            return Json(excelUploadResponse);
                        }
                        if (!excelRows.Any())
                        {
                            excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Không tìm thấy dữ liệu trong tệp đã chọn!" });
                            excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            return Json(excelUploadResponse);
                        }

                        RowCellsValidate rcValidate = new RowCellsValidate
                        {
                            AllRows = excelRows
                        };

                        bool isValid = rcValidate.ValidateRows();
                        if (!isValid || rcValidate.AllRows.Any(c => c.Cells.Any(m => m.IsNotValidCell)))
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


                        var allRequireGroup = revenueContents.Where(c => c.IsRequire).ToList();
                        allRequireGroup.ForEach(c =>
                        {
                            if (excelRows.All(x => x.GroupId != c.RevenuePlanContentId))
                                excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = $"Không tìm thấy nhóm dữ liệu <b>{c.RevenuePlanContentName}</b> trong biểu mẫu" });
                        });

                        var listErrors = new List<ExcelValidatorError>();
                        var listAggregates = _readAggregateExcels(cells, revenueContents, sessionUser.Unit.UnitType, listErrors);

                        string errorSignerPos = await _costStatusesRepository.ValidateSignerPositionInExcel(ws, GlobalEnums.Revenue,
                            GlobalEnums.Year, unitType, sessionUser.Unit.UnitId);
                        if (!string.IsNullOrEmpty(errorSignerPos))
                            listErrors.Add(new ExcelValidatorError { Message = errorSignerPos });
                        excelUploadResponse.Errors.AddRange(listErrors);


                        if (IsSubUnit(sessionUser))
                        {
                            var aggTotal = listAggregates.FirstOrDefault(c =>
                                c.RevenuePlanContentContent.StartsWith("Tổng",
                                    StringComparison.CurrentCultureIgnoreCase));
                            if (aggTotal != null)
                            {
                                var detailCustomerSum = listCustomer.FirstOrDefault()?.Total;
                                if (detailCustomerSum != aggTotal.NumberCustomers)
                                    excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Tổng SL Khách hàng không khớp so với bảng chi tiết!" });

                                var cashSum = listCash.Where(c => allRequireGroup.All(m => m.RevenuePlanContentId != c.RevenuePlanContentId)).Sum(c => c.Total);
                                if (cashSum != aggTotal.ExpectRevenue)
                                    excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Tổng DT năm dự kiến không khớp so với bảng chi tiết!" });

                            }
                        }
                        // nếu là đv y tế
                        else if (sessionUser.Unit.UnitType == GlobalEnums.UnitIn)
                        {
                            //Doanh thu theo đơn vị
                            var expectOfUnit = listAggregates.FirstOrDefault(x =>
                                x.RevenuePlanContentContent.Contains("Doanh thu và khách hàng theo đơn vị",
                                    StringComparison.CurrentCultureIgnoreCase));
                            // DOanh thu theo địa bản
                            var expectOfLocate = listAggregates.FirstOrDefault(x =>
                                x.RevenuePlanContentContent.Contains("Doanh thu và khách hàng theo địa bàn",
                                    StringComparison.CurrentCultureIgnoreCase));

                            if (expectOfUnit == null)
                                excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Không tìm thấy dữ liệu Doanh thu và khách hàng theo đơn vị!" });


                            if (expectOfLocate != null)
                            {
                                if (expectOfLocate?.ExpectRevenue != expectOfUnit?.ExpectRevenue)
                                    excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Doanh thu và khách hàng theo đơn vị và theo địa bàn phải giống nhau!" });

                                if (expectOfLocate?.NumberCustomers != expectOfUnit?.NumberCustomers)
                                    excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Số lượng khách hàng theo đơn vị và theo địa bàn phải giống nhau!" });
                            }
                        }
                        else
                        {
                            //Tổng cộng
                            var aggTotal = listAggregates.FirstOrDefault(c =>
                                c.RevenuePlanContentContent.StartsWith("Tổng",
                                    StringComparison.CurrentCultureIgnoreCase));
                            if (aggTotal != null)
                            {
                                var detailCustomerSum = listCustomer.FirstOrDefault()?.Total;
                                if (detailCustomerSum != aggTotal.NumberCustomers)
                                    excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Tổng SL Khách hàng không khớp so với bảng chi tiết!" });
                                var cashSum = listCash.Where(c => c.RevenuePlanContentId != 0).Sum(c => c.Total);
                                if (cashSum != aggTotal.ExpectRevenue)
                                    excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = "Tổng DT năm dự kiến không khớp so với bảng chi tiết!" });
                            }
                        }

                        #region Code rem
                        // nếu là đv y tế
                        //if (sessionUser.Unit.UnitType == GlobalEnums.UnitIn)
                        //{
                        //    var listAggregateUnitCriteria = new List<RevenuePlanAggregate>();
                        //    var listAggregateLocateCriteria = new List<RevenuePlanAggregate>();

                        //    #region Tổng hợp số lượng khách hàng - doanh thu theo đơn vị
                        //    //int indexOfPerUnitCustomer = -1;
                        //    int indexOfPerLocateCustomer = -1;
                        //    foreach (var c in listCustomer)
                        //    {

                        //        if (c.RevenuePlanContentName.StartsWith("SL khách hàng theo địa bàn"))
                        //            indexOfPerLocateCustomer = listCustomer.IndexOf(c);
                        //    }

                        //    foreach (var rc in revenueContents.OrderBy(x => x.Order))
                        //    {
                        //        var criteria = listCustomer.FirstOrDefault(x =>
                        //            x.RevenuePlanContentId == rc.RevenuePlanContentId);

                        //        var revenueCash = listCash.FirstOrDefault(x =>
                        //            x.RevenuePlanContentId == rc.RevenuePlanContentId);
                        //        var revenueTotal = revenueCash?.Total ?? 0;
                        //        listAggregateUnitCriteria.Add(new RevenuePlanAggregate
                        //        {
                        //            RevenuePlanContentId = rc.RevenuePlanContentId,
                        //            RevenuePlanContentContent = rc.RevenuePlanContentName,
                        //            NumberCustomers = criteria?.Total ?? 0,
                        //            ExpectRevenue = Math.Round(revenueTotal),
                        //        });
                        //        // nếu cần tính lại
                        //        // criteria.M1 + criteria.M2 + criteria.M3 + criteria.M4 + criteria.M5 + criteria.M6 + criteria.M7 + criteria.M8 + criteria.M9 + criteria.M10 + criteria.M11 + criteria.M12
                        //    }

                        //    // dòng tổng
                        //    // Doanh thu và Khách hàng theo đơn vị

                        //    var aggregateUnit = new RevenuePlanAggregate
                        //    {
                        //        RevenuePlanContentContent = "Doanh thu và Khách hàng theo đơn vị",
                        //        RevenuePlanContentId = -100,
                        //        ExpectRevenue = Math.Round(listAggregateUnitCriteria.Sum(x => x.ExpectRevenue)),
                        //        NumberCustomers = Math.Round(listAggregateUnitCriteria.Sum(x => x.NumberCustomers)),
                        //    };
                        //    listAggregateUnitCriteria.ForEach(x =>
                        //        {
                        //            x.ProportionCustomer = aggregateUnit.NumberCustomers > 0 ?
                        //                Math.Round(x.NumberCustomers / aggregateUnit.NumberCustomers * 100) : 0;
                        //            x.ProportionRevenue = aggregateUnit.ExpectRevenue > 0 ? Math.Round(x.ExpectRevenue / aggregateUnit.ExpectRevenue * 100) : 0;
                        //        });

                        //    listAggregates.Add(aggregateUnit);
                        //    listAggregates.AddRange(listAggregateUnitCriteria);
                        //    #endregion

                        //    #region Tổng hợp theo địa bàn
                        //    // Chỉ lấy từ dòng doanh thu theo địa bàn trở đi
                        //    var skipData = listCustomer.Skip(indexOfPerLocateCustomer + 1);
                        //    foreach (var sd in skipData)
                        //    {
                        //        listAggregateLocateCriteria.Add(new RevenuePlanAggregate
                        //        {
                        //            RevenuePlanContentId = -1,
                        //            RevenuePlanContentContent = sd.RevenuePlanContentName,
                        //            NumberCustomers = sd.Total,
                        //            ExpectRevenue = Math.Round(listCash.FirstOrDefault(x => x.RevenuePlanContentName.Equals(sd.RevenuePlanContentName, StringComparison.OrdinalIgnoreCase))?.Total ?? 0)
                        //        });
                        //    }

                        //    var aggregateLocate = new RevenuePlanAggregate
                        //    {
                        //        RevenuePlanContentContent = "Doanh thu và Khách hàng theo địa bàn",
                        //        RevenuePlanContentId = -100,
                        //        ExpectRevenue = Math.Round(listAggregateLocateCriteria.Sum(x => x.ExpectRevenue)),
                        //        NumberCustomers = Math.Round(listAggregateLocateCriteria.Sum(x => x.NumberCustomers)),
                        //    };
                        //    listAggregateLocateCriteria.ForEach(x =>
                        //    {
                        //        x.ProportionCustomer = aggregateLocate.NumberCustomers > 0 ?
                        //            Math.Round(x.NumberCustomers / aggregateLocate.NumberCustomers * 100) : 0;
                        //        x.ProportionRevenue = aggregateLocate.ExpectRevenue > 0 ? Math.Round(x.ExpectRevenue / aggregateLocate.ExpectRevenue * 100) : 0;
                        //    });

                        //    listAggregates.Add(aggregateLocate);
                        //    listAggregates.AddRange(listAggregateLocateCriteria);
                        //    #endregion
                        //}
                        //else
                        //{
                        //    foreach (var lc in listCash.Where(x => !x.RevenuePlanContentName.Equals("Tổng doanh thu", StringComparison.OrdinalIgnoreCase)))
                        //    {
                        //        listAggregates.Add(new RevenuePlanAggregate
                        //        {
                        //            RevenuePlanContentContent = lc.RevenuePlanContentName,
                        //            NumberCustomers = 0.0,
                        //            ExpectRevenue = lc.Total,
                        //            ProportionCustomer = Math.Round(listCustomer.Sum(x => x.Total))
                        //        });
                        //    }

                        //    var aggregateSum = new RevenuePlanAggregate
                        //    {
                        //        NumberCustomers = Math.Round(listCustomer.Sum(x => x.Total)),
                        //        ExpectRevenue = Math.Round(listAggregates.Sum(x => x.ExpectRevenue)),
                        //        RevenuePlanContentContent = "Tổng"
                        //    };

                        //    listAggregates.ForEach(x =>
                        //    {
                        //        x.ProportionRevenue = aggregateSum.ExpectRevenue > 0 ? Math.Round(x.ExpectRevenue / aggregateSum.ExpectRevenue * 100) : 0;
                        //    });

                        //}
                        #endregion


                        listData.RevenuePlanAggregates = listAggregates;
                        listData.RevenuePlanCustomers = listCustomer;
                        listData.RevenuePlanCash = listCash;
                        listData.RevenuePlan = new RevenuePlan
                        {
                            Year = year,
                            RevenuePlanType = GetUserUnit().UnitType,
                            UnitName = sessionUser.Unit.UnitName,
                            PathExcel = file.Replace(_physicalFolder, string.Empty),
                            UnitId = sessionUser.Unit.UnitId,
                            IsSub = IsSubUnit(sessionUser)
                        };

                        excelUploadResponse.SpecifyFieldValue = listData;

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
                    }


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

        public List<RevenuePlanAggregate> _readAggregateExcels(Cells cells, IList<RevenuePlanContents> allRevenuePlanContents, string type, List<ExcelValidatorError> listCellErrors)
        {
            try
            {
                bool matchHeaderAggregate = false;
                bool matchColHeaderAggregate = false;
                string sMatchHeaderAggregate = "I. Tổng hợp Kế hoạch Doanh thu - khách hàng";
                string sBreak = "II.";
                int rowCounter = 0;
                var excelValidator = new ExcelFormValidation(_configuration);
                excelValidator = excelValidator.Load($"ExcelFormValidator:RevenuePlanAggregate{type}");
                Type oType = typeof(RevenuePlanAggregate);

                var listAggregates = new List<RevenuePlanAggregate>();
                foreach (Row cellsRow in cells.Rows)
                {
                    if (cellsRow.IsHidden)
                        continue;
                    // bắt đầu từ dòng 7
                    var fCell = cellsRow.FirstCell.Row;
                    var cellB = cellsRow.GetCellOrNull(1);
                    // khi nào gặp cell này thì mới đọc dữ liệu
                    if (cellB?.StringValue.Contains(sMatchHeaderAggregate, StringComparison.OrdinalIgnoreCase) == true)
                        matchHeaderAggregate = true;
                    else if (cellB?.StringValue.Contains("STT", StringComparison.OrdinalIgnoreCase) == true && matchHeaderAggregate)
                        matchColHeaderAggregate = true;
                    //|| cellB?.StringValue.StartsWith("Tổng", StringComparison.OrdinalIgnoreCase) == true
                    if (cellB?.StringValue.Contains(sBreak, StringComparison.OrdinalIgnoreCase) == true)
                        break;
                    var instanceOf = Activator.CreateInstance(oType);

                    if (fCell < excelValidator.StartAtRow) { continue; }

                    if (!matchHeaderAggregate || !matchColHeaderAggregate)
                        continue;

                    rowCounter++;
                    bool header = rowCounter == 1;
                    for (int j = 0; j <= excelValidator.ColumnConfigs.Max(x => x.Position); j++)
                    {
                        var dataAt = cellsRow[j];
                        var xCellColumnValidator = excelValidator.ColumnConfigs.FirstOrDefault(x => x.Position == j);
                        // nếu cột <Nội dung> trống thì bỏ qua luôn row
                        if (!header && xCellColumnValidator?.FieldNameMapper == "CashFollowGroupName" &&
                            string.IsNullOrEmpty(dataAt.StringValue))
                            break;
                        var contentCell = dataAt.Name.StartsWith("C", StringComparison.OrdinalIgnoreCase);
                        if (contentCell)
                            if (dataAt.StringValue.Equals("…", StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrWhiteSpace(dataAt.StringValue))
                                break;

                        if (xCellColumnValidator != null && header)
                        {
                            var cellValid = excelValidator.ValidCell(xCellColumnValidator, dataAt.StringValue, true, instanceOf,
                                typeof(RevenuePlanAggregate), dataAt);
                            if (!cellValid.Item1)
                                listCellErrors.Add(new ExcelValidatorError { Column = dataAt.Name, Message = cellValid.Item2 });
                        }
                        else if (xCellColumnValidator != null && fCell > excelValidator.StartAtRow)
                        {
                            if (instanceOf != null)
                            {
                                var cellValid = excelValidator.ValidCell(xCellColumnValidator, dataAt.StringValue, false, instanceOf, oType, dataAt);
                                if (!cellValid.Item1)
                                    listCellErrors.Add(new ExcelValidatorError { Column = dataAt.Name, Message = cellValid.Item2 });
                            }
                        }
                    }

                    if (oType == typeof(RevenuePlanAggregate))
                    {
                        var oAdd = (RevenuePlanAggregate)instanceOf;
                        var oContent = allRevenuePlanContents.FirstOrDefault(x =>
                            oAdd?.RevenuePlanContentContent?.Trim().StartsWith(x.RevenuePlanContentName, StringComparison.OrdinalIgnoreCase) == true);

                        oAdd.RevenuePlanContentId = oContent?.RevenuePlanContentId ?? -1;
                        if (!string.IsNullOrEmpty(oAdd.RevenuePlanContentContent))
                            listAggregates.Add(oAdd);
                    }
                }

                return listAggregates;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        [AuthorizeUser(Module = Functions.RevenuePlanAdd, Permission = PermissionConstant.ADD)]
        public async Task<IActionResult> OnCreate(RevenuePlanCreateRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                request.UnitName = sessionUser.Unit.UnitName;
                request.UnitId = sessionUser.Unit.UnitId;
                request.Creator = sessionUser.UserId;
                request.CreatorName = sessionUser.UserName;
                request.IsSub = IsSubUnit(sessionUser); // SUB không có kế hoạch doanh thu KH

                #region Tạo PDF

                try
                {
                    var folder = _physicalFolder;
                    var fExcelFile = $"{folder}{request.RevenuePlan.PathExcel}";
                    if (System.IO.File.Exists(fExcelFile))
                    {
                        var ownerFolder = sessionUser.UserName.CreateDatePhysicalPathFileToUser();
                        var fullOwnerFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder,
                            ownerFolder);
                        if (!Directory.Exists(fullOwnerFolder))
                            Directory.CreateDirectory(fullOwnerFolder);
                        var fullOwnerFile = Path.Combine(fullOwnerFolder, $"{Path.GetRandomFileName()}.xlsx");
                        //System.IO.File.Move(fExcelFile, fullOwnerFile);
                        System.IO.File.Copy(fExcelFile, fullOwnerFile);

                        var sign = new SignatureConnect();
                        string template = await _createPdfName(request.RevenuePlan.Year, sessionUser.Unit);
                        string pdf = sign.CreatePdf(fullOwnerFile, template, _physicalFolder, sessionUser.UserName);

                        if (string.IsNullOrWhiteSpace(pdf))
                            return Json(new RevenuePlanCreateResponse
                            {
                                Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                Message = "Không tạo được PDF!"
                            });

                        request.RevenuePlan.PathPdf = pdf;
                        request.RevenuePlan.PathExcel = fullOwnerFile.Replace(_physicalFolder, string.Empty).NormalizePath();
                    }
                    else
                    {
                        return Json(new RevenuePlanCreateResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            Message = "Không tìm thấy dữ liệu gốc!"
                        });
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "{0}", e.Message);
                    return Json(new RevenuePlanCreateResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = GlobalEnums.ErrorMessage
                    });
                }
                #endregion

                var response = await _revenuePlanRepository.Create(request);
                if (response.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                {
                    await _pdfLogsRepository.CreateAsync(new FilePdfCreateLogs
                    {
                        Type = RevenuePlanConst.PublicKey,
                        UnitId = sessionUser.Unit.UnitId,
                        CreatedDate = DateTime.Now,
                        FilePath = request.RevenuePlan.PathPdf
                    });
                }

                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "request {0}", request);
                return Json(new RevenuePlanCreateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }


        [AuthorizeUser(Module = Functions.RevenuePlanView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, SearchRevenuePlanRequest @base)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                @base.PageRequest = RevenuePlanConst.PublicKey;
                @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();
                @base.PermissionApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.RevenuePlanView, PermissionConstant.APPROVE);
                @base.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.RevenuePlanView, PermissionConstant.EDIT);
                @base.PermissionDelete = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.RevenuePlanView, PermissionConstant.DELETE);
                @base.IsSub = IsSubUnit(sessionUser); //IsSubUnit(sessionUser);

                @base.UserUnitsManages = await _userRepository.GetUserUnitManages(sessionUser.UserId);

                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.Revenue,
                    CostEstimateType = GlobalEnums.Year,
                    Subject = @base.IsSub ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
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

                bool searchAllUnit = ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0, ComparingMode.GatherThan);
                // từ kế toán trưởng MG trở lên 
                // thì có xem tất cả đơn vị
                if (!searchAllUnit)
                    @base.UserUnit = GetUserUnit().UnitId;
                @base.HostFileView = _fileHostView;

                var data = await _revenuePlanRepository.SearchAsync(@base, start, length, sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut);
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
        [AuthorizeUser(Module = Functions.RevenuePlanView, Permission = PermissionConstant.APPROVE)]

        public async Task<IActionResult> OnApproval(RevenuePlanApproveRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();

                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.Revenue,
                    CostEstimateType = GlobalEnums.Year,
                    Subject = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };

                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new RevenuePlanApproveResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });

                bool searchAllUnit = ComparePositionLevelUpper(((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0), ComparingMode.GatherThan);
                // từ kế toán trưởng MG trở lên 
                // thì có xem tất cả đơn vị
                request.UnitId = !searchAllUnit ? sessionUser.Unit.UnitId : (int)GlobalEnums.StatusDefaultEnum.All;

                request.PageRequest = RevenuePlanConst.PublicKey;

                request.Positions = sessionUser.Positions;
                request.UserId = sessionUser.UserId;
                request.UserName = sessionUser.UserName;
                request.StatusAllowsSeen = listStatsForDepartmentGroup;
                request.IsSub = IsSubUnit(sessionUser);
                request.HostFileView = _fileHostView;
                request.UnitType = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut;


                var record = await _revenuePlanRepository.GetByIdAsync(request.RawId);
                RevenuePlanApproveResponse response = new RevenuePlanApproveResponse();
                var sign = new SignatureConnect();

                var checkPermissionApprove = await _revenuePlanRepository.CheckPermissionApprove(request, record);
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
                            EnterpriseUser = sessionUser.AccountSignature,
                            EnterprisePass = sessionUser.PasswordSignature,

                            ClientId = _configuration.GetSection("Signature:APP_ID").Value,
                            ClientSec = _configuration.GetSection("Signature:APP_SECRET").Value,
                            PhysicalPath = _physicalFolder,
                            RelativePath = record.PathPdf,
                            Signer = request.UserName,
                            TextFilter = checkPermissionApprove.Position.Name
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
                        if (!checkPermissionApprove.IsApproval)
                        {
                            //string template = Path.GetFileName(record.PathPdf);
                            //var newPdf = sign.CreatePdf(record.PathExcel, template, _physicalFolder, record.CreatorName);
                            //if (string.IsNullOrEmpty(newPdf))
                            //    return Json(new CashFollowApproveResponse
                            //    {
                            //        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            //        Message = GlobalEnums.ErrorMessage
                            //    });
                            //record.PathPdf = newPdf;



                            var statusAllows = new DataSeenRequest
                            {
                                Type = GlobalEnums.Profit,
                                CostEstimateType = GlobalEnums.Year,
                                Subject = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                                GroupCodes = GetUserPositionCodes()
                            };

                            var listProfitStatusAllowSeen = await _costStatusesRepository.ListStatusesForSubject(statusAllows);
                            await _profitPlanRepository.AutoDecline(record.Year,
                                new ProfitPlanApproveRequest
                                {
                                    UnitId = sessionUser.Unit.UnitId,
                                    UserId = sessionUser.UserId,
                                    UserName = sessionUser.UserName,
                                    UnitType = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                                    Reason = request.Reason,
                                    IsSub = record.IsSub,
                                    Positions = sessionUser.Positions,
                                    StatusAllowsSeen = listProfitStatusAllowSeen,
                                    IsAuto = true
                                }, checkPermissionApprove.Position.GroupCode);
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
                    response = await _revenuePlanRepository.Approval(request, record, checkPermissionApprove);
                    //if (response.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                    //    sign.MoveTrash(pdfOlder.CreateAbsolutePath(_physicalFolder), _physicalFolder);
                }
                else
                    response = new RevenuePlanApproveResponse
                    {
                        Code = checkPermissionApprove.Code,
                        Message = checkPermissionApprove.Message
                    };

                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return Json(new ApproveRequestOnCostEstimateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }

        [AuthorizeUser(Module = Functions.RevenuePlanView, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> OnDelete(RevenuePlanApproveRequest request)
        {
            try
            {
                var position = GetUserPosition();
                var sessionUser = GetUsersSessionModel();
                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.Revenue,
                    CostEstimateType = GlobalEnums.Year,
                    //Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    Subject = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };

                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new RevenuePlanApproveResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });


                request.PageRequest = RevenuePlanConst.PublicKey;

                request.Positions = position;
                request.UserId = GetUserId();
                request.UserName = GetUserName();
                request.StatusAllowsSeen = listStatsForDepartmentGroup;
                request.IsSub = IsSubUnit(sessionUser);

                var response = await _revenuePlanRepository.Delete(request);
                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return Json(new RevenuePlanApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }
        //[AuthorizeUser(Module = Functions.RevenuePlanView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> ViewHistories(string record)
        {
            var request = new RevenuePlanViewHistoryRequest
            {
                Record = record,
                PageRequest = RevenuePlanConst.PublicKey
            };
            IList<RevenuePlanViewHistoryResponse> data = null;
            try
            {
                data = await _revenuePlanRepository.ViewHistories(request);
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
                    await _pdfLogsRepository.CounterDay(unit.UnitId, RevenuePlanConst.PublicKey);
                string templateName =
                    $"{DateTime.Now:yyyy-MM-dd}_{year}_{unit.UnitName.StringToNonUnicode().StringRemoveSpecial()}_Ke hoach doanh thu khach hang_{fileCounter}.pdf";
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