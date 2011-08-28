using System;

namespace MaintMan
{
    public class BadUrlException : Exception
    {
        public BadUrlException(string message) : base(message)
        {
        }
    }
}