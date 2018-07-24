using System;

namespace Farfetch.OrderBatchProcessor.Common.Exceptions
{
    public class CsvNotFoundException : Exception
    {
        public CsvNotFoundException()
        {
        }

        public CsvNotFoundException(string message)
            : base(message)
        {
        }

        public CsvNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}