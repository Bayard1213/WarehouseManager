using System;
using System.Collections.Generic;

namespace WarehouseManager.Server.Models;

public partial class ShipmentResource
{
    public int Id { get; set; }

    public int DocumentShipmentId { get; set; }

    public int ResourceId { get; set; }

    public int MeasureId { get; set; }

    public int Quantity { get; set; }

    public virtual DocumentsShipment DocumentShipment { get; set; } = null!;

    public virtual Measure Measure { get; set; } = null!;

    public virtual Resource Resource { get; set; } = null!;
}
