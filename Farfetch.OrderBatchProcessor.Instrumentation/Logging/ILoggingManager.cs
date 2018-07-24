using System;

namespace Farfetch.OrderBatchProcessor.Instrumentation.Logging
{
    public interface ILoggingManager
    {
        void LogInformation(string message);

        void LogInformation<T>(string message, T entity);

        void LogException(Exception exception);
    }
}