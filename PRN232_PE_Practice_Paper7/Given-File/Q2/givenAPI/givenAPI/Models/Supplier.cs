using System;

namespace givenAPI.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public DateTime ContractDate { get; set; }
    }
}
