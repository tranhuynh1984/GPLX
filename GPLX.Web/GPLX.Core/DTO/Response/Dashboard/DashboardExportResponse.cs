using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.Dashboard
{
    public class DashboardExportResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string ExportPath { get; set; }
    }

    public class FileNPlanType
    {
        public string FilePath { get; set; }
        public string Type { get; set; }
    }
}
