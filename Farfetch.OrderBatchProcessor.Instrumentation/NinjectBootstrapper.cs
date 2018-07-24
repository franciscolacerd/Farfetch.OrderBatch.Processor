using Farfetch.OrderBatchProcessor.Instrumentation.Logging;
using Ninject.Modules;

namespace Farfetch.OrderBatchProcessor.Instrumentation
{
    public class NinjectBootstrapper : NinjectModule
    {
        public NinjectBootstrapper()
        {
        }

        public override void Load()
        {
            this.Bind<ILoggingManager>().To<LoggingManager>().InTransientScope();
        }
    }
}