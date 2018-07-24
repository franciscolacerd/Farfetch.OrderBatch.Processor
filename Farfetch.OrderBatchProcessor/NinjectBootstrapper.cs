using Ninject;

namespace Farfetch.OrderBatchProcessor
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