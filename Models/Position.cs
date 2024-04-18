using System;
using System.Collections.Generic;

namespace FrostyBear.Models;

public partial class Position
{
    public string PositionId { get; set; } = null!;

    public string PositionName { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
