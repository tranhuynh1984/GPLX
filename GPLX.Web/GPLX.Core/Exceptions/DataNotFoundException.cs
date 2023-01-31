using System;

namespace GPLX.Core.Exceptions
{
    public class DataNotFoundException : Exception
    {
        public int Draw { get; private set; }
        
        public DataNotFoundException()
        {
        }

        public DataNotFoundException(string message, int draw = 0)
            : base(message)
        {
            Draw = draw;
        }

        public DataNotFoundException(string message, Exception inner, int draw = 0)
            : base(message, inner)
        {
            Draw = draw;
        }
    }
}