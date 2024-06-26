﻿using System;
using System.Collections.Generic;

namespace FrostyBear.Models;

public partial class Cart
{
    public string CartId { get; set; } = null!;

    public string? CustomerId { get; set; }

    public DateOnly? CartDate { get; set; }

    public double? CartMoney { get; set; }

    public double? CartQty { get; set; }

    public string? CartCf { get; set; }

    public string? CartPay { get; set; }

    public string? CartSend { get; set; }

    public string? CartVoid { get; set; }
}
