namespace WarehouseManager.Shared.Dtos.Shipment
{
    public class ShipmentResourcesDto
    {
        public required int Id { get; set; }
        public required int ResourceId { get; set; }
        public required string ResourceName { get; set; }
        public required int MeasureId { get; set; }
        public required string MeasureName { get; set; }
        public required int Quantity { get; set; }
    }
}
