using System;
using System.Collections.Generic;

namespace WarehouseManager.Server.Models;

public partial class VReceiptDocumentResource
{
    public int? ReceiptResourceId { get; set; }

    public int? DocumentReceiptId { get; set; }

    public int? ResourceId { get; set; }

    public string? ResourceName { get; set; }

    public int? MeasureId { get; set; }

    public string? MeasureName { get; set; }

    public int? Quantity { get; set; }
}
