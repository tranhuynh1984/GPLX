using System;

namespace GPLX.Core.Exceptions
{
    public class UploadFileFailedException : Exception
    {
        public int Draw { get; private set; }
        
        public UploadFileFailedException()
        {
        }

        public UploadFileFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}