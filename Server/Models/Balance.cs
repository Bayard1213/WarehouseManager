using System;
using System.Collections.Generic;

namespace WarehouseManager.Server.Models;

public partial class Balance
{
    public int Id { get; set; }

    public int ResourceId { get; set; }

    public int MeasureId { get; set; }

    public int Quantity { get; set; }

    public virtual Measure Measure { get; set; } = null!;

    public virtual Resource Resource { get; set; } = null!;
}
