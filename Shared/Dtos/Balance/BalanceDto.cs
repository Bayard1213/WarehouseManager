namespace WarehouseManager.Shared.Dtos.Balance
{
    public class BalanceDto
    {
        public required int ResourceId { get; set; }
        public required string ResourceName { get; set; }
        public required int MeasureId { get; set; }
        public required string MeasureName { get; set; }
        public required int BalanceQuantity { get; set; }
    }
}
