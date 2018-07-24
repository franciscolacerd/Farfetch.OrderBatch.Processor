using System;

namespace Farfetch.OrderBatchProcessor.Common.Exceptions
{
    public class NoValidOrderFormatException : Exception
    {
        public NoValidOrderFormatException()
        {
        }

        public NoValidOrderFormatException(string message)
            : base(message)
        {
        }

        public NoValidOrderFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}