using System;
using System.Collections.Generic;

namespace WarehouseManager.Server.Models;

public partial class Resource
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Status { get; set; }

    public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();

    public virtual ICollection<ReceiptResource> ReceiptResources { get; set; } = new List<ReceiptResource>();

    public virtual ICollection<ShipmentResource> ShipmentResources { get; set; } = new List<ShipmentResource>();
}
