using System;
using System.Collections.Generic;

namespace WarehouseManager.Server.Models;

public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int Status { get; set; }

    public virtual ICollection<DocumentsShipment> DocumentsShipments { get; set; } = new List<DocumentsShipment>();
}
