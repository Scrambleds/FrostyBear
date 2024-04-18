using System;
using System.Collections.Generic;

namespace FrostyBear.Models;

public partial class Employee
{
    public string EmployeeId { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string? EmployeeContact { get; set; }

    public string EmployeeUsername { get; set; } = null!;

    public string EmployeePassword { get; set; } = null!;

    public string? PositionId { get; set; }

    public virtual Position? Position { get; set; }
}
