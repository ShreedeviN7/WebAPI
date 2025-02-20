namespace WebAPITemplate.DatabaseEntities
{
    public class SaleRecord
    {
        public DateTime Date { get; set; }
        public string SKU { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
