using System;
using System.Collections.Generic;

namespace FrostyBear.Models;

public partial class Delivery
{
    public int DeliveryId { get; set; }

    public int? SaleId { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public virtual Sale? Sale { get; set; }
}
