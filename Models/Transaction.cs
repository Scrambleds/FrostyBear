using System;
using System.Collections.Generic;

namespace FrostyBear.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? SaleId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? SaleDate { get; set; }

    public decimal? Amount { get; set; }

    public string? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Sale? Sale { get; set; }
}
