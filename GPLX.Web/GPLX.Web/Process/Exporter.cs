using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Aspose.Cells;
using iTextSharp.text;
using iTextSharp.text.pdf;
using GPLX.Core.DTO.Response.Dashboard;
using GPLX.Core.Enum;
using GPLX.Infrastructure.Extensions;
using Serilog;

namespace GPLX.Web.Process
{
    public class Exporter
    {
        public DashboardExportResponse Export(IList<FileNPlanType> excelPaths, IList<FileNPlanType> pdfPaths, IList<int> listUnitIds, string exportType, string localPath, string hostView)
        {
            var rt = new DashboardExportResponse();
            try
            {
                var groupUnits = listUnitIds.Distinct().ToList();
                if (groupUnits.Count > 1)
                {
                    // cho vào zip

                    string randomFile = $"{localPath}\\Tong_Hop_Ke_Hoach_{DateTime.Now:yyyy-MM-dd}_{Guid.NewGuid():N}.zip";
                    if (exportType.Equals("pdf"))
                    {
                        CreateZipFile(randomFile, pdfPaths.Select(c => c.FilePath));
                        rt.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    }
                    else
                    {
                        var zip = ZipFile.Open(randomFile, ZipArchiveMode.Create);

                        for (int i = 0; i < excelPaths.Count; i++)
                        {
                            var templateName = pdfPaths[i].FilePath;
                            templateName = Path.ChangeExtension(templateName, "xlsx");
                            // Add the entry for each file
                            zip.CreateEntryFromFile(excelPaths[i].FilePath, Path.GetFileName(templateName), CompressionLevel.Optimal);
                            // Dispose of the object when we are done
                        }
                        zip.Dispose();
                        rt.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    }

                    rt.ExportPath = randomFile.Replace(localPath, string.Empty).NormalizePath();
                    rt.ExportPath = $"{hostView}/{rt.ExportPath}";
                }
                else
                {
                    exportType = exportType.ToLower();
                    switch (exportType)
                    {
                        case "pdf":
                            if (pdfPaths.Count == 1)
                            {
                                rt.ExportPath = $"{hostView}/{pdfPaths[0].FilePath.Replace(localPath, string.Empty).NormalizePath()}";
                                rt.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                            }
                            else
                            {
                                string randomFile = $"{localPath}\\Tong_Hop_Ke_Hoach_{DateTime.Now:yyyy-MM-dd}_{Guid.NewGuid():N}.pdf";
                                if (File.Exists(randomFile))
                                    File.Delete(randomFile);
                                bool createMergePdFs = MergePDFs(pdfPaths.Select(c => c.FilePath), randomFile);
                                if (createMergePdFs)
                                {
                                    rt.ExportPath = randomFile.Replace(localPath, string.Empty).NormalizePath();
                                    rt.ExportPath = $"{hostView}/{rt.ExportPath}";
                                    rt.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                                }
                                else
                                {
                                    rt.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                    rt.Message = "Lỗi tạo PDF";
                                }
                            }
                            break;
                        case "xlsx":
                            if (pdfPaths.Count == 1)
                            {
                                rt.ExportPath = $"{hostView}/{excelPaths[0].FilePath.Replace(localPath, string.Empty).NormalizePath()}";
                                rt.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                            }
                            else
                            {
                                string randomFile = $"{localPath}\\Tong_Hop_Ke_Hoach_{DateTime.Now:yyyy-MM-dd}_{Guid.NewGuid():N}.xlsx";
                                if (File.Exists(randomFile))
                                    File.Delete(randomFile);

                                File.Create(randomFile).Close();
                                var mergeExcelWorkbook = new Workbook();
                                int wsCounter = 0;
                                foreach (var fileNPlanType in excelPaths)
                                {
                                    var wbPlan = new Workbook(fileNPlanType.FilePath);
                                    var wsPlan = wbPlan.Worksheets.Cast<Worksheet>().Where(c => c.IsVisible).ToList();

                                    Worksheet dataWorksheet = wsPlan.FirstOrDefault(m => !m.Name.Contains("Hướng dẫn", StringComparison.OrdinalIgnoreCase));
                                    if (wsCounter > 0)
                                        mergeExcelWorkbook.Worksheets.Add();
                                    mergeExcelWorkbook.Worksheets[wsCounter].Copy(dataWorksheet);

                                    var sheetName = _sheetName(fileNPlanType.Type.ToLower());
                                    var countSheetFounds = mergeExcelWorkbook.Worksheets
                                        .Cast<Worksheet>().Count(cc => cc.Name.StartsWith(sheetName));
                                    
                                    mergeExcelWorkbook.Worksheets[wsCounter].Name = countSheetFounds == 0 ? sheetName : $"{sheetName}_{countSheetFounds + 1}";
                                    wsCounter++;
                                }

                                mergeExcelWorkbook.Save(randomFile, SaveFormat.Xlsx);

                                rt.ExportPath = randomFile.Replace(localPath, string.Empty).NormalizePath();
                                rt.ExportPath = $"{hostView}/{rt.ExportPath}";
                                rt.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                rt.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                rt.Message = GlobalEnums.ErrorMessage;
            }

            return rt;
        }

        static bool MergePDFs(IEnumerable<string> fileNames, string targetPdf)
        {
            try
            {
                bool merged = true;
                using (FileStream stream = new FileStream(targetPdf, FileMode.Create))
                {
                    Document document = new Document();
                    PdfCopy pdf = new PdfCopy(document, stream);
                    PdfReader reader = null;
                    try
                    {
                        document.Open();
                        foreach (string file in fileNames)
                        {
                            reader = new PdfReader(file);
                            pdf.AddDocument(reader);
                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error");
                        merged = false;
                        reader?.Close();
                    }
                    finally
                    {
                        document.Close();
                    }
                }
                return merged;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        static string _sheetName(string planType)
        {
            switch (planType)
            {
                case "revenue":
                    return "kế hoạch Doanh thu & KH";
                case "profit":
                    return "Kế hoạch Lợi nhuận";
                case "investment":
                    return "Kế hoạch Đầu tư";
                case "cashfollow":
                    return "Kế hoạch Dòng tiền";
            }

            return string.Empty;
        }

        static void CreateZipFile(string fileName, IEnumerable<string> files)
        {
            // Create and open a new ZIP file
            var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
            foreach (var file in files)
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
            }
            // Dispose of the object when we are done
            zip.Dispose();
        }
    }
}
