using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.Enum
{
    public static class CostEstimateItemLogEnums
    {
        // loại log trong bảng CostEstimateItemLog
        // phải phân biệt 2 trạng thái
        // trạng thái của yêu cầu gốc
        // trạng thái trong phiếu
        public static readonly string LogTypeRequest = "REQUEST";
        public static readonly string LogTypeCostEstimate = "COST-ESTIMATE";
    }
}
