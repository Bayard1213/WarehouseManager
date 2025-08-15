using System;
using System.Collections.Generic;

namespace WarehouseManager.Server.Models;

public partial class VBalance
{
    public int? ResourceId { get; set; }

    public string? ResourceName { get; set; }

    public int? ResourceStatus { get; set; }

    public int? MeasureId { get; set; }

    public string? MeasureName { get; set; }

    public int? MeasureStatus { get; set; }

    public decimal? BalanceQuantity { get; set; }
}
