using Farfetch.OrderBatchProcessor.Common;
using Farfetch.OrderBatchProcessor.Common.Exceptions;
using Farfetch.OrderBatchProcessor.DomainModel.Order;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Farfetch.OrderBatchProcessor.DomainModel.Tests
{
    [TestClass]
    public class OrderUnitTest
    {
        private IOrderDomainModel OrderDomainModel { get; set; }

        public OrderUnitTest()
        {
            var kernel = NinjectBootstrapper.Get();
            this.OrderDomainModel = kernel.Get<IOrderDomainModel>();
        }

        [TestMethod]
        public async Task GetCsvLinesAsyncTest()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFile}";

                var result = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path);

                Assert.IsTrue(result.Any());
            }
            catch (Exception e)
            {
                Assert.Fail($"GetCsvLinesAsyncTest raised exception - {e.Message}");
            }
        }

        [TestMethod]
        public async Task GetCsvLinesWithOneLineAsyncTest()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFileWithOneLine}";

                var result = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path);

                Assert.IsTrue(result.Any());
            }
            catch (Exception e)
            {
                Assert.Fail($"GetCsvLinesAsyncTest raised exception - {e.Message}");
            }
        }

        [TestMethod]
        public async Task GetCsvFileWithNoLinesAsyncTest()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFileWithNoLines}";

                await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path);
            }
            catch (EmptyDocumentException exception)
            {
                Assert.AreEqual(exception.Message, Contants.Exceptions.FileMustHaveLines);
            }
        }

        [TestMethod]
        public async Task GetCsvInvalidFileFormatAsyncTest()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.InvalidFileFormat}";

                await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path);
            }
            catch (InvalidFileFormatException exception)
            {
                Assert.AreEqual(exception.Message, Contants.Exceptions.MustBeCsv);
            }
        }

        [TestMethod]
        public async Task GetCsvFileNotFoundAsyncTest()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory()}{Contants.Path.InvalidFileFormat}";

                await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path);
            }
            catch (CsvNotFoundException exception)
            {
                Assert.AreEqual(exception.Message, Contants.Exceptions.FileNotFound);
            }
        }

        [TestMethod]
        public async Task GetOrdersWithInvalidLineAsyncTest()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFileWithInvalidLine}";

                var orderLines = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path);

                Assert.IsTrue(orderLines.Any());

                this.OrderDomainModel.GetOrders(orderLines);
            }
            catch (NoValidOrderFormatException exception)
            {
                Assert.AreEqual(exception.Message, "Order must be in <Boutique_ID>,<Order_ID>,<TotalOrderPrice> format at Line: 3");
            }
        }

        [TestMethod]
        public async Task GetOrdersAsyncTest()
        {
            var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFile}";

            var orderLines = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path);

            Assert.IsTrue(orderLines.Any());

            var orders = this.OrderDomainModel.GetOrders(orderLines);

            Assert.IsTrue(orders.Any());
        }

        [TestMethod]
        public async Task GetCsvFileWithInvalidAmountFormatAsyncTest()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFileWithInvalidAmountFormat}";

                var orderLines = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path);

                Assert.IsTrue(orderLines.Any());

                var orders = this.OrderDomainModel.GetOrders(orderLines);

                Assert.IsTrue(orders.Any());

                this.OrderDomainModel.CalculateBoutiquesOrdersCommissions(orders.ToList(), 10);
            }
            catch (InvalidAmountFormatException exception)
            {
                Assert.AreEqual(exception.Message, "Amount is in invalid format: 200.0x");
            }
        }

        [TestMethod]
        public async Task GetComissionsAsyncTest()
        {
            var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFile}";

            var orderLines = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path);

            Assert.IsTrue(orderLines.Any());

            var orders = this.OrderDomainModel.GetOrders(orderLines);

            Assert.IsTrue(orders.Any());

            var boutiquesWithOrders = this.OrderDomainModel.CalculateBoutiquesOrdersCommissions(orders.ToList(), 10);

            Assert.IsTrue(boutiquesWithOrders.Any());

            Assert.AreEqual(boutiquesWithOrders.Count(), 2);

            Assert.AreEqual(boutiquesWithOrders.First().BoutiqueId, "B10");

            Assert.AreEqual(boutiquesWithOrders.First().TotalOrdersCommission, 30);

            Assert.AreEqual(boutiquesWithOrders.Last().BoutiqueId, "B11");

            Assert.AreEqual(boutiquesWithOrders.Last().TotalOrdersCommission, 10);
        }
    }
}