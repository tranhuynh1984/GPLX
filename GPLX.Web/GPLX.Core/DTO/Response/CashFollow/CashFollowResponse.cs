using System;
using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.CashFollow
{
    public class CashFollowResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<CashFollowResponseData> Data { get; set; }
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }

    }

    public class CashFollowResponseData
    {
        public string Record { get; set; }
        public string Creator { get; set; }
        public int Year { get; set; }
        public string PathFile { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string UnitName { get; set; }
        public bool Viewable { get; set; }
        public bool Approvalable { get; set; }
        public bool Deleteable { get; set; }
        public bool Declineable { get; set; }
        public bool Editable { get; set; }

        /// <summary>
        /// Tổng doanh thu
        /// </summary>
        public double TotalRevenue { get; set; }
        /// <summary>
        /// Tổng chi phí
        /// </summary>
        public double TotalSpending { get; set; }
        /// <summary>
        /// Tổng luân chuyển
        /// </summary>
        public double TotalCirculation { get; set; }
    }

}
