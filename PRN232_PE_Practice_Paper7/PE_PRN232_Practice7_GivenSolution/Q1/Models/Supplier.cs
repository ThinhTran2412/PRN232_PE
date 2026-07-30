using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? Specialty { get; set; }

    public DateOnly? ContractDate { get; set; }

    public virtual ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
}
