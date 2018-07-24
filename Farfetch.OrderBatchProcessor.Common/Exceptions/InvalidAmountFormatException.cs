using System;

namespace Farfetch.OrderBatchProcessor.Common.Exceptions
{
    public class InvalidAmountFormatException : Exception
    {
        public InvalidAmountFormatException()
        {
        }

        public InvalidAmountFormatException(string message)
            : base(message)
        {
        }

        public InvalidAmountFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}