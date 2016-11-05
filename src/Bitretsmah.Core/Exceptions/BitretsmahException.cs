using System;

namespace Bitretsmah.Core.Exceptions
{
    public class BitretsmahException : Exception
    {
        public BitretsmahException()
            : base()
        {
        }

        public BitretsmahException(string message)
            : base(message)
        {
        }
    }
}