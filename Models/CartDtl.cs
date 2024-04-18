using System;
using System.Collections.Generic;

namespace FrostyBear.Models;

public partial class CartDtl
{
    public string CartId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public double? CdtlQty { get; set; }

    public double? CdtlPrice { get; set; }

    public double? CdtlMoney { get; set; }
}
