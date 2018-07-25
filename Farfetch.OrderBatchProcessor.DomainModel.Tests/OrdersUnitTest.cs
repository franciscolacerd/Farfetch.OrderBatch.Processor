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
    public class OrderUnitTests
    {
        private IOrderDomainModel OrderDomainModel { get; }

        public OrderUnitTests()
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

                var result = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);

                Assert.IsTrue(result.Count > 0);
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

                var result = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);

                Assert.IsTrue(result.Count > 0);
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

                await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);
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

                await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);
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

                await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);
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

                var orderLines = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);

                Assert.IsTrue(orderLines.Count > 0);

                this.OrderDomainModel.GetOrders(orderLines);
            }
            catch (NoValidOrderFormatException exception)
            {
                Assert.AreEqual("Order must be in <Boutique_ID>,<Order_ID>,<TotalOrderPrice> format at Line: 3", exception.Message);
            }
        }

        [TestMethod]
        public async Task GetOrdersAsyncTest()
        {
            var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFile}";

            var orderLines = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);

            Assert.IsTrue(orderLines.Count > 0);

            var orders = this.OrderDomainModel.GetOrders(orderLines);

            Assert.IsTrue(orders.Count > 0);
        }

        [TestMethod]
        public async Task GetCsvFileWithInvalidAmountFormatAsyncTest()
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFileWithInvalidAmountFormat}";

                var orderLines = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);

                Assert.IsTrue(orderLines.Count > 0);

                var orders = this.OrderDomainModel.GetOrders(orderLines);

                Assert.IsTrue(orders.Count > 0);

                this.OrderDomainModel.CalculateBoutiquesOrdersCommissions(orders.ToList(), 10);
            }
            catch (InvalidAmountFormatException exception)
            {
                Assert.AreEqual("Amount is in invalid format: 200.0x", exception.Message);
            }
        }

        [TestMethod]
        public async Task GetComissionsAsyncTest()
        {
            var path = $"{Directory.GetCurrentDirectory().Replace(Contants.Path.Debug, String.Empty)}{Contants.Path.CsvFile}";

            var orderLines = await this.OrderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);

            Assert.IsTrue(orderLines.Count > 0);

            var orders = this.OrderDomainModel.GetOrders(orderLines);

            Assert.IsTrue(orders.Count > 0);

            var boutiquesWithOrders = this.OrderDomainModel.CalculateBoutiquesOrdersCommissions(orders.ToList(), 10);

            Assert.IsTrue(boutiquesWithOrders.Count > 0);

            Assert.AreEqual(2, boutiquesWithOrders.Count);

            Assert.AreEqual("B10", boutiquesWithOrders[0].BoutiqueId);

            Assert.AreEqual(30, boutiquesWithOrders[0].TotalOrdersCommission);

            Assert.AreEqual("B11", boutiquesWithOrders.Last().BoutiqueId);

            Assert.AreEqual(10, boutiquesWithOrders.Last().TotalOrdersCommission);
        }
    }
}