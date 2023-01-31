using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Aspose.Cells;

namespace GPLX.Infrastructure.Services
{
    public class ExcelService
    {
        public static Workbook ExportExcel(Dictionary<string, string> mappingHeader, IList<dynamic> dynamicListData, string sheetName = "") {
            try
            {
                var workbook = new Workbook();
                var sheet = workbook.Worksheets[0];
                sheet.Name = sheetName;

                if (mappingHeader?.Count <= 0) return workbook;
                if (dynamicListData?.Count <= 0) return workbook;
                
                object firstData = dynamicListData?.FirstOrDefault();
                string[] propertyNames = firstData?.GetType().GetProperties().Select(p => p.Name).ToArray();
                if (propertyNames?.Length <= 0) return workbook;
                
                foreach (var entry in mappingHeader.Select((Entry, Index) => new { Entry, Index }))
                {
                    var cell = sheet.Cells[1, entry.Index];
                    var style = cell.GetStyle();      
                    style.ForegroundColor = Color.RoyalBlue;
                    style.Pattern = BackgroundType.Solid;
                    style.Font.Color = Color.White;
                    style.HorizontalAlignment = TextAlignmentType.Center;
                    cell.SetStyle(style);
                    cell.PutValue(entry.Entry.Value);
                }

                for (int i = 0; i < dynamicListData?.Count; i++)
                {
                    var data = dynamicListData[i];
                    foreach (var entry in mappingHeader.Select((Entry, Index) => new { Entry, Index }))
                    {
                        var type = firstData.GetType();
                        var propertyInfo = type.GetProperty(entry.Entry.Key);
                        object propValue = propertyInfo?.GetValue(data, null);
                            
                        var cell = sheet.Cells[i + 2, entry.Index];
                        cell.PutValue(propValue);
                    }
                }
                sheet.AutoFitColumns();

                return workbook;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR When export excel {Ex}", ex);
                throw ex;
            }
        }

    }
}