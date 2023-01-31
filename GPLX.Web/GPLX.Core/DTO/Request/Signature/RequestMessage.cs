using System.Collections.Generic;

namespace GPLX.Core.DTO.Request.Signature
{
    public class RequestMessage
    {
        public string RequestID { get; set; }
        public string ServiceID { get; set; }
        public string FunctionName { get; set; }

        public object Parameter { get; set; }
    }
    public class Parameter
    {
        public string Email { get; set; }
    }

    // Certificate ----------------------------------------------------
    public class CertParameter : Parameter
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class CertResponse
    {
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public List<Certificate> Items { get; set; }
    }
    public class Certificate
    {
        public string ID { get; set; }
        public string CertBase64 { get; set; }
        // More properties, see json response
    }

    // ---------------------------------------------------------------

    // Signature -----------------------------------------------------
    public class SignParameter
    {
        public string CertID { get; set; }
        public string ServiceGroupID { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string DataBase64 { get; set; }
    }
    public class SignResponse
    {
        public string TranID { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public string SignedData { get; set; }
    }
    // ---------------------------------------------------------------

    // Verify response -----------------------------------------------
    public class VerifyResultModel
    {
        public string TranID { get; set; }
        public bool status { get; set; }
        public string message { get; set; }

        public List<SignServerVerifyResultModel> signatures { get; set; }
    }
    public class SignServerVerifyResultModel
    {
        public string signingTime { get; set; }
        public bool signatureStatus { get; set; }
        public string certStatus { get; set; }
        public string certificate { get; set; }
        public int signatureIndex { get; set; }
        public int code { get; set; }
    }
    public class GetTokenBody
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
