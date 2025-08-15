using System;
using System.Collections.Generic;

namespace WarehouseManager.Server.Models;

public partial class DocumentsShipment
{
    public int Id { get; set; }

    public string Number { get; set; } = null!;

    public int ClientId { get; set; }

    public DateOnly DateShipment { get; set; }

    public int Status { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<ShipmentResource> ShipmentResources { get; set; } = new List<ShipmentResource>();
}
