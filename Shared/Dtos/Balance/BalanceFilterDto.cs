namespace WarehouseManager.Shared.Dtos.Balance
{
    public class BalanceFilterDto
    {
        public List<int>? ResourceIds { get; set; }
        public List<int>? MeasureIds { get; set; }
        public bool OnlyActive { get; set; } = false;
    }
}
