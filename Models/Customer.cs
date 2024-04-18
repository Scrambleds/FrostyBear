using System;
using System.Collections.Generic;

namespace FrostyBear.Models;

public partial class Customer
{
    public string CustomerId { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string? CustomerAddress { get; set; }

    public string? CustomerContact { get; set; }

    public string CustomerUsername { get; set; } = null!;

    public string CustomerPassword { get; set; } = null!;

    public DateOnly? Startdate { get; set; }

    public DateOnly? Lastlogin { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
