using WarehouseManager.Shared.Dtos.Client;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Shipment
{
    public class ShipmentWithResourcesDto
    {
        public required int DocumentId { get; set; }
        public required string DocumentNumber { get; set; }
        public required ClientDto Client { get; set; }
        public required DateOnly DateShipment { get; set; }
        public required DocumentStatus Status { get; set; }
        public required List<ShipmentResourcesDto> ShipmentResources { get; set; }
    }
}
