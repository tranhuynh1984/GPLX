using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aspose.Cells;
using GPLX.Core.Contants;
using GPLX.Core.Contracts.CostEstimate;
using GPLX.Core.Contracts.CostEstimateItem;
using GPLX.Core.Contracts.Dashboard;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Payment;
using GPLX.Core.Contracts.PdfLogs;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Core.DTO.Entities;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.CostEstimate;
using GPLX.Core.DTO.Request.CostEstimateItem;
using GPLX.Core.DTO.Request.Signature;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.Actually;
using GPLX.Core.DTO.Response.CostEstimate;
using GPLX.Core.DTO.Response.CostEstimateItem;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Core.Model;
using GPLX.Database.Models;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using GPLX.Web.Signature;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Web.Controllers
{
    public partial class CostEstimateController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CostEstimateController> _logger;
        private readonly ICostEstimateRepository _costEstimateRepository;
        private readonly ICostEstimateItemRepository _costEstimateItemRepository;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly ICostEstimateLogRepository _costEstimateLogRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPdfLogsRepository _pdfLogsRepository;
        private readonly IDashboardRepository _dashboardRepository;

        private readonly string _fileHostView;
        private readonly string _defaultRootFolder;
        private readonly string _physicalFolder;

        public CostEstimateController(IConfiguration configuration,
                                      ICostEstimateItemRepository costEstimateItemRepository,
                                      ICostEstimateRepository costEstimateRepository,
                                      IWebHostEnvironment webHostEnvironment,
                                      ILogger<CostEstimateController> logger,
                                      ICostStatusesRepository costStatusesRepository,
                                      ICostEstimateLogRepository costEstimateLogRepository, IPaymentRepository paymentRepository,
                                      IGroupsRepository groupsRepository,
                                      IUnitRepository unitRepository,
                                      IUserRepository userRepository,
                                      IPdfLogsRepository pdfLogsRepository,
                                      IDashboardRepository dashboardRepository
                                      )
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _costEstimateRepository = costEstimateRepository;
            _costEstimateItemRepository = costEstimateItemRepository;
            _fileHostView = configuration.GetValue<string>("FileHosting");
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");
            _costEstimateItemRepository = costEstimateItemRepository;
            _costStatusesRepository = costStatusesRepository;
            _costEstimateLogRepository = costEstimateLogRepository;
            _paymentRepository = paymentRepository;
            _groupsRepository = groupsRepository;
            _unitRepository = unitRepository;
            _userRepository = userRepository;
            _pdfLogsRepository = pdfLogsRepository;
            _dashboardRepository = dashboardRepository;

            _physicalFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder);
        }
        [AuthorizeUser(Module = Functions.CostEstimateView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> List(string type)
        {
            var model = new CostEstimateListModel
            {
                Stats = GlobalEnums.DefaultStatusSearchList,
                DefaultStats = Globs.DefaultSearchAllStats,
                DefaultStatsName = Globs.DefaultSearchAllStatsName,
                RequestType = type,
                EnableSearchUnit = ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0, ComparingMode.GatherThan),
                Units = new List<SelectListItem>()
            };
            try
            {
                await _costEstimateRepository.CheckUnitNotCreateYet();
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
                Log.Error(e, "error");
            }
            return View(model);
        }
        /// <summary>
        /// Kế toán trưởng pick chọn các yêu cầu để lên dự trù
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostEstimateCreate, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> ChiefAccountantApprove(string record = default)
        {
            var model = new CostEstimateCreateModel
            {
                Record = record,
                EnableApprove = true,
                EnableCreate = true,
                ViewMode = string.Empty
            };
            var sessionUser = GetUsersSessionModel();
            if (!string.IsNullOrEmpty(record))
            {
                if (Guid.TryParse(record.StringAesDecryption(CostElementConst.PublicKey), out _))
                {
                    var dataView = await _costEstimateRepository.LoadCostEstimateItemsData(new CostEstimateViewRequest
                    {
                        Status = (int)GlobalEnums.StatusDefaultEnum.All,
                        PageRequest = CostElementConst.PublicKey,
                        Record = record,
                        UserName = sessionUser.UserName,
                        UnitName = sessionUser.Unit.UnitName,
                        DepartmentName = sessionUser.Department.DepartmentName,
                        UnitId = GetUserUnit().UnitId,
                        UserId = GetUserId(),
                        DepartmentId = sessionUser.Department.DepartmentId,
                        Positions = sessionUser.Positions,
                    });
                    model.DataView = dataView;
                    model.RequestType = dataView.RequestType;
                    model.ReportForWeek = dataView.ReportForWeek;
                }
            }
            return PartialView(model);
        }

        /// <summary>
        /// Import yêu cầu dành cho kế toán viên
        /// </summary>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostElementItemImport, Permission = PermissionConstant.ADD)]
        public IActionResult AccountantImport(string type)
        {
            var model = new CostEstimateCreateModel
            {
                EnableCreate = true,
                RequestType = GlobalEnums.DefaultKeyToTypes.TryGetValue(type, out var i) ? i : -1
            };
            return View("Create", model);
        }

        [HttpPost]
        [AuthorizeUser(Module = Functions.CostElementItemImport, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnCreateBulkItem(CostEstimateItemBulkCreateRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                request.UserId = sessionUser.UserId;
                request.UnitId = sessionUser.Unit.UnitId;
                request.UnitName = sessionUser.Unit.UnitName;
                request.UserName = sessionUser.UserName;
                request.DepartmentId = sessionUser.Department.DepartmentId;
                request.DepartmentName = sessionUser.Department.DepartmentName;
                request.Positions = sessionUser.Positions;
                request.PageRequest = CostElementConst.PublicKey;
                request.HostPath = _fileHostView;
                request.UnitType = GetUserUnit().UnitType;
                request.IsSub = IsSubUnit();

                var validator = new CostEstimateItemBulkCreateRequestValidator();
                var validate = await validator.ValidateAsync(request);
                if (!validate.IsValid)
                {
                    var errors = new ErrorsValidationResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = "Thao tác không thành công, dữ liệu không hợp lệ!",
                        Errors = validate.Errors.Select(x => x.ErrorMessage).ToList()
                    };
                    return Json(errors);
                }
                var response = await _costEstimateItemRepository.CreateBulk(request);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                var errors = new ErrorsValidationResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                };
                return Json(errors);
            }
        }

        /// <summary>
        /// Func xử lý tệp excel
        /// Đọc và trả về theo model đã được định nghĩa
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser(Module = Functions.CostElementItemImport, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnExcelUpload()
        {
            var excelUploadResponse = new ExcelUploadResponse<CostEstimateItemFromExcel>();
            try
            {
                var dataOnRequest = Request.Form.TryGetValue("data", out var v) ? v[0] : "-1";
                int lastCode = dataOnRequest.ToInt32();

                var fileOnRequest = Request.Form.Files;
                if (fileOnRequest.Count > 0)
                {
                    var excelFile = fileOnRequest[0];
                    string randomFolderName = "temporary";
                    var fileCreateName = $"{Path.GetRandomFileName()}{Path.GetExtension(excelFile.FileName)}";
                    var folder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder, DateTime.Now.ToString("yyyyMMdd"), randomFolderName);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    var file = Path.Combine(folder, fileCreateName);

                    await using var fileStream = new FileStream(file, FileMode.Create);
                    await excelFile.CopyToAsync(fileStream);
                    await fileStream.DisposeAsync();
                    fileStream.Close();
                    var allTypes = await _paymentRepository.AllTypes(IsSubUnit() ? "sub" : GetUserUnit().UnitType);
                    var workBook = new Workbook(file);
                    Worksheet worksheet = workBook.Worksheets[0];

                    

                    var excelValidator = new ExcelFormValidation(_configuration);
                    excelValidator = excelValidator.Load("ExcelFormValidator:AccountantFormWeek");
                    var cells = worksheet.Cells;

                    var column = cells.Columns;
                    if (column.Count < excelValidator.ColumnConfigs.Count)
                    {
                        excelUploadResponse.AddError(new ExcelValidatorError
                        {
                            Message = "Số lượng cột không đúng định dạng"
                        });
                        return Json(excelUploadResponse);
                    }


                    int totalRequestUnit = lastCode;
                    //lấy số lượng yêu cầu
                    if (lastCode == -1)
                    {
                        totalRequestUnit = await _costEstimateItemRepository.CreateCostEstimateItemCodeForUnit(GetUserUnit().UnitId);
                        if (totalRequestUnit == -1)
                        {
                            excelUploadResponse.AddError(new ExcelValidatorError { Message = "Lỗi tạo mã dự trù!" });
                            excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            return Json(excelUploadResponse);
                        }
                    }
                    IList<ExcelRow> excelRows = new List<ExcelRow>();
                    for (int i = 0; i < cells.Rows.Count; i++)
                    {
                        var isHeader = i == 0;
                        if (!isHeader)
                            totalRequestUnit++;

                        Row row = cells.Rows[i];
                        var fCell = row.FirstCell.Row;                        
                        if (row.IsHidden)
                            continue;
                        var rowParse = new CostEstimateItemFromExcel
                        {
                            RequestCode = totalRequestUnit.CreateEstimateRequestCode(GetUserUnit().UnitCode),
                            CreatorId = GetUserId(),
                            CreatorName = GetUserName()
                        };
                        var cellsInRow = new List<ExcelCell>();

                        for (int j = 0; j <= excelValidator.ColumnConfigs.Max(x => x.Position); j++)
                        {
                            var dataAt = row[j];
                            var xCellColumnValidator =
                                excelValidator.ColumnConfigs.FirstOrDefault(x => x.Position == j);

                            if (xCellColumnValidator != null)
                            {
                                var cell = excelValidator.ReadCellAt(xCellColumnValidator, dataAt, isHeader);
                                cell.CellPosition = j;
                                cellsInRow.Add(cell);
                            }
                        }

                        string cellContent = cellsInRow.FirstOrDefault(x => x.CellPosition == 1)?.StringCellValue;
                        string moneyCell = cellsInRow.FirstOrDefault(x => x.CellPosition == 2)?.StringCellValue;
                        if (!string.IsNullOrEmpty(cellContent) || !string.IsNullOrEmpty(moneyCell))
                        {
                            excelRows.Add(new ExcelRow { Cells = cellsInRow, RowIndex = fCell });
                        }
                    }

                    Type oType = typeof(CostEstimateItemFromExcel);
                    RowCellsValidate rcValidate = new RowCellsValidate
                    {
                        AllRows = excelRows
                    };
                    bool isValid = rcValidate.ValidateRows();
                    if (isValid && rcValidate.AllRows.All(c => c.Cells.All(m => !m.IsNotValidCell)))
                    {
                        Type oCellType = typeof(CostEstimateItemFromExcel);
                        int counter = 0;
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

                            var oExcelRow = (CostEstimateItemFromExcel)instanceOf;
                            oExcelRow.RequestCode = (totalRequestUnit + counter).CreateEstimateRequestCode(GetUserUnit().UnitCode);
                            oExcelRow.CreatorId = GetUserId();
                            oExcelRow.CreatorName = GetUserName();
                            var validType = allTypes.FirstOrDefault(x => x.Name.Equals(oExcelRow.CostEstimateItemTypeName));
                            if (validType == null)
                                excelUploadResponse.AddError(new ExcelValidatorError
                                {
                                    Message = "Nhóm chi phí không tồn tại!",
                                });
                            else
                                oExcelRow.CostEstimateType = validType.Id;


                            if (!string.IsNullOrEmpty(oExcelRow.Explanation))
                                excelUploadResponse.Add(oExcelRow);
                            else
                            {
                                if (!string.IsNullOrEmpty(oExcelRow.BillCode) &&
                                    oExcelRow.BillDate != DateTime.MinValue && oExcelRow.BillCost > 0)
                                {
                                    if (await _costEstimateItemRepository.CheckBillCodeExists(oExcelRow.BillCode))
                                    {
                                        excelUploadResponse.AddError(new ExcelValidatorError
                                        {
                                            Message = $"{excelRow.Cells.FirstOrDefault(c => c.CellPosition == 8)?.CellName}: Số hóa đơn/phiếu thu đã tồn tại!"
                                        });
                                    }
                                    else
                                        excelUploadResponse.Add(oExcelRow);
                                }
                                else if (!string.IsNullOrEmpty(oExcelRow.BillCode))
                                    excelUploadResponse.AddError(new ExcelValidatorError
                                    {
                                        Message = $"Dòng {excelRow.RowIndex}: Vui lòng điền đầy đủ thông tin hóa đơn!"
                                    });
                                else
                                {
                                    excelUploadResponse.AddError(new ExcelValidatorError
                                    {
                                        Message = $"Dòng {excelRow.RowIndex}: Bạn vui lòng nhập giải trình với các yêu cầu không có thông tin hóa đơn"
                                    });
                                }
                            }

                            counter++;
                        }

                        excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        excelUploadResponse.SpecifyFieldValue = totalRequestUnit;
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

                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogLevel.Error, e, e.Message);
                    }
                }
                else
                {
                    excelUploadResponse.AddError(new ExcelValidatorError { Message = "Không có tệp nào được tải lên" });
                    excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                }
                return Json(excelUploadResponse);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                excelUploadResponse.AddError(new ExcelValidatorError { Message = "Có lỗi xảy ra, vui lòng thử lại sau!" });
                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                return Json(excelUploadResponse);
            }
        }
        /// <summary>
        /// Upload chứng từ cho hợp đồng
        /// Hỗ trợ upload bulk (.zip)
        /// Hỗ trợ upload các file riêng lẻ
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeUser(Module = Functions.CostElementItemImport, Permission = PermissionConstant.ADD)]
        public async Task<IActionResult> OnUpdateContractFile()
        {
            var response = new ContractUploadResponse();
            try
            {
                string ownerFolder = GetUserSyncId().CreateDatePhysicalPathFileToUser();
                //zip
                string randomExtractingFolder = Path.GetRandomFileName();
                var fileOnRequest = Request.Form.Files;
                if (fileOnRequest.Count > 0)
                {
                    var uploadFile = fileOnRequest[0];
                    var fileExtension = Path.GetExtension(uploadFile.FileName);
                    response.FileUploadExtension = fileExtension;
                    string fileCreateName = $"{Path.GetRandomFileName()}{fileExtension}";

                    // Nếu là file nén mới cần tạo subFolder để giải nén
                    string fullPathCombine = fileExtension == ".zip" ?
                        Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder, ownerFolder, randomExtractingFolder) :
                        Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder, ownerFolder);
                    string fullFilePath = Path.Combine(fullPathCombine, fileCreateName);
                    try
                    {
                        if (!Directory.Exists(fullPathCombine))
                            Directory.CreateDirectory(fullPathCombine);

                        await using var fileStream = new FileStream(fullFilePath, FileMode.Create);
                        await uploadFile.CopyToAsync(fileStream);
                        await fileStream.DisposeAsync();
                        fileStream.Close();

                        switch (fileExtension)
                        {
                            case ".zip":
                                try
                                {
                                    FileInfo ff = new FileInfo(fullFilePath);
                                    System.IO.Compression.ZipFile.ExtractToDirectory(fullFilePath, fullPathCombine, false);

                                    response.FileUploadSize = ff.Length;
                                    DirectoryInfo di = new DirectoryInfo(fullPathCombine);

                                    var fileOnFolder = di.GetFiles("*", SearchOption.TopDirectoryOnly).Where(x => x.Name != fileCreateName).ToArray();
                                    if (!fileOnFolder.Any())
                                    {
                                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                                        response.Message = "Không tìm thấy dữ liệu trong tệp nén!";
                                    }
                                    else
                                    {
                                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                                        response.Message = "";
                                        foreach (var f in fileOnFolder)
                                        {
                                            string relative = f.FullName.Replace(Path.Combine(_webHostEnvironment.WebRootPath,
                                                _defaultRootFolder), string.Empty);
                                            response.Add(f.Name, relative.CreateHostFileView(_fileHostView), f.Length);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    System.IO.File.Delete(fullFilePath);
                                    _logger.Log(LogLevel.Error, e, e.Message, fullFilePath);
                                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                    response.Message = "Lỗi giải nén tệp tin, vui lòng sử dụng định dạng .zip!";
                                }
                                break;
                            default:
                                FileInfo defaultFi = new FileInfo(fullFilePath);
                                string defaultRelative = defaultFi.FullName.Replace(Path.Combine(_webHostEnvironment.WebRootPath,
                                    _defaultRootFolder), string.Empty);
                                response.Add(defaultFi.Name, defaultRelative.CreateHostFileView(_fileHostView), defaultFi.Length);
                                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                                response.FileUploadSize = defaultFi.Length;
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogLevel.Error, e, e.Message);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = "Lỗi hệ thống, vui lòng thử lại sau!";
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = "Lỗi hệ thống, vui lòng thử lại sau!";
            }
            return Json(response);
        }


        /// <summary>
        /// Danh sách dự trù trong phiếu
        /// </summary>
        /// <param name="length"></param>
        /// <param name="start"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostEstimateCreate, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> LoadEstimateCostData(int length, int start, CostEstimateViewRequest @base)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                if (!string.IsNullOrEmpty(@base.Record) && @base.RawId != Guid.Empty)
                {
                    @base.Status = (int)GlobalEnums.StatusDefaultEnum.All;
                    @base.UserName = sessionUser.UserName;
                    @base.UserId = sessionUser.UserId;
                    @base.UnitId = sessionUser.Unit.UnitId;
                    @base.UserName = sessionUser.UserName;

                    @base.DepartmentId = sessionUser.Department.DepartmentId;
                    @base.DepartmentName = sessionUser.Department.DepartmentName;
                    @base.Positions = sessionUser.Positions;

                    var data = await _costEstimateRepository.LoadCostEstimateItemsData(@base);
                    return Json(new
                    {
                        Draw = 1,
                        Data = data,
                        RecordsTotal = data.Data.Count,
                        RecordsFiltered = data.Data.Count
                    });
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
            }

            return Json(new
            {
                Draw = 1,
                Data = new List<CostEstimateItem>(),
                RecordsTotal = 0,
                RecordsFiltered = 0
            });
        }
        /// <summary>
        /// Tạo phiếu
        /// </summary>
        /// <param name="create"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostEstimateCreate, Permission = PermissionConstant.ADD)]

        public async Task<IActionResult> OnCreate(PreviewCostEstimateRequest create)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();

                Guid g = Guid.NewGuid();
                Database.Models.CostEstimate olderPrimary = null;
                if (!string.IsNullOrEmpty(create.Record))
                {
                    var ok = Guid.TryParse(create.Record.StringAesDecryption(CostElementConst.PublicKey), out g);
                    if (!ok)
                    {
                        return Json(new ErrorsValidationResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            Message = "Không tìm thấy dữ liệu yêu cầu!"
                        });
                    }

                    olderPrimary = await _costEstimateRepository.GetByIdAsync(g);
                    if (olderPrimary == null)
                    {
                        return Json(new ErrorsValidationResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            Message = "Không tìm thấy dữ liệu yêu cầu!"
                        });
                    }
                }
                else
                {
                    var canCreate = await _costEstimateRepository.CanCreate(sessionUser.Unit.UnitId, IsSubUnit(sessionUser), create.Week);
                    if (!canCreate)
                    {
                        return Json(new ErrorsValidationResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                        });
                    }
                }

                create.UnitName = sessionUser.Unit.UnitName;
                create.UnitId = sessionUser.Unit.UnitId;
                create.UserId = sessionUser.UserId;
                create.UserName = sessionUser.UserName;
                create.PageRequest = CostElementConst.PublicKey;
                create.IsSub = IsSubUnit(sessionUser);

                bool isEditCreate = string.IsNullOrEmpty(create.Type);


                string template = IsSubUnit(sessionUser) ? @"./Resources/SmartMarkerSubTemplate.xlsx" : "./Resources/SmartMarkerUnitTemplate.xlsx";
                Workbook workbook = new Workbook(template);

                WorkbookDesigner designer = new WorkbookDesigner(workbook);
                var dataLayer = new GenerateExcelCreateCostEstimateModel();
                var data = create.Data;

                #region Các biến dữ liệu trên cell

                double equityCostVal = 0.0;
                double rotationProposalValTu = 0.0;
                double rotationProposalValWe = 0.0;

                #endregion

                #region Tạo file excel để tính theo công thức
                var w = DateTime.Now.Year.WeekInYear();
                var weekInfo = w.FirstOrDefault(c => c.weekNum == create.Week);
                string reportName =
                    $"Dự trù tuần {create.Week} - {create.UnitName} (từ {weekInfo?.weekStart:dd/MM/yyyy} - {weekInfo?.weekFinish:dd/MM/yyyy})";
                var oCashValue = isEditCreate ? _findCellValue(data, "Tiền mặt") : olderPrimary?.Cash;
                var cashValue = Math.Round(oCashValue.ToDouble(), MidpointRounding.AwayFromZero);
                //Ngân hàng Techcombank cty

                var tcbCompanyValue = isEditCreate ? _findCellValue(data, "Ngân hàng Techcombank cty") : olderPrimary?.BankTCB;
                var techcomValue = Math.Round(tcbCompanyValue.ToDouble(), MidpointRounding.AwayFromZero);

                // Ngân hàng BIDV
                var bidvCompanyValue = isEditCreate ? _findCellValue(data, "Ngân hàng BIDV cty") : olderPrimary?.BankBIDV;
                var bidvcomValue = Math.Round(bidvCompanyValue.ToDouble(), MidpointRounding.AwayFromZero);

                var bidvCompanyValue2 = isEditCreate ? _findCellValue(data, "Ngân hàng BIDV cty2 (TK chuyên thu)") : olderPrimary?.BankBIDVRevenue;
                var bidvcomValue2 = Math.Round(bidvCompanyValue2.ToDouble(), MidpointRounding.AwayFromZero);

                var amountAvailableValue = isEditCreate ? _findCellValue(data, "Hạn mức tín dụng – Tổng số dư khả dụng") : olderPrimary?.LoanAvailable;
                var amountAvailable = Math.Round(amountAvailableValue.ToDouble(), MidpointRounding.AwayFromZero);

                var operatingValueCell = isEditCreate ? _findCellValue(data, "Thu từ hoạt động kinh doanh") : olderPrimary?.OperatingRevenue;
                var operatingValue = Math.Round(operatingValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                var contributionValueCell = isEditCreate ? _findCellValue(data, "Thu từ hoạt động góp vốn") : olderPrimary?.ContributionRevenue;
                var contribution = Math.Round(contributionValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                var diffReceiveValueCell = isEditCreate ? _findCellValue(data, "Thu khác") : olderPrimary?.DiffRevenue;
                var diffReceive = Math.Round(diffReceiveValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                if (!IsSubUnit(sessionUser))
                {
                    var equityValueCell = isEditCreate ? _findCellValue(data, "Định mức tồn quỹ (5)") : olderPrimary?.EquityCost;
                    equityCostVal = Math.Round(equityValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                    var rotationTuValueCell = isEditCreate ? _findCellValue(data, "Chuyển về thứ 3") : olderPrimary?.RotationProposalTue;
                    rotationProposalValTu = Math.Round(rotationTuValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                    var rotationWeValueCell = isEditCreate ? _findCellValue(data, "Chuyển về thứ 5") : olderPrimary?.RotationProposalWe;
                    rotationProposalValWe = Math.Round(rotationWeValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                    designer.SetDataSource("EquityCost", equityCostVal);
                    designer.SetDataSource("RotationProposalTue", rotationProposalValTu);
                    designer.SetDataSource("RotationProposalWe", rotationProposalValWe);
                }


                designer.SetDataSource("Cash", cashValue);
                designer.SetDataSource("Techcombank", techcomValue);
                designer.SetDataSource("BIDV", bidvcomValue);
                designer.SetDataSource("BIDVReceive", bidvcomValue2);
                designer.SetDataSource("Available", amountAvailable);

                designer.SetDataSource("OperatingValue", operatingValue);
                designer.SetDataSource("ContributeValue", contribution);
                designer.SetDataSource("DiffReceiveValue", diffReceive);


                var notSpent = await _costEstimateItemRepository.GetCostEstimateItemNotSpent(create.Week, sessionUser.Unit.UnitId, IsSubUnit(sessionUser), string.Empty);
                if (notSpent.Count > 0)
                {
                    var dataNoSpent = new List<NoSpentModel>();
                    foreach (var costEstimateItem in notSpent)
                    {
                        dataNoSpent.Add(new NoSpentModel
                        {
                            RequestCode = costEstimateItem.RequestCode,
                            StatusName = "Đã duyệt và chưa được chi",
                            Cost = costEstimateItem.Cost,
                            RequestContent = costEstimateItem.RequestContent,
                            Date = $"Tuần {costEstimateItem.PayWeek}"
                        });
                    }
                    designer.SetDataSource("MissingSpent", dataNoSpent);
                }

                var operatingData = create.Request.Where(c => c.CostEstimatePaymentType == 2).ToList();
                var investData = create.Request.Where(c => c.CostEstimatePaymentType == 3).ToList();
                var financeData = create.Request.Where(c => c.CostEstimatePaymentType == 1).ToList();
                var lstDecline = new List<CostEstimateItemSearchResponseData>();
                if (!string.IsNullOrEmpty(create.Record))
                    lstDecline = create.Request.Where(c => c.IsDeleted == 1).ToList();

                if (lstDecline.Count > 0)
                {
                    foreach (var dcl in lstDecline)
                    {
                        if (dcl.Updater == 0)
                        {
                            dcl.Updater = sessionUser.UserId;
                            dcl.UpdaterName = sessionUser.UserName;
                            dcl.UpdatedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        }
                    }
                    dataLayer.NOperating = lstDecline.Where(c => c.CostEstimatePaymentType == 2).ToList();
                    dataLayer.NInvest = lstDecline.Where(c => c.CostEstimatePaymentType == 3).ToList();
                    dataLayer.NFinance = lstDecline.Where(c => c.CostEstimatePaymentType == 1).ToList();
                }


                dataLayer.Operating = operatingData.Where(c => c.IsDeleted == (int)GlobalEnums.StatusDefaultEnum.InActive).ToList();
                dataLayer.Investment = investData.Where(c => c.IsDeleted == (int)GlobalEnums.StatusDefaultEnum.InActive).ToList();
                dataLayer.Finance = financeData.Where(c => c.IsDeleted == (int)GlobalEnums.StatusDefaultEnum.InActive).ToList();

                designer.SetDataSource("Operating", dataLayer.Operating);
                designer.SetDataSource("Investment", dataLayer.Investment);
                designer.SetDataSource("Finance", dataLayer.Finance);
                designer.SetDataSource("Name", reportName);

                // không được chọn
                designer.SetDataSource("NOperating", dataLayer.NOperating);
                designer.SetDataSource("NInvest", dataLayer.NInvest);
                designer.SetDataSource("NFinance", dataLayer.NFinance);

                designer.SetDataSource("ActuallySpentMissing", "III. BÁO CÁO CÁC KHOẢN DỰ TRÙ ĐÃ DUYỆT NHƯNG CHƯA CHI");

                CalculationOptions opts = new CalculationOptions { IgnoreError = true };
                designer.Process();
                workbook.CalculateFormula(opts);

                #endregion

                #region Tìm các dòng theo nội dung

                int startRowAutoFit = 0;
                int endRowAutoFit = 0;
                double equityCapital = 0.0;
                double availableLoan = 0.0;
                double planCutCostVal = 0.0;
                double rotationProposalVal = 0.0;

                var cells = workbook.Worksheets[0].Cells;
                int startDeclineTable = 0;
                int endDeclineTable = 0;

                var totalExpenditureCapitalVal = _getDoubleCellValue(cells, "Tổng chi bằng vốn tự có");
                foreach (Row cellRow in cells.Rows)
                {
                    string cellValue = cellRow[1].StringValue;
                    string cellContent = cellRow[2].StringValue;
                    if (cellValue.StartsWith("II. Dự trù", StringComparison.CurrentCultureIgnoreCase))
                        startDeclineTable = cellRow.Index;
                    if (cellValue.StartsWith("III. BÁO CÁO", StringComparison.CurrentCultureIgnoreCase))
                        endDeclineTable = cellRow.Index;

                    if (cellContent.StartsWith("Vốn tự có (I.1", StringComparison.CurrentCultureIgnoreCase))
                        equityCapital = cellRow[3].DoubleValue;
                    if (cellContent.StartsWith("Số dư khả dụng hạn mức tín dụng", StringComparison.CurrentCultureIgnoreCase))
                        availableLoan = cellRow[3].DoubleValue;

                    if (!IsSubUnit(sessionUser))
                    {
                        if (cellContent.StartsWith("Định mức tồn quỹ", StringComparison.CurrentCultureIgnoreCase))
                            equityCostVal = cellRow[3].DoubleValue;
                        if (cellContent.StartsWith("Dự kiến cắt tiền về dòng tiền tập trung", StringComparison.CurrentCultureIgnoreCase))
                            planCutCostVal = cellRow[3].DoubleValue;
                        if (cellContent.StartsWith("Đề xuất luân chuyển tiền về ĐVTV", StringComparison.CurrentCultureIgnoreCase))
                            rotationProposalVal = cellRow[3].DoubleValue;
                        if (cellContent.StartsWith("Chuyển về thứ 3", StringComparison.CurrentCultureIgnoreCase))
                            rotationProposalValTu = cellRow[3].DoubleValue;
                        if (cellContent.StartsWith("Chuyển về thứ 5", StringComparison.CurrentCultureIgnoreCase))
                            rotationProposalValWe = cellRow[3].DoubleValue;
                        //Các khoản chi hoạt động
                        //Tổng chi bằng vốn tự có
                        if (cellContent.StartsWith("Các khoản chi hoạt động", StringComparison.CurrentCultureIgnoreCase) && startRowAutoFit == 0)
                            startRowAutoFit = cellRow.Index;
                        if (cellContent.StartsWith("Tổng chi bằng vốn tự có", StringComparison.CurrentCultureIgnoreCase) && endRowAutoFit == 0)
                            endRowAutoFit = cellRow.Index;
                    }
                }

                for (int i = startRowAutoFit; i <= endRowAutoFit - 1; i++)
                {
                    var cellWrap = workbook.Worksheets[0].Cells[i, 2];
                    var cellStyle = cellWrap.GetStyle();
                    cellStyle.IsTextWrapped = true;
                    cellWrap.SetStyle(cellStyle);
                }
                workbook.Worksheets[0].AutoFitRows(startRowAutoFit, endRowAutoFit - 1, new AutoFitterOptions { IgnoreHidden = true });

                if (lstDecline.Count == 0)
                    cells.HideRows(startDeclineTable, endDeclineTable - startDeclineTable);
                else
                {
                    for (int i = startDeclineTable; i <= endDeclineTable - 1; i++)
                    {
                        var cellWrap = workbook.Worksheets[0].Cells[i, 2];
                        var cellStyle = cellWrap.GetStyle();
                        cellStyle.IsTextWrapped = true;
                        cellWrap.SetStyle(cellStyle);
                    }
                    workbook.Worksheets[0].AutoFitRows(startDeclineTable, endDeclineTable - 1, new AutoFitterOptions { IgnoreHidden = true });
                }

                //mặc định ẩn phần chưa đc chi
                if (notSpent.Count == 0)
                    cells.HideRows(endDeclineTable, 3);

                #endregion

                if (equityCapital < 0)
                {
                    // vốn tự có
                    return Json(new ErrorsValidationResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = "Vốn tự có không được nhỏ hơn 0!"
                    });
                }

                if (availableLoan < 0)
                {
                    return Json(new ErrorsValidationResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = "Số dư khả dụng âm, đề nghị chọn lại hình thức chi!"
                    });
                }

                if (IsSubUnit(sessionUser))
                {
                    if (rotationProposalVal > totalExpenditureCapitalVal * 1.5)
                    {
                        //Bạn đã nhập số tiền đề xuất luân chuyển vượt định mức, đề nghị chọn số tiền nhỏ hơn
                        return Json(new ErrorsValidationResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            Message = "Bạn đã nhập số tiền đề xuất luân chuyển vượt định mức, đề nghị chọn số tiền nhỏ hơn!"
                        });
                    }
                }

                var createModel = new CostEstimateCreateRequest
                {
                    UnitId = sessionUser.Unit.UnitId,
                    UnitName = sessionUser.Unit.UnitName,
                    Record = create.Record,
                    UserId = sessionUser.UserId,
                    UserName = sessionUser.UserName,
                    PageRequest = CostElementConst.PublicKey,
                    Data = create.Request,
                    PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                        Functions.CostEstimateCreate, PermissionConstant.EDIT),
                    Type = create.Type,
                    IsApproval = create.IsApprove,
                    OlderPrimary = olderPrimary,
                    Reason = create.Reason
                };

                // phê duyệt - từ chối
                if (!string.IsNullOrEmpty(createModel.Type))
                {
                    var rqLoadStatusAllows = new DataSeenRequest
                    {
                        Type = GlobalEnums.CostStatusesElement,
                        CostEstimateType = GlobalEnums.Week,
                        Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                        GroupCodes = GetUserPositionCodes()
                    };

                    var listAllowSeen = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                    if (!listAllowSeen.Any())
                        return Json(new ErrorsValidationResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                            Message = "Không có quyền thực hiện thao tác!"
                        });
                    createModel.StatusAllowsSeen = listAllowSeen.ToList();

                    var check = await _costEstimateRepository.CheckPermissionApprove(createModel, olderPrimary);

                    if (check.Code != (int)GlobalEnums.ResponseCodeEnum.Success)
                        return Json(new ErrorsValidationResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                            Message = check.Message
                        });
                    createModel.CheckApprove = check;
                }

                var ownerFolder = sessionUser.UserName.CreateDatePhysicalPathFileToUser();
                var fullOwnerFolder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder, ownerFolder);
                if (!Directory.Exists(fullOwnerFolder))
                    Directory.CreateDirectory(fullOwnerFolder);
                var fullOwnerFile = Path.Combine(fullOwnerFolder, $"{Path.GetRandomFileName()}.xlsx");
                workbook.Save(fullOwnerFile, SaveFormat.Xlsx);


                var sign = new SignatureConnect();
                string templateName = string.IsNullOrEmpty(create.Record) ?
                    await _createPdfName(DateTime.Now.Year, create.Week, sessionUser.Unit) : Path.GetFileName(olderPrimary?.PathPdf);
                string temporaryPdf = $"{Guid.NewGuid()}.pdf";
                // nếu là cập nhật -> tạo file tạm
                // cập nhật thành công -> mới move sang đè file cũ

                string pdf;
                if (!string.IsNullOrEmpty(create.Record))
                {
                    pdf = olderPrimary?.PathPdf;
                    temporaryPdf = sign.CreatePdf(fullOwnerFile, temporaryPdf, _physicalFolder, string.Empty);
                    if (string.IsNullOrWhiteSpace(temporaryPdf))
                        return Json(new ErrorsValidationResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            Message = "Không tạo được PDF!"
                        });

                    #region Ký thử nếu là bước phê duyệt
                    if (createModel.CheckApprove?.IsApproval == true)
                    {
                        // ký số
                        if (createModel.CheckApprove.Granted.Sign)
                        {
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
                                RelativePath = temporaryPdf.Replace(_physicalFolder, string.Empty),
                                Signer = sessionUser.UserName,
                                TextFilter = createModel.CheckApprove.Granted.PositionName
                            };

                            var signRt = sign.Signature(signOp);

                            if (signRt.IsError)
                            {
                                return Json(new ErrorsValidationResponse
                                {
                                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                    Message = "Lỗi ký số phê duyệt không thành công!"
                                });
                            }
                        }
                        else
                        {
                            var imgSignature = $"{_physicalFolder}\\{sessionUser.PathSignature}";
                            if (System.IO.File.Exists(imgSignature))
                            {

                                var checkSign = sign.SingNormal($"{_physicalFolder}\\{temporaryPdf}",
                                    System.IO.File.ReadAllBytes(imgSignature), sessionUser.UserName, createModel.CheckApprove.Granted.PositionName,
                                    isChiefMg: true);
                                if (checkSign.IsError)
                                {
                                    return Json(new ErrorsValidationResponse
                                    {
                                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                        Message = checkSign.Message
                                    });
                                }
                            }
                            else
                            {
                                return Json(new ErrorsValidationResponse
                                {
                                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                    Message = "Tài khoản chưa cấu hình chữ ký điện tử!"
                                });
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    pdf = sign.CreatePdf(fullOwnerFile, templateName, _physicalFolder, sessionUser.UserName);//sessionUser.UserName
                    if (string.IsNullOrWhiteSpace(pdf))
                        return Json(new ErrorsValidationResponse
                        {
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                            Message = "Không tạo được PDF!"
                        });
                }


                var primary = new Database.Models.CostEstimate
                {
                    BeginAvailableBalance = _getDoubleCellValue(cells, "Số dư khả dụng đầu kỳ"),
                    AccountBalance = _getDoubleCellValue(cells, "Số dư TK thanh toán"),
                    ExpectRevenue = _getDoubleCellValue(cells, "Số tiền thu dự kiến"),
                    EstimatedCost = _getDoubleCellValue(cells, "Số tiền chi phí dự kiến"),
                    OperatingCost = _getDoubleCellValue(cells, "Các khoản chi hoạt động"),
                    InvestmentCost = _getDoubleCellValue(cells, "Các khoản chi đầu tư"),
                    FinancialCost = _getDoubleCellValue(cells, "Các khoản chi hoạt động tài chính"),
                    TotalExpenditureCapital = _getDoubleCellValue(cells, "Tổng chi bằng vốn tự có"),
                    TotalSpendingLoan = _getDoubleCellValue(cells, "Tổng chi bằng vay lưu động"),
                    EndAvailableBalance = _getDoubleCellValue(cells, "Số dư khả dụng cuối kỳ"),
                    Funds = _getDoubleCellValue(cells, "Vốn tự có (=I.1 + I.2 + II - III.4)"),
                    WorkingBalanceCost = _getDoubleCellValue(cells, "Số dư khả dụng hạn mức tín dụng"),
                    CostEstimateBody = JsonConvert.SerializeObject(create.Data),
                    UserId = createModel.UserId,
                    UserName = createModel.UserName,
                    UnitId = createModel.UnitId,
                    UnitName = createModel.UnitName,
                    IsSub = IsSubUnit(sessionUser),
                    ReportForWeek = create.Week,
                    ReportForWeekName = reportName,
                    Status = createModel.CheckApprove?.Granted.Value ?? (int)GlobalEnums.StatusDefaultEnum.InActive,
                    StatusName = createModel.CheckApprove?.Granted.Name ?? "Chờ duyệt",
                    CostEstimateType = (int)GlobalEnums.StatusDefaultType.Weekly,
                    CostEstimateTypeName = GlobalEnums.DefaultTypeNames[(int)GlobalEnums.StatusDefaultType.Weekly],
                    CreatedDate = DateTime.Now,
                    PathExcel = fullOwnerFile.Replace(_physicalFolder, string.Empty).NormalizePath(),
                    PathPdf = pdf,
                    Id = g,
                    ContributionRevenue = contribution,
                    BankBIDV = bidvcomValue,
                    BankBIDVRevenue = bidvcomValue2,
                    LoanAvailable = amountAvailable,
                    DiffRevenue = diffReceive,
                    BankTCB = techcomValue,
                    Cash = cashValue,
                    OperatingRevenue = operatingValue,
                    EquityCost = equityCostVal,
                    PlanCutCost = planCutCostVal,
                    RotationProposal = rotationProposalVal,
                    RotationProposalTue = rotationProposalValTu,
                    RotationProposalWe = rotationProposalValWe
                };
                createModel.Primary = primary;

                #region Ký ngược tại bước cuối

                if (!string.IsNullOrEmpty(create.Record))
                {
                    string absolute = $"{_physicalFolder}\\{temporaryPdf}";
                    if (createModel.CheckApprove?.IsMaxStep == true)
                    {
                        // Người lập
                        var pathSigner = sign.SingNormal(absolute, null, olderPrimary?.UserName, "Người lập", true);
                        var logs = await _costEstimateRepository.GetCostEstimateLogs(olderPrimary?.Id ?? Guid.Empty);
                        if (logs != null && !pathSigner.IsError)
                        {
                            // bước phê duyệt của user đăng nhập
                            logs.Add(new CostEstimateLogs
                            {
                                UserName = sessionUser.UserName,
                                UserId = sessionUser.UserId,
                                PositionName = createModel.CheckApprove.Granted.PositionName,
                                ToStatus = createModel.CheckApprove.Granted.Value
                            });


                            var allStats = await _costStatusesRepository.GetAll();

                            var sigNature = allStats.Where(c =>
                                c.Type == GlobalEnums.CostStatusesElement && c.IsApprove == 1
                                          && c.StatusForSubject.Equals(
                                              IsSubUnit(sessionUser)
                                                  ? CostElementItemConst.RequestTypeSub
                                                  : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                                              StringComparison.CurrentCultureIgnoreCase)).ToList();

                            #region Ký ngược ở bước cuối

                            var olderApprove = logs.Where(mm => sigNature.Any(c => c.Value == mm.ToStatus)).ToList();
                            foreach (var c in olderApprove)
                            {
                                var oUser = await _userRepository.GetUserByIdAsync(c.UserId);
                                var imgSignature = $"{_physicalFolder}\\{oUser.PathSignature}";
                                if (sigNature.First(ec => ec.Value == c.ToStatus).Singed)
                                {
                                    var signOp = new SignOpts
                                    {
                                        EndpointToken = _configuration.GetSection("Signature:SERVICE_GET_TOKENURL").Value,
                                        EndpointQuery = _configuration.GetSection("Signature:SERVICE_URL").Value,
                                        EnterpriseAcc = _configuration.GetSection("Signature:ENTERPRISE_ACC").Value,

                                        EnterpriseUser = oUser.AccDigitalSignature,
                                        EnterprisePass = oUser.PasswordDigitalSignature.StringAesDecryption(UsersConst.PublicKey),

                                        ClientId = _configuration.GetSection("Signature:APP_ID").Value,
                                        ClientSec = _configuration.GetSection("Signature:APP_SECRET").Value,
                                        PhysicalPath = _physicalFolder,
                                        RelativePath = pathSigner.SignaturePath.Replace(_physicalFolder, string.Empty),
                                        Signer = c.UserName,
                                        TextFilter = c.PositionName
                                    };

                                    var signRt = sign.Signature(signOp);

                                    if (signRt.IsError)
                                    {
                                        return Json(new ErrorsValidationResponse
                                        {
                                            Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                            Message = "Lỗi ký số phê duyệt không thành công!"
                                        });
                                    }
                                }
                                else
                                {
                                    if (System.IO.File.Exists(imgSignature))
                                    {
                                        pathSigner = sign.SingNormal(pathSigner.SignaturePath,
                                            System.IO.File.ReadAllBytes(imgSignature), oUser.UserName, c.PositionName,
                                            isChiefMg: true);
                                        if (pathSigner.IsError)
                                        {
                                            return Json(new ErrorsValidationResponse
                                            {
                                                Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                                                Message = pathSigner.Message
                                            });
                                        }
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }

                #endregion

                var createRt = await _costEstimateRepository.Create(createModel);

                if (createRt.Code != (int)GlobalEnums.ResponseCodeEnum.Success)
                {

                    if (string.IsNullOrEmpty(create.Record))
                    {
                        sign.DeleteOlder(fullOwnerFile);
                        sign.DeleteOlder($"{_physicalFolder}\\{pdf}");
                    }
                    else
                        sign.DeleteOlder($"{_physicalFolder}\\{temporaryPdf}");
                }
                else
                {
                    if (string.IsNullOrEmpty(create.Record))
                        await _pdfLogsRepository.CreateAsync(new FilePdfCreateLogs
                        {
                            Type = CostElementConst.PublicKey,
                            UnitId = sessionUser.Unit.UnitId,
                            CreatedDate = DateTime.Now,
                            FilePath = pdf
                        });
                    else
                        System.IO.File.Move($"{_physicalFolder}\\{temporaryPdf}", $"{_physicalFolder}\\{olderPrimary?.PathPdf}", true);
                }
                return Json(createRt);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");

                return Json(new ErrorsValidationResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                });
            }
        }
        [AuthorizeUser(Module = Functions.CostEstimateCreate, Permission = PermissionConstant.APPROVE)]
        public async Task<IActionResult> OnDecline(PreviewCostEstimateRequest create)
        {
            try
            {
                var ok = Guid.TryParse(create.Record.StringAesDecryption(CostElementConst.PublicKey), out var g);
                if (!ok)
                {
                    return Json(new ErrorsValidationResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });
                }

                var olderPrimary = await _costEstimateRepository.GetByIdAsync(g);
                if (olderPrimary == null)
                {
                    return Json(new ErrorsValidationResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = "Không tìm thấy dữ liệu yêu cầu!"
                    });
                }

                var sessionUser = GetUsersSessionModel();

                var createModel = new CostEstimateCreateRequest
                {
                    UnitId = sessionUser.Unit.UnitId,
                    UnitName = sessionUser.Unit.UnitName,
                    Record = create.Record,
                    UserId = sessionUser.UserId,
                    UserName = sessionUser.UserName,
                    PageRequest = CostElementConst.PublicKey,
                    Data = create.Request,
                    PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                        Functions.CostEstimateCreate, PermissionConstant.EDIT),
                    Type = create.Type,
                    IsApproval = create.IsApprove,
                    OlderPrimary = olderPrimary,
                    Reason = create.Reason
                };

                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.CostStatusesElement,
                    CostEstimateType = GlobalEnums.Week,
                    Subject = IsSubUnit(sessionUser) ? CostElementItemConst.RequestTypeSub : sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut,
                    GroupCodes = GetUserPositionCodes()
                };

                var listAllowSeen = await _costStatusesRepository.ListStatusesForSubject(rqLoadStatusAllows);
                if (!listAllowSeen.Any())
                    return Json(new ErrorsValidationResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không có quyền thực hiện thao tác!"
                    });
                createModel.StatusAllowsSeen = listAllowSeen.ToList();

                var check = await _costEstimateRepository.CheckPermissionApprove(createModel, olderPrimary);

                if (check.Code != (int)GlobalEnums.ResponseCodeEnum.Success)
                    return Json(new ErrorsValidationResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = check.Message
                    });
                createModel.CheckApprove = check;
                createModel.OlderPrimary = olderPrimary;

                var primary = olderPrimary;
                primary.Status = check.Granted.Value;
                primary.StatusName = check.Granted.Name;

                createModel.Primary = primary;
                createModel.IsApproval = false;

                var createRt = await _costEstimateRepository.Create(createModel);

                return Json(createRt);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");

                return Json(new ErrorsValidationResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                });
            }
        }

        public async Task<IActionResult> FetchData(string record, [FromForm] List<LuckySheetCellModel> data, List<CostEstimateItemSearchResponseData> request, string type, int week)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                string template = IsSubUnit(sessionUser) ? @"./Resources/SmartMarkerSubTemplate.xlsx" : @"./Resources/SmartMarkerUnitTemplate.xlsx";
                Workbook workbook = new Workbook(template);

                WorkbookDesigner designer = new WorkbookDesigner(workbook);
                var dataLayer = new GenerateExcelCreateCostEstimateModel();

                #region Các biến dữ liệu trên cell

                double cashValue = 0.0;
                double techcomValue = 0.0;
                double bidvcomValue = 0.0;
                double bidvcomValue2 = 0.0;
                double amountAvailable = 0.0;

                double operatingValue = 0.0;
                double contribution = 0.0;
                double diffReceive = 0.0;
                double equityCostVal = 0.0;
                double rotationProposalValTu = 0.0;
                double rotationProposalValWe = 0.0;

                #endregion


                if (data.Count > 0)
                {
                    // Tiền mặt
                    var oCashValue = _findCellValue(data, "Tiền mặt");
                    cashValue = Math.Round(oCashValue.ToDouble(), MidpointRounding.AwayFromZero);
                    //Ngân hàng Techcombank cty

                    var tcbCompanyValue = _findCellValue(data, "Ngân hàng Techcombank cty");
                    techcomValue = Math.Round(tcbCompanyValue.ToDouble(), MidpointRounding.AwayFromZero);

                    // Ngân hàng BIDV
                    var bidvCompanyValue = _findCellValue(data, "Ngân hàng BIDV cty");
                    bidvcomValue = Math.Round(bidvCompanyValue.ToDouble(), MidpointRounding.AwayFromZero);

                    var bidvCompanyValue2 = _findCellValue(data, "Ngân hàng BIDV cty2 (TK chuyên thu)");
                    bidvcomValue2 = Math.Round(bidvCompanyValue2.ToDouble(), MidpointRounding.AwayFromZero);

                    var amountAvailableValue = _findCellValue(data, "Hạn mức tín dụng – Tổng số dư khả dụng");
                    amountAvailable = Math.Round(amountAvailableValue.ToDouble(), MidpointRounding.AwayFromZero);

                    var operatingValueCell = _findCellValue(data, "Thu từ hoạt động kinh doanh");
                    operatingValue = Math.Round(operatingValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                    var contributionValueCell = _findCellValue(data, "Thu từ hoạt động góp vốn");
                    contribution = Math.Round(contributionValueCell.ToDouble(), MidpointRounding.AwayFromZero);
                    var diffReceiveValueCell = _findCellValue(data, "Thu khác");
                    diffReceive = Math.Round(diffReceiveValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                    if (!IsSubUnit(sessionUser))
                    {
                        var equityValueCell = _findCellValue(data, "Định mức tồn quỹ (5)");
                        equityCostVal = Math.Round(equityValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                        var rotationTuValueCell = _findCellValue(data, "Chuyển về thứ 3");
                        rotationProposalValTu =
                            Math.Round(rotationTuValueCell.ToDouble(), MidpointRounding.AwayFromZero);

                        var rotationWeValueCell = _findCellValue(data, "Chuyển về thứ 5");
                        rotationProposalValWe =
                            Math.Round(rotationWeValueCell.ToDouble(), MidpointRounding.AwayFromZero);
                    }
                }

                designer.SetDataSource("Cash", cashValue);
                designer.SetDataSource("Techcombank", techcomValue);
                designer.SetDataSource("BIDV", bidvcomValue);
                designer.SetDataSource("BIDVReceive", bidvcomValue2);
                designer.SetDataSource("Available", amountAvailable);

                designer.SetDataSource("OperatingValue", operatingValue);
                designer.SetDataSource("ContributeValue", contribution);
                designer.SetDataSource("DiffReceiveValue", diffReceive);

                if (!IsSubUnit(sessionUser))
                {
                    designer.SetDataSource("EquityCost", equityCostVal);
                    designer.SetDataSource("RotationProposalTue", rotationProposalValTu);
                    designer.SetDataSource("RotationProposalWe", rotationProposalValWe);
                }

                // TRONG TUẦN 28 VÀ 29
                designer.SetDataSource("ActuallySpentMissing", $"III. BÁO CÁO CÁC KHOẢN DỰ TRÙ ĐÃ DUYỆT NHƯNG CHƯA CHI TRONG TUẦN {week - 2} VÀ {week - 1}");


                var w = DateTime.Now.Year.WeekInYear().FirstOrDefault(c => c.weekNum == week);

                var lstDecline = new List<CostEstimateItemSearchResponseData>();
                string reportName = $"DỰ TRÙ TUẦN {week} - {sessionUser.Unit.UnitName} (từ {w?.weekStart:dd/MM/yyyy} - {w?.weekFinish:dd/MM/yyyy})";
                if (week == 0) reportName = string.Empty;

                if (!string.IsNullOrEmpty(record))
                {
                    var dataView = await _costEstimateRepository.LoadCostEstimateItemsData(new CostEstimateViewRequest
                    {
                        Status = (int)GlobalEnums.StatusDefaultEnum.All,
                        PageRequest = CostElementConst.PublicKey,
                        Record = record,
                        UserName = sessionUser.UserName,
                        UnitName = sessionUser.Unit.UnitName,
                        DepartmentName = sessionUser.Department.DepartmentName,
                        UnitId = GetUserUnit().UnitId,
                        UserId = GetUserId(),
                        DepartmentId = sessionUser.Department.DepartmentId,
                        Positions = sessionUser.Positions,
                    });

                    if (dataView.Code == (int)GlobalEnums.ResponseCodeEnum.Success)
                    {
                        if (data.Count == 0 && !string.IsNullOrEmpty(dataView.CostEstimate.PathExcel))
                        {
                            string fullFile = $"{_physicalFolder}\\{dataView.CostEstimate.PathExcel}";
                            if (System.IO.File.Exists(fullFile))
                            {
                                var oldWorkbook = new Workbook(fullFile);
                                var oldMem = new MemoryStream();
                                oldWorkbook.Save(oldMem, SaveFormat.Xlsx);
                                oldMem.Position = 0;

                                return Json(new CostEstimateCreateResponse
                                {
                                    Code = (int)GlobalEnums.ResponseCodeEnum.Success,
                                    Message = ConvertToBase64(oldMem)
                                });
                            }
                        }

                        var operatingData = request.Where(c => c.CostEstimatePaymentType == 2).ToList();
                        var investData = request.Where(c => c.CostEstimatePaymentType == 3).ToList();
                        var financeData = request.Where(c => c.CostEstimatePaymentType == 1).ToList();
                        // trường hợp thêm mới hoặc cập nhật --> không có mục bị từ chối
                        if (!string.IsNullOrEmpty(type) && !type.Equals("create"))
                        {
                            lstDecline = request.Where(c => c.IsDeleted == (int)GlobalEnums.StatusDefaultEnum.Active).ToList();
                            foreach (var dcl in lstDecline)
                            {
                                if (dcl.Updater == 0)
                                {
                                    dcl.Updater = sessionUser.UserId;
                                    dcl.UpdaterName = sessionUser.UserName;
                                    dcl.UpdatedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                }
                            }
                        }

                        dataLayer.Operating = operatingData.Where(c => c.IsDeleted == (int)GlobalEnums.StatusDefaultEnum.InActive).ToList();
                        dataLayer.NOperating = lstDecline.Where(c => c.CostEstimatePaymentType == 2).ToList();

                        dataLayer.Investment = investData.Where(c => c.IsDeleted == (int)GlobalEnums.StatusDefaultEnum.InActive).ToList();
                        dataLayer.NInvest = lstDecline.Where(c => c.CostEstimatePaymentType == 3).ToList();

                        dataLayer.Finance = financeData.Where(c => c.IsDeleted == (int)GlobalEnums.StatusDefaultEnum.InActive).ToList();
                        dataLayer.NFinance = lstDecline.Where(c => c.CostEstimatePaymentType == 1).ToList();
                    }
                }
                else
                {
                    var operatingData = request.Where(c => c.CostEstimatePaymentType == 2).ToList();
                    var investData = request.Where(c => c.CostEstimatePaymentType == 3).ToList();
                    var financeData = request.Where(c => c.CostEstimatePaymentType == 1).ToList();

                    dataLayer.Operating = operatingData;
                    dataLayer.Investment = investData;
                    dataLayer.Finance = financeData;
                }

                designer.SetDataSource("Operating", dataLayer.Operating);
                designer.SetDataSource("Investment", dataLayer.Investment);
                designer.SetDataSource("Finance", dataLayer.Finance);
                designer.SetDataSource("Name", reportName);

                // không được chọn
                designer.SetDataSource("NOperating", dataLayer.NOperating);
                designer.SetDataSource("NInvest", dataLayer.NInvest);
                designer.SetDataSource("NFinance", dataLayer.NFinance);

                var notSpent = await _costEstimateItemRepository.GetCostEstimateItemNotSpent(week, sessionUser.Unit.UnitId, IsSubUnit(sessionUser), string.Empty);
                if (notSpent.Count > 0)
                {
                    var dataNoSpent = new List<NoSpentModel>();
                    foreach (var costEstimateItem in notSpent)
                    {
                        dataNoSpent.Add(new NoSpentModel
                        {
                            RequestCode = costEstimateItem.RequestCode,
                            StatusName = "Đã duyệt và chưa được chi",
                            Cost = costEstimateItem.Cost,
                            RequestContent = costEstimateItem.RequestContent,
                            Date = $"Tuần {costEstimateItem.PayWeek}"
                        });
                    }
                    designer.SetDataSource("MissingSpent", dataNoSpent);
                }


                CalculationOptions opts = new CalculationOptions { IgnoreError = true };

                designer.Process();

                workbook.CalculateFormula(opts);

                var cells = workbook.Worksheets[0].Cells;

                if (lstDecline.Count == 0)
                {
                    int startDeclineTable = 0;
                    int endDeclineTable = 0;
                    foreach (Row cellRow in cells.Rows)
                    {
                        string cellValue = cellRow[1].StringValue;
                        if (cellValue.StartsWith("II. Dự trù", StringComparison.CurrentCultureIgnoreCase))
                            startDeclineTable = cellRow.Index;
                        if (cellValue.StartsWith("III. BÁO CÁO", StringComparison.CurrentCultureIgnoreCase))
                            endDeclineTable = cellRow.Index;
                    }

                    cells.HideRows(startDeclineTable, endDeclineTable - startDeclineTable);
                }

                #region Đã duyệt nhưng chưa được chi


                if (notSpent.Count == 0)
                    foreach (Row cellRow in cells.Rows)
                    {
                        string cellValue = cellRow[1].StringValue;
                        if (cellValue.StartsWith("III. BÁO CÁO", StringComparison.CurrentCultureIgnoreCase))
                        {
                            cells.HideRows(cellRow.Index, 3);
                            break;
                        }
                    }


                #endregion

                await using var mem = new MemoryStream();
                workbook.Protect(ProtectionType.All, "MEDCOM@2021");
                var sheet = workbook.Worksheets[0];

                Style style;

                // Loop through all the columns in the worksheet and lock them.
                for (int i = 0; i <= 255; i++)
                {
                    style = sheet.Cells.Columns[(byte)i].Style;
                    style.IsLocked = true;
                    var styleFlag = new StyleFlag { Locked = true };
                    sheet.Cells.Columns[(byte)i].ApplyStyle(style, styleFlag);
                }

                var cellNotLocks = new[] { "D6", "D8", "D9", "D10", "D11", "D13", "D14", "D15" };
                foreach (var cellNotLock in cellNotLocks)
                {
                    style = sheet.Cells[cellNotLock].GetStyle();
                    style.IsLocked = false;
                    sheet.Cells[cellNotLock].SetStyle(style);
                }

                sheet.Protect(ProtectionType.All, "MEDCOM@2021", null);
                workbook.Save(mem, SaveFormat.Xlsx);
                mem.Position = 0;
                string base64Data = ConvertToBase64(mem);
                var rp = new CostEstimateCreateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Success,
                    Message = base64Data
                };
                return Json(rp);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return Json(new ApproveRequestOnCostEstimateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = GlobalEnums.ErrorMessage
                });
            }
        }

        /// <summary>
        /// Kế toán trưởng
        /// Chỉ load ra danh sách các yêu cầu đã đc phê duyệt bởi phòng ban
        /// </summary>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostElementItemView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> SearchRequestList(int length, int start, CostEstimateItemSearchRequest @base)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                @base.KeywordsNonUnicode = @base.Keywords.StringToNonUnicode();
                @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();
                @base.FileHostView = _fileHostView;
                @base.PermissionDelete = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostElementItemView, PermissionConstant.DELETE);
                @base.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostElementItemView, PermissionConstant.EDIT);
                @base.PermissionApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostElementItemView, PermissionConstant.APPROVE);
                @base.IsSub = IsSubUnit();
                @base.UserUnit = GetUserUnit().UnitId;
                @base.UserUnitsManages = await _userRepository.GetUserUnitManages(sessionUser.UserId);

                // nếu là >= kế toán thì set UserDepartmentId = -100
                // vì kế toán có thể nhìn thấy hết các yêu cầu đã duyệt của các phòng ban trong đơn vị
                @base.UserDepartmentId =
                    !ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfStaffAccountant))?.Order ?? 0, ComparingMode.GatherThan)
                        ? GetUserDepartment().DepartmentId
                        : (int)GlobalEnums.StatusDefaultEnum.All;

                // mã hóa khác so với trang danh sách yêu cầu
                @base.PageRequest = CostElementConst.PublicKey;

                //Chỉ lấy các yêu cầu đã được duyệt
                @base.Status = (int)GlobalEnums.StatusDefaultEnum.Active;
                @base.Status = (int)GlobalEnums.StatusDefaultEnum.Active;

                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.CostStatusesElementItem,
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
                @base.EstimateType = GlobalEnums.DefaultKeyToTypes.TryGetValue(GlobalEnums.Week, out var i) ? i : -1;
                var wiYear = DateTime.Now.Year.WeekInYear().OrderBy(c => c.weekNum)
                    .FirstOrDefault(c => c.weekStart >= DateTime.Now);
                @base.ReportWeek = wiYear?.weekNum ?? 0;

                if (!string.IsNullOrEmpty(@base.CurrentCostEstimateRecord))
                    @base.Current = Guid.TryParse(@base.CurrentCostEstimateRecord.StringAesDecryption(CostElementConst.PublicKey), out var g) ? g : Guid.Empty;

                var data = await _costEstimateItemRepository.SearchAsync(@base, start, length, sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut);
                return Json(data);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new CostEstimateItemSearchResponse
                {
                    Code = (int)HttpStatusCode.Forbidden,
                    Draw = @base.Draw,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!"
                });
            }
        }

        /// <summary>
        /// Tìm kiếm dự trù
        /// </summary>
        /// <param name="length"></param>
        /// <param name="start"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostEstimateView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> SearchEstimate(int length, int start, SearchCostEstimateRequest @base)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                @base.PageRequest = CostElementConst.PublicKey;
                @base.Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32();
                @base.PermissionApprove = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostEstimateView, PermissionConstant.APPROVE);
                @base.PermissionEdit = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostEstimateView, PermissionConstant.EDIT);
                @base.PermissionDelete = await _groupsRepository.IsAuthorize(GetUserPositionIds(),
                    Functions.CostEstimateView, PermissionConstant.DELETE);
                @base.IsSub = IsSubUnit();
                @base.UnitsManages = await _userRepository.GetUserUnitManages(sessionUser.UserId);

                // Lấy danh sách trạng thái mà đối tượng có thể xem
                var rqLoadStatusAllows = new DataSeenRequest
                {
                    Type = GlobalEnums.CostStatusesElement,
                    CostEstimateType = @base.RequestType,
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

                var requestType = GlobalEnums.DefaultKeyToTypes.TryGetValue(@base.RequestType.ToLower(), out var i) ? i : -1;
                @base.CostEstimateTypeId = requestType;

                bool searchAllUnit = ComparePositionLevelUpper((await _groupsRepository.GetByCode(GlobalEnums.DefaultCodeOfUnitManager))?.Order ?? 0, ComparingMode.GatherThan);
                // từ kế toán trưởng MG trở lên 
                // thì có xem tất cả đơn vị
                if (!searchAllUnit)
                    @base.UserUnit = GetUserUnit().UnitId;
                @base.HostFileView = _fileHostView;

                var data = await _costEstimateRepository.SearchAsync(@base, start, length, sessionUser.Unit.UnitType ?? GlobalEnums.UnitOut);
                return Json(data);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return Json(new SearchCostEstimateResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau!",
                    Draw = @base.Draw
                });
            }
        }

        /// <summary>
        /// Xem lịch sử phê duyệt / từ chối
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        [AuthorizeUser(Module = Functions.CostEstimateView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> ViewHistories(string record)
        {
            var request = new CostEstimateLogRequest
            {
                Record = record,
                PageRequest = CostElementConst.PublicKey
            };
            IList<CostEstimateLogResponse> data = null;
            try
            {
                data = await _costEstimateLogRepository.ViewHistories(request);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
            }
            return PartialView("_ViewHistory", data);
        }

        public async Task<string> _createPdfName(int year, int week, UnitModel unit)
        {
            try
            {
                int fileCounter =
                    await _pdfLogsRepository.CounterDay(unit.UnitId, CostElementConst.PublicKey);
                string templateName = $"{DateTime.Now:yyyy-MM-dd}_{year}_{unit.UnitName.StringToNonUnicode().StringRemoveSpecial()}_Du tru tuan_{week}_{fileCounter}.pdf";
                return templateName;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return string.Empty;
            }
        }

        public string ConvertToBase64(Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        #region Tìm dữ liệu trong data LuckyCell
        LuckySheetCellModel _findCell(List<LuckySheetCellModel> data, string word)
        {
            try
            {
                var oCellWithName = data.FirstOrDefault(c =>
                    c.v?.ct?.s?.Any(g =>
                        g.v?.Equals(word, StringComparison.CurrentCultureIgnoreCase) == true) == true);
                return oCellWithName;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        object _findCellValue(List<LuckySheetCellModel> data, int row, int col)
        {
            try
            {
                var oCellValue =
                    data.FirstOrDefault(cc => cc.r == row && cc.c == col)?.v?.v;
                return oCellValue;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        object _findCellValue(List<LuckySheetCellModel> data, string word)
        {
            var cellName = _findCell(data, word);
            if (cellName != null)
                return _findCellValue(data, cellName.r, cellName.c + 1);
            return null;
        }


        #endregion

        #region Tìm dữ liệu các cell trong excel theo nội dung cột

        double _getDoubleCellValue(Cells cell, string filter)
        {
            try
            {
                foreach (Row row in cell.Rows)
                {
                    var contentCell = row[2].StringValue;
                    if (contentCell.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                    {
                        try
                        {
                            return Math.Round(row[3].DoubleValue, MidpointRounding.AwayFromZero);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Error");
                            return Math.Round(row[3].StringValue.ToDouble(), MidpointRounding.AwayFromZero);
                        }
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return 0;
            }
        }

        #endregion
    }
}
