using System;
using System.Collections.Generic;

namespace WarehouseManager.Server.Models;

public partial class DocumentsReceipt
{
    public int Id { get; set; }

    public string Number { get; set; } = null!;

    public DateOnly DateReceipt { get; set; }

    public virtual ICollection<ReceiptResource> ReceiptResources { get; set; } = new List<ReceiptResource>();
}
