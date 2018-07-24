using Ninject;

namespace Farfetch.OrderBatchProcessor.Instrumentation.Tests
{
    public static class NinjectBootstrapper
    {
        public static StandardKernel Get()
        {
            var kernel = new StandardKernel();
            kernel.Load("*.dll");
            return kernel;
        }
    }
}