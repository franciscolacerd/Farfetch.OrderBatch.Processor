// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderDomainModel.cs" company="">
//
// </copyright>
// <summary>
//   The order domain model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Farfetch.OrderBatchProcessor.DomainModel.Order
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Common;
    using Common.Exceptions;
    using Common.Helpers;
    using Dtos;
    using Dtos.Structs;
    using Instrumentation.Logging;

    ///https://www.c-sharpcorner.com/article/measure-your-code-using-code-metrics/
    /// <summary>
    /// The order domain model.
    /// </summary>
    public class OrderDomainModel : IOrderDomainModel
    {
        /// <summary>
        /// The _logging.
        /// </summary>
        private readonly ILoggingManager _logging;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDomainModel"/> class.
        /// </summary>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public OrderDomainModel(ILoggingManager logging) => this._logging = logging;

        /// <summary>
        /// The get order lines from document async.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="CsvNotFoundException">
        /// </exception>
        /// <exception cref="EmptyDocumentException">
        /// </exception>
        public async Task<List<string>> GetOrderLinesFromDocumentAsync(string path)
        {
            _logging.LogInformation($"Enter GetOrderLinesFromDocumentAsync with path {path}");

            ValidateIfCsvWasFound(path);

            FileInfo fileInfo = new FileInfo(path);

            ValidateIfCsvFile(fileInfo);

            string document;

            using (var reader = File.OpenText(path))
            {
                document = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            document = StringHelpers.SanitizeTextFile(document);

            ValidateIfDocumentIsEmpty(document);

            var lines = document.Split('\n').ToList();

            _logging.LogInformation($"Leaving GetOrderLinesFromDocumentAsync with parameter ", lines);

            return lines;
        }

        /// <summary>
        /// The get orders.
        /// </summary>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <returns>
        /// The <see cref="List&lt;OrderDto&gt;"/>.
        /// </returns>
        /// <exception cref="NoValidOrderFormatException">
        /// </exception>
        public List<OrderDto> GetOrders(List<string> lines)
        {
            _logging.LogInformation($"Enter GetOrders with parameter ", lines);

            var counter = 1;

            var orders = new List<OrderDto>();

            foreach (var line in lines)
            {
                var order = line.Split(',');

                ValidateIfOrderLineIsCorrect(order, counter);

                orders.Add(new OrderDto
                {
                    BoutiqueId = order[OrderFormat.BoutiqueId],
                    OrderId = order[OrderFormat.OrderId],
                    TotalOrderPrice = FormaterHelpers.ConvertFromStringToDecimal(order[OrderFormat.TotalOrderPrice])
                });

                counter++;
            }

            _logging.LogInformation($"Leaving GetOrders with parameter ", orders);

            return orders;
        }

        /// <summary>
        /// The calculate boutiques orders commissions.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <param name="commissionPercentage">
        /// The commission percentage.
        /// </param>
        /// <returns>
        /// The <see cref="List&lt;BoutiqueDto&gt;"/>.
        /// </returns>
        public List<BoutiqueDto> CalculateBoutiquesOrdersCommissions(List<OrderDto> orders, decimal commissionPercentage)
        {
            _logging.LogInformation($"Enter CalculateBoutiquesOrdersCommissions with parameter ", orders);

            var boutiques = new List<BoutiqueDto>();

            foreach (var boutiqueId in GetBoutiqueIds(orders))
            {
                var boutiqueOrders = orders.Where(x => x.BoutiqueId == boutiqueId).OrderBy(x => x.OrderId).ToList();

                var ordersToCharge = boutiqueOrders.Count > 1 ? boutiqueOrders.DropLast().ToList() : boutiqueOrders;

                boutiques.Add(new BoutiqueDto
                {
                    BoutiqueId = boutiqueId,
                    Orders = ordersToCharge.Select(
                                          order => new OrderDto
                                          {
                                              BoutiqueId = order.BoutiqueId,
                                              OrderId = order.OrderId,
                                              TotalOrderPrice = order.TotalOrderPrice,
                                              OrderCommission = order.TotalOrderPrice / commissionPercentage
                                          }).ToList(),

                    TotalOrdersCommission = ordersToCharge.Sum(x => x.TotalOrderPrice / commissionPercentage)
                });
            }

            _logging.LogInformation($"Leaving CalculateBoutiquesOrdersCommissions with parameter ", boutiques);

            return boutiques;
        }

        private static List<string> GetBoutiqueIds(List<OrderDto> orders)
        {
            return orders.OrderBy(x => x.BoutiqueId).Select(x => x.BoutiqueId).Distinct().ToList();
        }

        /// <summary>
        /// The is valid file.
        /// </summary>
        /// <param name="fileInfo">
        /// The file info.
        /// </param>
        /// <exception cref="InvalidFileFormatException">
        /// </exception>
        private static void ValidateIfCsvFile(FileInfo fileInfo)
        {
            if (fileInfo.Extension.IndexOf("csv", System.StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidFileFormatException(Contants.Exceptions.MustBeCsv);
            }
        }

        /// <summary>
        /// The is order line valid.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="counter">
        /// The counter.
        /// </param>
        /// <exception cref="NoValidOrderFormatException">
        /// </exception>
        private static void ValidateIfOrderLineIsCorrect(string[] order, int counter)
        {
            if (order.Length == 0 || order.Length != 3)
            {
                throw new NoValidOrderFormatException($"{Contants.Exceptions.NoValidOrderFormat}{counter}");
            }
        }

        /// <summary>
        /// The is csv not found.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <exception cref="CsvNotFoundException">
        /// </exception>
        private static void ValidateIfCsvWasFound(string path)
        {
            if (!File.Exists(path))
            {
                throw new CsvNotFoundException(Contants.Exceptions.FileNotFound);
            }
        }

        /// <summary>
        /// The empty document.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        private static void ValidateIfDocumentIsEmpty(string document)
        {
            if (string.IsNullOrEmpty(document))
            {
                throw new EmptyDocumentException(Contants.Exceptions.FileMustHaveLines);
            }
        }
    }
}