using Farfetch.OrderBatchProcessor.DomainModel.Order;
using Ninject.Modules;

namespace Farfetch.OrderBatchProcessor.DomainModel
{
    public class SelfNinjectBootstrapper : NinjectModule
    {
        public SelfNinjectBootstrapper()
        {
        }

        public override void Load()
        {
            this.Bind<IOrderDomainModel>().To<Order.OrderDomainModel>().InTransientScope();
        }
    }
}