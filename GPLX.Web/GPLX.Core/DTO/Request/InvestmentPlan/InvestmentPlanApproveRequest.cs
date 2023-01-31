using GPLX.Core.DTO.Response.CostStatus;
using System;
using System.Collections.Generic;
using GPLX.Core.Extensions;
using GPLX.Database.Models;

namespace GPLX.Core.DTO.Request.InvestmentPlan
{
    public class InvestmentPlanApproveRequest
    {
        public string Record { get; set; }
        public Guid RawId => Guid.TryParse(Record.StringAesDecryption(PageRequest, true), out var g) ? g : Guid.Empty;
        public bool IsApproval { get; set; }
        public string Reason { get; set; }
        public string PageRequest { get; set; }

        public int UnitId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UnitType { get; set; }

        public IEnumerable<StatusesGranted> StatusAllowsSeen { get; set; }
        public bool IsSub { get; set; }
        public string HostFileView { get; set; }

        public IList<PositionModel> Positions { get; set; }
        public string NewPdf { get; set; }

    }
}
