using System;
using Farfetch.OrderBatchProcessor.Instrumentation.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace Farfetch.OrderBatchProcessor.Instrumentation.Tests
{
    [TestClass]
    public class InstrumentationUnitTest
    {
        private ILoggingManager LoggingManager { get; set; }

        public InstrumentationUnitTest()
        {
            var kernel = NinjectBootstrapper.Get();
            this.LoggingManager = kernel.Get<ILoggingManager>();
        }

        [TestMethod]
        public void CallLoggingManagerWithInformationTest()
        {
            try
            {
                this.LoggingManager.LogInformation($"Test {DateTime.UtcNow}");
            }
            catch (Exception e)
            {
                Assert.Fail($"CallLoggingManagerWithInformationTest raised exception - {e.Message}");
            }
        }

        [TestMethod]
        public void CallLoggingManagerWithInformationAndEntityTest()
        {
            try
            {
                this.LoggingManager.LogInformation($"Test {DateTime.UtcNow} with parameter: ", new { test = 1, content = "abd" });
            }
            catch (Exception e)
            {
                Assert.Fail($"CallLoggingManagerWithInformationAndEntityTest raised exception - {e.Message}");
            }
        }

        [TestMethod]
        public void CallLoggingManagerWithExceptionTest()
        {
            try
            {
                this.LoggingManager.LogException(new Exception($"Test exception {DateTime.UtcNow}"));
            }
            catch (Exception e)
            {
                Assert.Fail($"CallLoggingManagerWithInformationTest raised exception - {e.Message}");
            }
        }
    }
}