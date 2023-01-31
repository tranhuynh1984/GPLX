using System;
using System.Collections.Generic;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.Actually;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.Actually
{
    public class CreateActuallySpentRequest
    {
        public string Record { get; set; }
        public Guid RawId => !string.IsNullOrEmpty(Record) && 
                             !string.IsNullOrEmpty(RequestPage) ? 
            Guid.TryParse(Record.StringAesDecryption(RequestPage,true), out var g) ? g : Guid.Empty : Guid.Empty;
        public List<ActuallySpentItemResponse> Data { get; set; }

        public List<SCTView> SctData { get; set; }

        public int ReportForWeek { get; set; }
        public string ReportForWeekName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int Creator { get; set; }
        public string CreatorName { get; set; }
        public string RequestPage { get; set; }

        public int Status { get; set; }
        /// <summary>
        /// Đường dẫn file 111-112 backup
        /// </summary>
        public string PathBackup { get; set; }

        public IList<StatusesGranted> StatsAllowSeen { get; set; }
        public bool PermissionEdit { get; set; }
        public bool IsSub { get; set; }

    }
}
