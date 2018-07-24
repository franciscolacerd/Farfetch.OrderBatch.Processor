using System;

namespace Farfetch.OrderBatchProcessor.Common.Exceptions
{
    public class EmptyDocumentException : Exception
    {
        public EmptyDocumentException()
        {
        }

        public EmptyDocumentException(string message)
            : base(message)
        {
        }

        public EmptyDocumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}