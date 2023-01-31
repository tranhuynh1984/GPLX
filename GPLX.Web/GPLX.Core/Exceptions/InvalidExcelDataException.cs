using System;

namespace GPLX.Core.Exceptions
{
    public class InvalidExcelDataException : Exception
    {
        public InvalidExcelDataException()
        {
        }

        public InvalidExcelDataException(string message) : base(message)
        {
        }
        
        public InvalidExcelDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}