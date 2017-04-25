namespace CommerceManagerCreditEnhancements.DTO
{
    public class CreditItem
    {
        public long LineItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public decimal Discount { get; set; }
        public decimal OrderLevelDiscount { get; set; }
        public decimal ExtraDiscount { get; set; }
        public decimal Quantity { get; set; }
        public decimal PlacedPrice { get; set; }
    }
}
