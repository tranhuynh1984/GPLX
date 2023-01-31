using Microsoft.AspNetCore.Mvc;
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
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.HDCTV;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.HDCTV;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.HDCTV;
using GPLX.Core.Enum;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GPLX.Core.Contracts.TBL_CTVGROUPSUB;
using GPLX.Core.DTO.Request.TBL_CTVGROUPSUB;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB;
using GPLX.Core.Contracts.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.Contracts.TBL_CTVGROUPSUB2_DETAIL;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB2_DETAIL;
using GPLX.Core.DTO.Request.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.DTO.Request.TBL_CTVGROUPSUB2_DETAIL;
using GPLX.Core.Contracts.DMCP;
using GPLX.Core.DTO;
using GPLX.Core.DTO.Entities;
using GPLX.Core.DTO.Request.DMCP;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.Actually;
using GPLX.Core.Exceptions;
using GPLX.Core.Extensions;
using GPLX.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using HttpContentMediaTypes = GPLX.Core.Contants.HttpContentMediaTypes;

namespace GPLX.Web.Controllers
{
    public class HDCTVController : BaseController
    {
        private readonly ILogger<HDCTVController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;
        private readonly ITBL_CTVGROUPSUBRepository _TBL_CTVGROUPSUBRepository;
        private readonly ITBL_CTVGROUPSUB1_DETAILRepository _TBL_CTVGROUPSUB1_DETAILRepository;
        private readonly ITBL_CTVGROUPSUB2_DETAILRepository _TBL_CTVGROUPSUB2_DETAILRepository;
        private readonly IDMCPRepository _DMCPRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _defaultRootFolder;
        private readonly StorageConfig _storageConfig;
        public HDCTVController(
            IWebHostEnvironment webHostEnvironment,
            IDMCPRepository dMCPRepository,
            ITBL_CTVGROUPSUBRepository tBL_CTVGROUPSUBRepository, 
            ITBL_CTVGROUPSUB1_DETAILRepository tBL_CTVGROUPSUB1_DETAILRepository, 
            ITBL_CTVGROUPSUB2_DETAILRepository tBL_CTVGROUPSUB2_DETAILRepository, 
            ILogger<HDCTVController> logger, 
            IMedAuthenticateConnect authenticateConnect, 
            IConfiguration configuration)
        {
            _TBL_CTVGROUPSUBRepository = tBL_CTVGROUPSUBRepository;
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
            _TBL_CTVGROUPSUB1_DETAILRepository = tBL_CTVGROUPSUB1_DETAILRepository;
            _TBL_CTVGROUPSUB2_DETAILRepository = tBL_CTVGROUPSUB2_DETAILRepository;
            _DMCPRepository = dMCPRepository;
            _webHostEnvironment = webHostEnvironment;
            _storageConfig = configuration.StorageConfig();
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Create(int SubId = 0, int GroupId = 1)
        {
            HDCTVDetailSearchResponse model = new HDCTVDetailSearchResponse();
            model.GroupId = GroupId;
            if (GroupId == 1)
                model.GroupName = "Nhóm I: Bác sĩ";
            else if (GroupId == 2)
                model.GroupName = "Nhóm II: CTV ngành Y";
            else if (GroupId == 3)
                model.GroupName = "Nhóm III: CTV ngoài ngành Y";
            model.SubId = SubId;
            
            model.CTVGROUPSUB = new TBL_CTVGROUPSUBSearchResponseData();
            model.CTVGROUPSUB.IsUse = 1;
            model.CTVGROUPSUB.FromDate = DateTime.Now;
            model.CTVGROUPSUB.ToDate = DateTime.Now;
            if (SubId>0)
            {
                model.CTVGROUPSUB = await _TBL_CTVGROUPSUBRepository.GetById(SubId);
            }
            
            DMCPSearchRequest requestdmcp = new DMCPSearchRequest();
            model.ListDMCP = await _DMCPRepository.SearchAll(requestdmcp);

            return View(model);
        }

        public async Task<IActionResult> Search(int length, int start, TBL_CTVGROUPSUBSearchRequest @base)
        {
            TBL_CTVGROUPSUBSearchResponse data = new TBL_CTVGROUPSUBSearchResponse();

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            data = await _TBL_CTVGROUPSUBRepository.Search(start, length, @base);

            return Json(data);
        }
        public async Task<IActionResult> SearchCreateType1(int length, int start, TBL_CTVGROUPSUB1_DETAILSearchRequest @base)
        {
            TBL_CTVGROUPSUB1_DETAILSearchResponse data = new TBL_CTVGROUPSUB1_DETAILSearchResponse();

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            //@base.IsUse = -1;
            data = await _TBL_CTVGROUPSUB1_DETAILRepository.Search(start, length, @base);

            return Json(data);
        }

        public async Task<IActionResult> SearchCreateType2(int length, int start, TBL_CTVGROUPSUB2_DETAILSearchRequest @base)
        {
            TBL_CTVGROUPSUB2_DETAILSearchResponse data = new TBL_CTVGROUPSUB2_DETAILSearchResponse();

            @base.Draw = Request.Query["draw"].ToString().ToInt32();
            @base.RequestPage = DepartmentConst.PublicKey;
            //@base.IsUse = -1;
            data = await _TBL_CTVGROUPSUB2_DETAILRepository.Search(start, length, @base);

            return Json(data);
        }

        public async Task<IActionResult> OnCreateHD(TBL_CTVGROUPSUBCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _TBL_CTVGROUPSUBRepository.Create(request);

            return Json(response);
        }

        public async Task<IActionResult> OnCreateType1(TBL_CTVGROUPSUB1_DETAILCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _TBL_CTVGROUPSUB1_DETAILRepository.Create(request);

            return Json(response);
        }

        public async Task<IActionResult> OnCreateType2(TBL_CTVGROUPSUB2_DETAILCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _TBL_CTVGROUPSUB2_DETAILRepository.Create(request);

            return Json(response);
        }
        public async Task<IActionResult> OnRemoveType1(TBL_CTVGROUPSUB1_DETAILCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _TBL_CTVGROUPSUB1_DETAILRepository.Remove(request);
            //var response = new TBL_CTVGROUPSUB1_DETAILCreateResponse();

            return Json(response);
        }

        public async Task<IActionResult> OnRemoveSub(TBL_CTVGROUPSUBCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _TBL_CTVGROUPSUBRepository.Remove(request);

            return Json(response);
        }

        public async Task<IActionResult> OnRemoveType2(TBL_CTVGROUPSUB2_DETAILCreateRequest request)
        {
            request.Creator = GetUserId();
            request.CreatorName = GetUserSyncId();
            var response = await _TBL_CTVGROUPSUB2_DETAILRepository.Remove(request);
            //var response = new TBL_CTVGROUPSUB1_DETAILCreateResponse();

            return Json(response);
        }

        public async Task<IActionResult> OnSCTDataUpload()
        {
            try
            {
                var formType = Request.Form["formType"];
                var subId = Request.Form["subId"];
                if (string.IsNullOrWhiteSpace(formType))
                {
                    var excelUploadResponse = new ExcelUploadResponse<HdctvImportExcelBaseResponse>();
                    excelUploadResponse.AddError(new ExcelValidatorError { Message = "Loại hợp đồng không hợp lệ!" });
                    excelUploadResponse.Code = (int) GlobalEnums.ResponseCodeEnum.NoContent;
                    return Json(excelUploadResponse);
                }
                
                if (string.IsNullOrWhiteSpace(subId))
                {
                    var excelUploadResponse = new ExcelUploadResponse<HdctvImportExcelBaseResponse>();
                    excelUploadResponse.AddError(new ExcelValidatorError { Message = "Hợp đồng không tồn tại!" });
                    excelUploadResponse.Code = (int) GlobalEnums.ResponseCodeEnum.NoContent;
                    return Json(excelUploadResponse);
                }
                
                var groupSub = await _TBL_CTVGROUPSUBRepository.GetById(subId[0].ToInt16());
                if (groupSub == null)
                {
                    var excelUploadResponse = new ExcelUploadResponse<HdctvImportExcelBaseResponse>();
                    excelUploadResponse.AddError(new ExcelValidatorError { Message = "Hợp đồng không tồn tại!" });
                    excelUploadResponse.Code = (int) GlobalEnums.ResponseCodeEnum.NoContent;
                    return Json(excelUploadResponse);
                }
                
                var fileOnRequest = Request.Form.Files;
                if (fileOnRequest.Count == 0)
                {
                    var excelUploadResponse = new ExcelUploadResponse<HdctvImportExcelBaseResponse>();
                    excelUploadResponse.AddError(new ExcelValidatorError { Message = "Không có tệp nào được tải lên" });
                    excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;

                    return Json(excelUploadResponse);
                }
                
                if (formType[0].ToInt16() == 1)
                {
                    var excelUploadResponse = new ExcelUploadResponse<HdctvType1UploadResponse>();
                    var result =  await ProcessHdctvForm<HdctvType1UploadResponse>(
                        fileOnRequest, subId[0].ToInt16(), "HDCTV_N1", HdctvDataConst.SheetType1.name, typeof(HdctvType1UploadResponse)).ConfigureAwait(false);
                    result.SubId = subId[0].ToInt16();
                    var insertDBResponse = await _TBL_CTVGROUPSUB1_DETAILRepository.CreateRange(result);
                    excelUploadResponse.AddErrorRange(insertDBResponse.ErrorList.Select(x => new ExcelValidatorError
                    {
                        Column = x.Key,
                        Message = x.Reason
                    }));
                    excelUploadResponse.Data = result.ImportData;
                    excelUploadResponse.ExcelFileError = result.ErrorFile;
                    excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    return Json(excelUploadResponse);
                }
                
                if (formType[0].ToInt16() == 2)
                {
                    var excelUploadResponse = new ExcelUploadResponse<HdctvType2UploadResponse>();
                    var result = await ProcessHdctvForm<HdctvType2UploadResponse>(
                        fileOnRequest, subId[0].ToInt16(), "HDCTV_N2", HdctvDataConst.SheetType2.name, typeof(HdctvType2UploadResponse)).ConfigureAwait(false);
                    result.SubId = subId[0].ToInt16();
                    var insertDBResponse = await _TBL_CTVGROUPSUB2_DETAILRepository.CreateRange(result);
                    excelUploadResponse.AddErrorRange(insertDBResponse.ErrorList.Select(x => new ExcelValidatorError
                    {
                        Column = x.Key,
                        Message = x.Reason
                    }));
                    excelUploadResponse.Data = result.ImportData;
                    excelUploadResponse.ExcelFileError = result.ErrorFile;
                    excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    return Json(excelUploadResponse);
                }
                
                var defaultResponse = new ExcelUploadResponse<HdctvImportExcelBaseResponse>();
                defaultResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                return Json(defaultResponse);
            }
            catch (Exception e)
            {
                var excelUploadResponse = new ExcelUploadResponse<HdctvImportExcelBaseResponse>();
                if (e is InvalidExcelDataException)
                {
                    excelUploadResponse.AddError(new ExcelValidatorError { Message = e.Message });
                    excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    return Json(excelUploadResponse);
                }
                
                _logger.Log(LogLevel.Error, e, e.Message);
                excelUploadResponse.AddError(new ExcelValidatorError { Message = "Có lỗi xảy ra, vui lòng thử lại sau!" });
                excelUploadResponse.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                return Json(excelUploadResponse);
            }
        }

        private async Task<HdctvImportExcel<T>> ProcessHdctvForm<T>(IFormFileCollection fileOnRequest, int subId, string validatorName, string sheetName, Type oType) where T: HdctvImportExcelBaseResponse
        {
            var hdctvImportExcelResponse = new HdctvImportExcel<T>();
            var excelFile = fileOnRequest[0];
            string randomFolderName = "temporary";
            var folder = Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder, randomFolderName);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
                
            var fileCreateName = $"{Path.GetRandomFileName()}{Path.GetExtension(excelFile.FileName)}";
            var file = Path.Combine(folder, fileCreateName);
            await using var fileStream = new FileStream(file, FileMode.Create);
            await excelFile.CopyToAsync(fileStream);
            await fileStream.DisposeAsync();
            fileStream.Close();
            
            var numberCode = new Regex("[0-9]+");
            var workBook = new Workbook(file);
            Worksheet worksheet = workBook.Worksheets.Cast<Worksheet>().FirstOrDefault(x => x.Name.Trim() == sheetName && x.IsVisible);
            if (worksheet == null)
            {
                throw new InvalidExcelDataException("File import không hợp lệ. Bạn vui lòng kiểm tra lại file!");
            }
            var excelValidator = new ExcelFormValidation(_configuration);
            var dmcp = await _DMCPRepository.SearchAll(new DMCPSearchRequest());
            var dmcpByMa = dmcp.Data.ToDictionary(x => x.MaCP, x => x.TenCP);
            excelValidator = excelValidator.Load($"ExcelFormValidator:{validatorName}");
            IList<ExcelRow> excelRows = new List<ExcelRow>();
            Row headerRow = worksheet.Cells.Rows[0];
            var headerPositionByName = new Dictionary<string, int>();
            var headerPosition = 0;
            foreach (Cell headerCell in headerRow)
            {
                var headerName = headerCell.StringValue.Trim();
                if (!headerPositionByName.ContainsKey(headerName))
                {
                    headerPositionByName.Add(headerName, headerPosition);
                }
                headerPosition++;
            }
            
            for (var index = 1; index < worksheet.Cells.Rows.Count; index++)
            {
                Row cellsRow = worksheet.Cells.Rows[index];
                if (cellsRow.IsHidden) continue;

                var fCell = cellsRow.FirstCell.Row;
                var cellsInRow = new List<ExcelCell>();
                foreach (var xCellColumnValidator in excelValidator.ColumnConfigs)
                {
                    var hasColumn = headerPositionByName.TryGetValue(xCellColumnValidator.Name.Trim(), out int position);
                    if (!hasColumn) continue; 
                    
                    var dataAt = cellsRow[position];
                    var cell = excelValidator.ReadCellAt(xCellColumnValidator, dataAt);
                    cell.CellPosition = position;

                    var hasMaCP = headerPositionByName.TryGetValue("MaCP", out int maCpPosition);
                    var isMaCP = hasMaCP && maCpPosition == position;
                    if (isMaCP)
                    {
                        if (!dmcpByMa.ContainsKey(cell.StringCellValue.Trim()))
                        {
                            cell.IsNotValidCell = true;
                            cell.ErrorMessage = "Mã không tồn tại!";
                            cellsInRow.Add(cell);
                            continue;
                        }
                        
                        var hasTenCP = headerPositionByName.TryGetValue("TenCP", out int tenCpPosition);
                        if (!hasTenCP) continue;
                        var tenCP = cellsRow.GetCellOrNull(tenCpPosition)?.StringValue?.Trim();
                        if (dmcpByMa[cell.StringCellValue.Trim()] != tenCP)
                        {
                            cell.IsNotValidCell = true;
                            cell.ErrorMessage = "Tên DV không đúng!";
                            cellsInRow.Add(cell);
                            continue;
                        }
                    }
                    
                    cellsInRow.Add(cell);
                }

                string cellContent = cellsInRow.FirstOrDefault(x => x.CellPosition == 1)?.StringCellValue;
                if (!string.IsNullOrEmpty(cellContent))
                    excelRows.Add(new ExcelRow { Cells = cellsInRow, RowIndex = fCell }); 
            }
            
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
                        if (string.IsNullOrEmpty(excelRowCell.FieldMapper) || excelRowCell.ReaderVal == null)
                            continue;
                        
                        var field = oType.GetProperty(excelRowCell.FieldMapper, BindingFlags.Instance | BindingFlags.Public);
                        if (field == null) continue;
                        
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
                    
                    var oExcelRow = (T) instanceOf;
                    oExcelRow.Type = numberCode.Match(HdctvDataConst.SheetType1.name).Value;
                    oExcelRow.FileName = fileCreateName;
                    oExcelRow.Path = Path.Combine(_defaultRootFolder, randomFolderName, fileCreateName);
                    oExcelRow.SubId = subId;
                    hdctvImportExcelResponse.AddImportData(oExcelRow);
                }
                
                return hdctvImportExcelResponse;
            }

            var errorFileName = rcValidate.CreateErrorFile(workBook, worksheet, folder, excelFile.FileName);
            string errorFilePath = _defaultRootFolder + "/" + randomFolderName  + "/" + errorFileName;
            if (string.IsNullOrEmpty(errorFilePath))
            {
                throw new InvalidExcelDataException("Không thể tạo file excel lỗi!");
            }
            
            hdctvImportExcelResponse.ErrorFile = errorFilePath;
            return hdctvImportExcelResponse;
        }

        [HttpPost]
        public async Task<IActionResult> OnFileUpload()
        {
            var excelUploadResponse = new ExcelUploadResponse<FileResponse>();
            var files = await StorageService.UploadFiles(
                _webHostEnvironment.WebRootPath, 
                _storageConfig, 
                "hdctv/",
                Request.Form.Files);
            excelUploadResponse.Code = (int) GlobalEnums.ResponseCodeEnum.Success;
            excelUploadResponse.Data = files;
            return Json(excelUploadResponse);
        }
        
        public async Task<IActionResult> ExportExcel(int subId, int formType)
        {
            Workbook workbook = null;
            if (formType == 1)
            {
                var data = await _TBL_CTVGROUPSUB1_DETAILRepository.GetAllBySubId(subId);
                var mappingHeader = new Dictionary<string, string>
                {
                    ["Index"] = "STT",
                    ["MaCP"] = "Mã DV",
                    ["TenCP"] = "Tên DV",
                    ["BP1"] ="BP1",
                    ["BP2"] ="BP2",
                    ["BP3"] ="BP3",
                    ["BP4"] ="BP4",
                    ["BP5"] ="BP5",
                    ["BP6"] ="BP6",
                    ["BP7"] ="BP7",
                    ["BP8"] ="BP8",
                    ["BP9"] ="BP9",
                    ["BP10"] ="BP10",
                    ["BP11"] ="BP11",
                    ["IsActiveName"] ="Trạng thái",
                };
                workbook = ExcelService.ExportExcel(mappingHeader, data.Cast<dynamic>().ToList(), "Hop_dong_CTV_N1");
            } else if (formType == 2)
            {
                var data = await _TBL_CTVGROUPSUB2_DETAILRepository.GetAllBySubId(subId);
                var mappingHeader = new Dictionary<string, string>
                {
                    ["Index"] = "STT",
                    ["MaCP"] = "Mã DV",
                    ["TenCP"] = "Tên DV",
                    ["FixedPrice"] ="Giá ĐB",
                    ["IsActiveName"] ="Trạng thái",
                };
                workbook = ExcelService.ExportExcel(mappingHeader, data.Cast<dynamic>().ToList(), "Hop_dong_CTV_N2");
            }

            if (workbook == null)
            {
                throw new InvalidExcelDataException("Không thể xuất dữ liệu!");
            }
            
            using (var memoryStream = new MemoryStream())
            {
                workbook.Save(memoryStream, new OoxmlSaveOptions(SaveFormat.Xlsx));
                memoryStream.Position = 0;
                byte[] sheetData = memoryStream.ToArray();
                return File(sheetData, HttpContentMediaTypes.XLSX, $"Hop_dong_CTV_{subId}.xlsx");
            }
        }
    }
}
