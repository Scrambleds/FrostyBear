using System;
using System.Collections.Generic;

namespace FrostyBear.Models;

public partial class SalesDaily
{
    public DateOnly SaleDate { get; set; }

    public decimal? TotalSale { get; set; }
}
