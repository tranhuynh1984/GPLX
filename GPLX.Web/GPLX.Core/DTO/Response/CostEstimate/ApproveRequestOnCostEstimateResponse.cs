namespace GPLX.Core.DTO.Response.CostEstimate
{
    public class ApproveRequestOnCostEstimateResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Record { get; set; }

        public SearchCostEstimateResponseData Data { get; set; }
    }
}
