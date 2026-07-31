using System;
using System.Collections.Generic;

namespace Q1.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public int? BatchId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public double? Rating { get; set; }

    public virtual Batch? Batch { get; set; }

    public virtual Customer? Customer { get; set; }
}
