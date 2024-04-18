using System;
using System.Collections.Generic;

namespace FrostyBear.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public string ProductId { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public int EmployeeId { get; set; }

    public int? Quantity { get; set; }

    public DateTime? SaleDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
