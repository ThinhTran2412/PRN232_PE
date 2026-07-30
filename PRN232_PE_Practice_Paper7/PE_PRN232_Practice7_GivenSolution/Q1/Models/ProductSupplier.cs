using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class ProductSupplier
{
    public int ProductId { get; set; }

    public int SupplierId { get; set; }

    public DateOnly? SupplyDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
