using System.Collections.Generic;

namespace GPLX.Core.DTO.Response.ProfitPlan
{
    public class ProfitPlanExcelUploadResponse
    {
       
            public IList<ProfitPlanAggregatesExcel> ProfitPlanAggregates { get; set; }
            public IList<ProfitPlanDetailExcel> ProfitPlanDetails { get; set; }
            public Database.Models.ProfitPlan ProfitPlan { get; set; }
    }


    public class ProfitPlanAggregatesExcel
    {
        public int ProfitPlanGroupId { get; set; }
        public string ProfitPlanGroupName { get; set; }
        /// <summary>
        /// Số tiền
        /// </summary>
        public double TotalCosh { get; set; }
        /// <summary>
        /// Tl/DT
        /// </summary>
        public double Proportion { get; set; }

        public string Style { get; set; }
        /// <summary>
        /// Thứ tự
        /// </summary>
        public string No { get; set; }
    }

    public class ProfitPlanDetailExcel
    {
        public int ProfitPlanGroupId { get; set; }
        public string ProfitPlanGroupName { get; set; }

        public double M1 { get; set; }
        public double M2 { get; set; }
        public double M3 { get; set; }
        public double M4 { get; set; }
        public double M5 { get; set; }
        public double M6 { get; set; }
        public double M7 { get; set; }
        public double M8 { get; set; }
        public double M9 { get; set; }
        public double M10 { get; set; }
        public double M11 { get; set; }
        public double M12 { get; set; }

        public double Total { get; set; }
        /// <summary>
        /// Tỉ lệ /doanh thu
        /// </summary>
        public float ProPortion { get; set; }

        public string Style { get; set; }
        /// <summary>
        /// Thứ tự
        /// </summary>
        public string No { get; set; }

        public int Row { get; set; }
    }


}
