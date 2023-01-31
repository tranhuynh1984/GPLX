using System;
using System.Collections.Generic;
using System.Text;
using GPLX.Core.DTO.Response.UpdateTime;

namespace GPLX.Core.DTO.Response.Process
{
    public class ProcessSearchResponseData : UpdateTimeResponseData
    {
        public int Index { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
    }

    public class ProcessSearchResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<ProcessSearchResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}
