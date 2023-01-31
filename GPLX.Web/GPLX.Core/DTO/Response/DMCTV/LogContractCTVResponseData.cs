using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.DMCTV
{
    public class LogContractCTVResponseData
    {
        public long LogCtvId { get; set; }
        public string DoctorId { get; set; }
        public int CTVGroupID { get; set; }
        public int SubId { get; set; }
        public string BP { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Note { get; set; }
        public string HTCK { get; set; }
        public bool IsHetHan { get; set; }
        public int IsActive { get; set; }
        public int ThuSau { get; set; }
        public int LogCtvStatus { get; set; }
    }
}