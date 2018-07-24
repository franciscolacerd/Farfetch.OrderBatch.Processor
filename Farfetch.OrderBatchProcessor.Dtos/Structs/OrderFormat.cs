namespace Farfetch.OrderBatchProcessor.Dtos.Structs
{
    //<Boutique_ID>,<Order_ID>,<TotalOrderPrice>
    public struct OrderFormat
    {
        public static int BoutiqueId = 0;

        public static int OrderId = 1;

        public static int TotalOrderPrice = 2;
    }
}