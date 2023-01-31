using System;

namespace GPLX.Core.Exceptions
{
    public class InvalidParameterException : Exception
    {
        public InvalidParameterException()
        {
        }

        public InvalidParameterException(string message)
            : base(message)
        {
        }
    }
}