using System.Collections.Generic;

namespace Farfetch.OrderBatchProcessor.Dtos
{
    public class BoutiqueDto
    {
        public string BoutiqueId { get; set; }

        public decimal TotalOrdersCommission { get; set; }

        public IEnumerable<OrderDto> Orders { get; set; }
    }
}