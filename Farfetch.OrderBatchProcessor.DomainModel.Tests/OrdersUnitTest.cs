using Farfetch.OrderBatchProcessor.Common;
using Farfetch.OrderBatchProcessor.DomainModel.Order;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.IO;
using System.Linq;

namespace Farfetch.OrderBatchProcessor.DomainModel.Tests
{
    [TestClass]
    public class OrderUnitTests
    {
        private IOrderDomainModel OrderDomainModel { get; }

        public OrderUnitTests()
        {
            var kernel = NinjectBootstrapper.Get();
            this.OrderDomainModel = kernel.Get<IOrderDomainModel>();
        }

        [TestMethod]
        public void GetComissionsAsyncTest()
        {
            var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFile}";

            var boutiquesWithOrders = this.OrderDomainModel.CalculateBoutiquesOrdersCommissions(path, 10);

            Assert.IsTrue(boutiquesWithOrders.Count > 0);

            Assert.AreEqual(2, boutiquesWithOrders.Count);

            Assert.AreEqual("B10", boutiquesWithOrders[0].BoutiqueId);

            Assert.AreEqual(30, boutiquesWithOrders[0].TotalOrdersCommission);

            Assert.AreEqual("B11", boutiquesWithOrders.Last().BoutiqueId);

            Assert.AreEqual(10, boutiquesWithOrders.Last().TotalOrdersCommission);
        }
    }
}