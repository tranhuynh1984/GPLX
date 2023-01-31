using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using GPLX.Core.DTO.Request.InvestmentPlan;
using GPLX.Core.DTO.Request.Signature;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.CashFollow;
using GPLX.Core.DTO.Response.CostEstimate;
using GPLX.Core.DTO.Response.InvestmentPlan;
using GPLX.Core.Enum;
using GPLX.Database.Models;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models.Investment;
using GPLX.Web.Signature;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Web.Controllers
{
    public class InvestmentPlanController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IInvestmentPlanRepository _investmentPlanRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPdfLogsRepository _pdfLogsRepository;
        private readonly ICashFollowRepository _cashFollowRepository;
        private readonly string _defaultRootFolder;
        private readonly string _fileHostView;
        private readonly string _physicalFolder;

        public InvestmentPlanController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IInvestmentPlanRepository investmentPlanRepository, IGroupsRepository groupsRepository, ICostStatusesRepository costStatusesRepository, IUnitRepository unitRepository, IUserRepository userRepository, IPdfLogsRepository pdfLogsRepository, ICashFollowRepository cashFollowRepository)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _investmentPlanRepository = investmentPlanRepository;
            _groupsRepository = groupsRepository;
            _costStatusesRepository = costStatusesRepository;
            _unitRepository = unitRepository;
            _userRepository = userRepository;
            _pdfLogsRepository = pdfLogsRepository;
            _cashFollowRepository = cashFollowRepository;
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");
            _fileHostView = configuration.GetValue<string>("FileHosting");
            _physicalFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder);
        }

        [AuthorizeUser(Module = Functions.InvestmentPlanAdd, Permission = PermissionConstant.VIEW)]

        public IActionResult Create()
        {
            ViewBag.IsSub = IsSubUnit();
            ViewBag.UnitType = IsSubUnit() ? "sub" : GetUserUnit().UnitType.ToLower();
            return View();
        }
        [AuthorizeUser(Module = Functions.InvestmentPlanView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> List()
        {
            var model = new InvestmentListModel
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
                    Functions.InvestmentPlanAdd, PermissionConstant.ADD);
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
        [AuthorizeUser(Module = Functions.InvestmentPlanAdd, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnExcelUpload([FromQuery(Name = "year")] int selectedFinanceYear)
        {
            var excelUploadResponse = new ExcelUploadResponse<InvestmentPlanDetails>();
            if (selectedFinanceYear.IsLastDayOfYear())
            {
                excelUploadResponse.AddError(new ExcelValidatorError { Message = $"Hết hạn năm tài chính {selectedFinanceYear}!" });
                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                return Json(excelUploadResponse);
            }
            if (selectedFinanceYear <= 0)
            {
                excelUploadResponse.AddError(new ExcelValidatorError { Message = $"Cần chọn năm tài chính!" });
                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                return Json(excelUploadResponse);
            }

            var listData = new InvestmentPlanExcelResponse();
            try
            {
                var sessionUser = GetUsersSessionModel();

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
                            excelUploadResponse.AddError(new ExcelValidatorError
                            {
                                Message = $"Cần đẩy sheet dữ liệu theo đúng năm tài chính đã chọn {selectedFinanceYear}. Năm tài chính của sheet là {year}!"
                            });
                            excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            return Json(excelUploadResponse);
                        }

                        bool matchHeader = false;
                        bool matchColHeader = false;
                        string sMatchHeader = "II. Kế hoạch đầu tư chi tiết";

                        int rowCounter = 0;
                        //
                        var excelValidator = new ExcelFormValidation(_configuration);
                        excelValidator = excelValidator.Load("ExcelFormValidator:InvestmentPlan");
                        var listFinals = new List<InvestmentPlanDetails>();
                        string unitType = IsSubUnit() ? "sub" : sessionUser.Unit.UnitType;

                        var listInvestmentContents =
                            await _investmentPlanRepository.GetAllInvestmentPlanContentsForSubject(unitType);

                        Type oType = typeof(InvestmentPlanDetails);
                        IList<ExcelRow> excelRows = new List<ExcelRow>();
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

                            

                            if (fCell < excelValidator.StartAtRow) { continue; }
                            if (!matchHeader || !matchColHeader) { continue; }


                            rowCounter++;
                            bool header = rowCounter == 1;
                            var cellsInRow = new List<ExcelCell>();
                            for (int j = 0; j <= excelValidator.ColumnConfigs.Max(x => x.Position); j++)
                            {
                                var dataAt = cellsRow[j];
                                var xCellColumnValidator =
                                    excelValidator.ColumnConfigs.FirstOrDefault(x => x.Position == j);

                                if (xCellColumnValidator != null)
                                {
                                    var cell = excelValidator.ReadCellAt(xCellColumnValidator, dataAt, header);
                                    cell.CellPosition = j;

                                    cellsInRow.Add(cell);
                                }
                            }

                            string cellContent = cellsInRow.FirstOrDefault(x => x.CellPosition == 2)?.StringCellValue;
                            if (!string.IsNullOrEmpty(cellContent))
                            {
                                if (cellContent.StartsWith("Tổng cộng", StringComparison.CurrentCultureIgnoreCase))
                                    break;
                                excelRows.Add(new ExcelRow { Cells = cellsInRow, RowIndex = fCell });
                            }
                        }


                        foreach (var xcw in excelRows)
                        {
                            var fInvestGroup = xcw.Cells.FirstOrDefault(x => x.FieldMapper == "InvestmentPlanContentName");
                            var capitalPlanName = xcw.Cells.FirstOrDefault(x => x.FieldMapper == "CapitalPlanName");
                            var spendingLoanPercent = xcw.Cells.FirstOrDefault(x => x.FieldMapper == "SpendingLoanPercent");
                            var costExpected = xcw.Cells.FirstOrDefault(x => x.FieldMapper == "CostExpected");
                            var spendingLoan = xcw.Cells.FirstOrDefault(x => x.FieldMapper == "SpendingLoan");


                            var oCapitalPlan = GlobalEnums.CapitalPlanNamesToValueDefaults.TryGetValue(capitalPlanName.StringCellValue.Trim() ?? string.Empty, out var cap) ? cap : -1;
                            if (oCapitalPlan == -1)
                            {
                                capitalPlanName.IsNotValidCell = true;
                                capitalPlanName.ErrorMessage = $"Phương án vốn {capitalPlanName.StringCellValue} không hợp lệ";
                            }

                            if (spendingLoanPercent.ReaderVal != null)
                            {
                                if ((double)spendingLoanPercent.ReaderVal > 0 && !capitalPlanName.StringCellValue.Equals("Vốn vay", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    capitalPlanName.IsNotValidCell = true;
                                    capitalPlanName.ErrorMessage = $"Phương án vốn {capitalPlanName.CellName} không hợp lệ;";
                                }
                                else if ((double)spendingLoanPercent.ReaderVal == 0 && !capitalPlanName.StringCellValue.Equals("Vốn tự có", StringComparison.CurrentCultureIgnoreCase) && capitalPlanName.StringCellValue.Equals("Đầu tư của MG", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    capitalPlanName.IsNotValidCell = true;
                                    capitalPlanName.ErrorMessage = $"Phương án vốn {capitalPlanName.CellName} không hợp lệ;";
                                }
                                else if ((double)spendingLoanPercent.ReaderVal <= 0 && capitalPlanName.StringCellValue.Equals("Vốn vay", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    spendingLoanPercent.IsNotValidCell = true;
                                    spendingLoanPercent.ErrorMessage = "Tỷ lệ vay phải lơn hơn 0";
                                }
                                else if ((double)spendingLoanPercent.ReaderVal < 0)
                                {
                                    spendingLoanPercent.IsNotValidCell = true;
                                    spendingLoanPercent.ErrorMessage = $"Dữ liệu ô {spendingLoanPercent.CellName} phải lớn hơn hoặc bằng 0";
                                }
                            }
                            if (costExpected.ReaderVal != null && (double)costExpected.ReaderVal < 0)
                            {
                                costExpected.IsNotValidCell = true;
                                costExpected.ErrorMessage = $"Dữ liệu ô {costExpected.CellName} phải lớn hơn hoặc bằng 0"; ;
                            }
                            else if (spendingLoan.ReaderVal != null && (double)spendingLoan.ReaderVal < 0)
                            {
                                spendingLoan.IsNotValidCell = true;
                                spendingLoan.ErrorMessage = $"Dữ liệu ô {spendingLoan.CellName} phải lớn hơn hoặc bằng 0";
                            }
                            var investGroup = listInvestmentContents.FirstOrDefault(c =>
                                c.InvestmentPlanContentName.Equals(fInvestGroup?.ReaderVal?.ToString().Trim(), StringComparison.CurrentCultureIgnoreCase));

                            if (investGroup == null && fInvestGroup != null)
                            {
                                fInvestGroup.IsNotValidCell = true;
                                fInvestGroup.ErrorMessage = $"Hạng mục {fInvestGroup.ReaderVal} không hợp lệ;";
                            }
                            else if (investGroup != null)
                            {
                                xcw.GroupId = investGroup.InvestmentPlanContentId;
                                xcw.GroupName = investGroup.InvestmentPlanContentName;

                            }
                        }


                        string errorSignerPos = await _costStatusesRepository.ValidateSignerPositionInExcel(ws, GlobalEnums.Investment,
                            GlobalEnums.Year, unitType, sessionUser.Unit.UnitId);
                        if (!string.IsNullOrEmpty(errorSignerPos))
                            excelUploadResponse.Errors.Add(new ExcelValidatorError { Message = errorSignerPos });

                        RowCellsValidate rcValidate = new RowCellsValidate
                        {
                            AllRows = excelRows
                        };
                        bool isValid = rcValidate.ValidateRows();


                        if (isValid && rcValidate.AllRows.All(c => c.Cells.All(m => !m.IsNotValidCell)))
                        {
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

                                var oExcelRow = (InvestmentPlanDetails)instanceOf;
                                oExcelRow.InvestmentPlanContentId = excelRow.GroupId;
                                oExcelRow.InvestmentPlanContentName = excelRow.GroupName;
                                oExcelRow.CapitalPlanId = GlobalEnums.CapitalPlanNamesToValueDefaults.TryGetValue(oExcelRow.CapitalPlanName ?? string.Empty, out var cap) ? cap : -1;

                                listFinals.Add(oExcelRow);
                            }

                            var listAggregate = new List<InvestmentPlanAggregate>();
                            foreach (var content in listInvestmentContents.OrderBy(x => x.Order))
                            {
                                var aggregate = listFinals.Where(x => x.InvestmentPlanContentId == content.InvestmentPlanContentId).ToList();
                                // đầu tư của MG
                                var mdGroup = aggregate.Where(x =>
                                        x.CapitalPlanId == (int)GlobalEnums.CapitalPlanEnum.PayFormInvestMedGroup).ToList();
                                // vốn vay
                                var spdLoan = aggregate
                                    .Where(x => x.CapitalPlanId == (int)GlobalEnums.CapitalPlanEnum.PayFormLoan)
                                    .Sum(x => x.SpendingLoan);
                                var totalMedGroup = 0.0;

                                mdGroup.ForEach(p =>
                                {
                                    if (p.SpendingLoanPercent > 0)
                                        totalMedGroup += Math.Round(p.CostExpected * p.SpendingLoanPercent / 100);
                                    // nếu đầu tư từ MG -> % không set thì là all
                                    else
                                        totalMedGroup += p.CostExpected;
                                });

                                var totalExpect = aggregate.Sum(x => x.CostExpected);

                                listAggregate.Add(new InvestmentPlanAggregate
                                {
                                    InvestmentPlanContentId = content.InvestmentPlanContentId,
                                    InvestmentPlanContentName = content.InvestmentPlanContentName,
                                    ExpectCostInvestment = aggregate.Any() ? Math.Round(aggregate.Sum(x => x.CostExpected)) : 0,
                                    // Vốn vay
                                    SpendingLoan = spdLoan,
                                    // Vốn MG
                                    CapitalMedGroup = totalMedGroup,
                                    ExpenditureCapital = totalExpect - (spdLoan + totalMedGroup)
                                });
                            }

                            listAggregate.ForEach(p =>
                            {
                                double expected = Math.Round(p.ExpectCostInvestment - (p.CapitalMedGroup + p.SpendingLoan));
                                p.ExpenditureCapital = expected > 0 ? expected : 0;
                            });

                            listData.InvestmentPlanDetails = listFinals;
                            listData.InvestmentPlanAggregates = listAggregate;

                            var primary = new InvestmentPlan
                            {
                                Year = year,
                                IsSub = IsSubUnit(sessionUser),
                                UnitId = sessionUser.Unit.UnitId,
                                UnitName = sessionUser.Unit.UnitName,
                                PathExcel = file.Replace(_physicalFolder, string.Empty),
                                TotalCapitalMedGroup = Math.Round(listData.InvestmentPlanAggregates.Sum(x => x.CapitalMedGroup)),
                                TotalExpectCostInvestment = Math.Round(listData.InvestmentPlanAggregates.Sum(x => x.ExpectCostInvestment)),
                                TotalExpenditureCapital = Math.Round(listData.InvestmentPlanAggregates.Sum(x => x.ExpenditureCapital)),
                                TotalSpendingLoan = Math.Round(listData.InvestmentPlanAggregates.Sum(x => x.SpendingLoan))
                            };

                            listData.InvestmentPlan = primary;
                            excelUploadResponse.SpecifyFieldValue = listData;
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

                        excelUploadResponse.AddErrorRange(excelValidator.Errors);
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
            return Json(excelUploadResponse);
        }
        [AuthorizeUser(Module = Functions.InvestmentPlanAdd, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> OnCreate(InvestmentPlanCreateRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                request.UnitName = sessionUser.Unit.UnitName;
                request.UnitId = sessionUser.Unit.UnitId;
                request.Creator = sessionUser.UserId;
                request.CreatorName = sessionUser.UserName;
                request.IsSub = IsSubUnit();
                request.UnitType = sessionUser.Unit.UnitType;

                #region Tạo PDF

                try
                {
                    var folder = _physicalFolder;
                    var fExcelFile = $"{folder}{request.InvestmentPlan.PathExcel}";
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
                        string templateName =
                            await _createPdfName(request.InvestmentPlan.Year, sessionUser.Unit);
                        string pdf = sign.CreatePdf(fullOwnerFile, templateName, _physicalFolder, sessionUser.UserName);

                        if (string.IsNullOrWhiteSpace(pdf))
                            return Json(new InvestmentPlanCreateResponse
                            {
                                Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                Message = "Không tạo được PDF!"
                            });
                        request.InvestmentPlan.PathPdf = pdf;
                        request.InvestmentPlan.PathExcel = fullOwnerFile.Replace(_physicalFolder, string.Empty).NormalizePath();
                    }
                    else
                    {
                        return Json(new InvestmentPlanCreateResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            Message = "Không tìm thấy dữ liệu gốc!"
                        });
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "{0}", e.Message);
                    return Json(new InvestmentPlanCreateResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = GlobalEnums.ErrorMessage
                    });
                }
                #endregion

                var response = await _investmentPlanRepository.Create(request);
                if (response.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                {
                    await _pdfLogsRepository.CreateAsync(new FilePdfCreateLogs
                    {
                        Type = InvestmentPlanConst.PublicKey,
                        UnitId = sessionUser.Unit.UnitId,
                        CreatedDate = DateTime.Now,
                        FilePath = request.InvestmentPlan.PathPdf
                    });
                }
                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "request {0}", request);
                return Json(new InvestmentPlanCreateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }
        [AuthorizeUser(Module = Functions.InvestmentPlanView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, SearchInvestmentPlanRequest @base)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                @base.PageRequest = InvestmentPlanConst.PublicKey;
                @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();
                @base.PermissionApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.InvestmentPlanView, PermissionConstant.APPROVE);
                @base.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.InvestmentPlanView, PermissionConstant.EDIT);
                @base.PermissionDelete = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.InvestmentPlanView, PermissionConstant.DELETE);
                @base.IsSub = IsSubUnit();
                @base.UserUnitsManages = await _userRepository.GetUserUnitManages(sessionUser.UserId);

                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.Investment,
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


                bool searchAllUnit = ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0, ComparingMode.GatherThan);
                // từ kế toán trưởng MG trở lên 
                // thì có xem tất cả đơn vị
                if (!searchAllUnit)
                    @base.UserUnit = sessionUser.Unit.UnitId;
                @base.HostFileView = _fileHostView;

                var data = await _investmentPlanRepository.SearchAsync(@base, start, length, sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut);
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
        [AuthorizeUser(Module = Functions.InvestmentPlanView, Permission = PermissionConstant.APPROVE)]

        public async Task<IActionResult> OnApproval(InvestmentPlanApproveRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.Investment,
                    CostEstimateType = GlobalEnums.Year,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };

                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new InvestmentPlanApproveResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });

                bool searchAllUnit = ComparePositionLevelUpper(((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0), ComparingMode.GatherThan);
                // từ kế toán trưởng MG trở lên 
                // thì có xem tất cả đơn vị
                request.UnitId = !searchAllUnit ? GetUserUnit().UnitId : (int)GlobalEnums.StatusDefaultEnum.All;

                request.PageRequest = InvestmentPlanConst.PublicKey;

                request.Positions = sessionUser.Positions;
                request.UserId = sessionUser.UserId;
                request.UserName = sessionUser.UserName;
                request.StatusAllowsSeen = listStatsForDepartmentGroup;
                request.IsSub = IsSubUnit();
                request.HostFileView = _fileHostView;
                request.UnitType = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut;


                var record = await _investmentPlanRepository.GetByIdAsync(request.RawId);
                //string pdfOlder = record?.PathPdf;
                InvestmentPlanApproveResponse response = new InvestmentPlanApproveResponse();
                var checkPermissionApprove = await _investmentPlanRepository.CheckPermissionApprove(request, record);
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
                            //string templateName = Path.GetFileName(record.PathPdf);
                            //var newPdf = sign.CreatePdf(record.PathExcel, templateName, _physicalFolder, record.CreatorName);

                            //if (string.IsNullOrEmpty(newPdf))
                            //    return Json(new CashFollowApproveResponse
                            //    {
                            //        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            //        Message = GlobalEnums.ErrorMessage
                            //    });
                            //record.PathPdf = newPdf;

                            // nếu từ chối kế hoạch đầu tư --> từ chối kế hoạch dòng tiền

                            var rqLoadStatusCashFollowAllows = new DataSeenRequest
                            {
                                Type = GlobalEnums.CashFollow,
                                CostEstimateType = GlobalEnums.Year,
                                Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                                GroupCodes = GetUserPositionCodes()
                            };

                            var listCashFollowAllowSeen = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusCashFollowAllows);

                            var clearCashFollowInYear = await _cashFollowRepository.AutoDecline(record.Year, new CashFollowApproveRequest
                            {
                                Positions = GetUserPosition(),
                                UserId = sessionUser.UserId,
                                UserName = sessionUser.UserName,
                                UnitType = sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                                Reason = request.Reason,
                                StatusAllowsSeen = listCashFollowAllowSeen
                            }, checkPermissionApprove.Position.GroupCode);
                            if (!clearCashFollowInYear)
                            {
                                return Json(new CashFollowApproveResponse
                                {
                                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                    Message = $"Không xóa được kế hoạch dòng tiền năm {record.Year}!"
                                });
                            }
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
                                response.Message = sigRt.Message;
                                return Json(response);
                            }

                            record.PathPdf = sigRt.SignaturePath.Replace(_physicalFolder, string.Empty).NormalizePath();
                        }
                    }

                    response = await _investmentPlanRepository.Approval(request, record, checkPermissionApprove);
                    //if (response.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                    //    sign.MoveTrash(pdfOlder.CreateAbsolutePath(_physicalFolder), _physicalFolder);
                }
                else
                    response = new InvestmentPlanApproveResponse
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
        [AuthorizeUser(Module = Functions.InvestmentPlanView, Permission = PermissionConstant.DELETE)]

        public async Task<IActionResult> OnDelete(InvestmentPlanApproveRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.Investment,
                    CostEstimateType = GlobalEnums.Year,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };

                var listStatsForDepartmentGroup = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listStatsForDepartmentGroup.Any())
                    return Json(new InvestmentPlanApproveResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });


                request.PageRequest = InvestmentPlanConst.PublicKey;

                request.Positions = sessionUser.Positions;
                request.UserId = sessionUser.UserId;
                request.UserName = sessionUser.UserName;
                request.StatusAllowsSeen = listStatsForDepartmentGroup;
                request.IsSub = IsSubUnit();

                var response = await _investmentPlanRepository.DeleteAsync(request);
                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", request);
                return Json(new InvestmentPlanApproveResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }
        //[AuthorizeUser(Module = Functions.InvestmentPlanView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> ViewHistories(string record)
        {
            var request = new InvestmentPlanViewHistoryRequest
            {
                Record = record,
                PageRequest = InvestmentPlanConst.PublicKey
            };
            IList<InvestmentPlanViewHistoryResponse> data = null;
            try
            {
                data = await _investmentPlanRepository.ViewHistories(request);
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
                    await _pdfLogsRepository.CounterDay(unit.UnitId, InvestmentPlanConst.PublicKey);
                string templateName =
                    $"{DateTime.Now:yyyy-MM-dd}_{year}_{unit.UnitName.StringToNonUnicode().StringRemoveSpecial()}_Ke hoach dau tu_{fileCounter}.pdf";
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
