using System;

namespace GPLX.Core.DTO.Response
{
    public class ExceptionResultResponse : Exception
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string RawExceptionMessage { get; set; }
        public string InnerExceptionMessage { get; set; }

        public ExceptionResultResponse() { }

        public ExceptionResultResponse(int statusCode, string message = "") : base(message)
        {
            Code = statusCode;
            Message = message;
        }
        
        public ExceptionResultResponse(int statusCode, string message = "", string rawExceptionMessage = "") : base(message)
        {
            Code = statusCode;
            Message = message;
            RawExceptionMessage = rawExceptionMessage;
        }
    }
}