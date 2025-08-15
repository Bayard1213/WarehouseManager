using System;
using System.Collections.Generic;

namespace WarehouseManager.Server.Models;

public partial class ReceiptResource
{
    public int Id { get; set; }

    public int DocumentReceiptId { get; set; }

    public int ResourceId { get; set; }

    public int MeasureId { get; set; }

    public int Quantity { get; set; }

    public virtual DocumentsReceipt DocumentReceipt { get; set; } = null!;

    public virtual Measure Measure { get; set; } = null!;

    public virtual Resource Resource { get; set; } = null!;
}
