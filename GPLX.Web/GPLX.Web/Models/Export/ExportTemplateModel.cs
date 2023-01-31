using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPLX.Web.Models.Export
{
    public class ExportTemplateModel
    {
    }

    public class PaymentExport
    {
        /// <summary>
        /// Nhóm dự trù
        /// </summary>
        public string PaymentName { get; set; }
        /// <summary>
        /// Nhóm chi phí
        /// </summary>
        public string CostEstimateTypeName { get; set; }
        public string ComparingType { get; set; }
    }

    public class PayFormExport
    {
        public int Number { get; set; }
        public string Value { get; set; }
    }
   
}
