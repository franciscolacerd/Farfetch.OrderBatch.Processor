using System;
using System.Diagnostics;
using Farfetch.OrderBatchProcessor.Common;
using Newtonsoft.Json;
using Serilog;

namespace Farfetch.OrderBatchProcessor.Instrumentation.Logging
{
    public class LoggingManager : ILoggingManager
    {
        private static readonly TraceSwitch TraceSwitch = new TraceSwitch(
            Contants.TraceSwitch.Name,
            Contants.TraceSwitch.Description);

        public LoggingManager() => Log.Logger = new LoggerConfiguration()
            .WriteTo.EventLog(Contants.TraceSwitch.Source, Contants.TraceSwitch.Application, manageEventSource: true)
            .CreateLogger();

        public void LogInformation(string message)
        {
            if (TraceSwitch.TraceVerbose)
            {
                Log.Information("{Message}", message);

                Log.CloseAndFlush();
            }
        }

        public void LogInformation<T>(string message, T entity)
        {
            if (TraceSwitch.TraceError)
            {
                var json = JsonConvert.SerializeObject(entity, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                Log.Information("Information {Message}::{Json}", message, json);

                Log.CloseAndFlush();
            }
        }

        public void LogException(Exception exception)
        {
            if (TraceSwitch.TraceError)
            {
                var json = JsonConvert.SerializeObject(exception, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                Log.Error("Error {Json}", json);

                Log.CloseAndFlush();
            }
        }
    }
}