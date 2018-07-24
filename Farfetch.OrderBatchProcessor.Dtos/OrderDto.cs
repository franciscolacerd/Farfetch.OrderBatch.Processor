namespace Farfetch.OrderBatchProcessor.Dtos
{
    //<Boutique_ID>,<Order_ID>,<TotalOrderPrice>
    public class OrderDto
    {
        public string BoutiqueId { get; set; }

        public string OrderId { get; set; }

        public decimal TotalOrderPrice { get; set; }

        public decimal OrderCommission { get; set; }

        //   public bool WillNotPayCommission { get; set; }
    }
}