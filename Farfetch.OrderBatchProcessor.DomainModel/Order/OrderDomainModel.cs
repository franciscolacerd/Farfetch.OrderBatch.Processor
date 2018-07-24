using Farfetch.OrderBatchProcessor.Common;
using Farfetch.OrderBatchProcessor.Common.Exceptions;
using Farfetch.OrderBatchProcessor.Common.Helpers;
using Farfetch.OrderBatchProcessor.Dtos;
using Farfetch.OrderBatchProcessor.Dtos.Structs;
using Farfetch.OrderBatchProcessor.Instrumentation.Logging;
using Ninject.Infrastructure.Language;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Farfetch.OrderBatchProcessor.DomainModel.Order
{
    public class OrderDomainModel : IOrderDomainModel
    {
        private readonly ILoggingManager _logging;

        public OrderDomainModel(ILoggingManager logging)
        {
            this._logging = logging;
        }

        public async Task<IEnumerable<string>> GetOrderLinesFromDocumentAsync(string path)
        {
            _logging.LogInformation($"Enter GetOrderLinesFromDocumentAsync with path {path}");

            if (!File.Exists(path))
            {
                throw new CsvNotFoundException(Contants.Exceptions.FileNotFound);
            }

            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Extension.ToLower().Contains("csv"))
            {
                throw new InvalidFileFormatException(Contants.Exceptions.MustBeCsv);
            }

            string document;

            using (var reader = File.OpenText(path))
            {
                document = await reader.ReadToEndAsync();
            }

            document = StringHelpers.SanitizeTextFile(document);

            if (string.IsNullOrEmpty(document))
            {
                throw new EmptyDocumentException(Contants.Exceptions.FileMustHaveLines);
            }

            var lines = document.Split('\n').ToEnumerable();

            _logging.LogInformation($"Leaving GetOrderLinesFromDocumentAsync with parameter ", lines);

            return lines;
        }

        public IEnumerable<OrderDto> GetOrders(IEnumerable<string> lines)
        {
            _logging.LogInformation($"Enter GetOrders with parameter ", lines);

            var counter = 1;

            var orders = new List<OrderDto>();

            foreach (var line in lines)
            {
                var order = line.Split(',');

                if (!order.Any() || order.Count() != 3)
                {
                    throw new NoValidOrderFormatException($"{Contants.Exceptions.NoValidOrderFormat}{counter}");
                }

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

        public IEnumerable<BoutiqueDto> CalculateBoutiquesOrdersCommissions(IEnumerable<OrderDto> orders, decimal commissionPercentage)
        {
            _logging.LogInformation($"Enter CalculateBoutiquesOrdersCommissions with parameter ", orders);

            var boutiques = new List<BoutiqueDto>();

            var boutiqueIds = orders.OrderBy(x => x.BoutiqueId).Select(x => x.BoutiqueId).Distinct().ToList();

            foreach (var boutiqueId in boutiqueIds)
            {
                var boutiqueOrders = orders.Where(x => x.BoutiqueId == boutiqueId).OrderBy(x => x.OrderId);
                //The order with the highest value of the day will not be subject to commission - Not appliable if only one order.
                IEnumerable<OrderDto> ordersToCharge = boutiqueOrders.Count() > 1 ? boutiqueOrders.DropLast() : boutiqueOrders;

                boutiques.Add(new BoutiqueDto
                {
                    BoutiqueId = boutiqueId,
                    Orders = ordersToCharge.Select((order) => new OrderDto
                    {
                        BoutiqueId = order.BoutiqueId,
                        OrderId = order.OrderId,
                        TotalOrderPrice = order.TotalOrderPrice,
                        //* Boutiques should be charged by 10% of the total value every order - Single Order Commission
                        OrderCommission = order.TotalOrderPrice / commissionPercentage
                    }).ToList(),
                    //* Boutiques should be charged by 10% of the total value every order - Total Orders Commission
                    TotalOrdersCommission = ordersToCharge.Sum(x => x.TotalOrderPrice / commissionPercentage)
                });
            }

            _logging.LogInformation($"Leaving CalculateBoutiquesOrdersCommissions with parameter ", boutiques);

            return boutiques;
        }
    }
}