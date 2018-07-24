using System.Collections.Generic;
using System.Threading.Tasks;
using Farfetch.OrderBatchProcessor.Dtos;

namespace Farfetch.OrderBatchProcessor.DomainModel.Order
{
    public interface IOrderDomainModel
    {
        Task<IEnumerable<string>> GetOrderLinesFromDocumentAsync(string path);

        IEnumerable<OrderDto> GetOrders(IEnumerable<string> lines);

        IEnumerable<BoutiqueDto> CalculateBoutiquesOrdersCommissions(IEnumerable<OrderDto> orders, decimal commissionPercentage);
    }
}