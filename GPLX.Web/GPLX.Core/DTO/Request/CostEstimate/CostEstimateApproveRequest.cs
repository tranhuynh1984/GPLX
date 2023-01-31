using System;
using System.Collections.Generic;
using GPLX.Core.DTO.Request.Signature;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.Extensions;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Request.CostEstimate
{
    public class CostEstimateApproveRequest
    {
        public string Record { get; set; }
        public Guid RawId => Guid.TryParse(Record.StringAesDecryption(PageRequest,true), out var g) ? g : Guid.Empty; 
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public IList<PositionModel> Positions { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitType { get; set; }
        public string Reason { get; set; }

        public int Status { get; set; }
        public string StatusName { get; set; }

        public bool IsApproval { get; set; }
        public string PageRequest { get; set; }
        public int Type { get; set; }

        public IEnumerable<StatusesGranted> StatusAllowsSeen { get; set; }
        public bool IsSub { get; set; }
        public string HostFileView { get; set; }
        public string NewPdf { get; set; }

        public SignOpts SignOpts { get; set; }
    }
}
