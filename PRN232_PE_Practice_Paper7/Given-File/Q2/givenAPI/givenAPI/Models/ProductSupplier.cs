using System;

namespace givenAPI.Models
{
    public class ProductSupplier
    {
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public DateTime SupplyDate { get; set; } = DateTime.Now;
    }
}
