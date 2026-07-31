using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Batch
{
    public int BatchId { get; set; }

    public int? ProductId { get; set; }

    public string? WarehouseCode { get; set; }

    public string? Quarter { get; set; }

    public int? Quantity { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Product? Product { get; set; }
}
