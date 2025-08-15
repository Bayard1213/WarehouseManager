using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Measure
{
    public class MeasureDto
    {
        public  int Id { get; set; }
        public  string Name { get; set; } = string.Empty;
        public  EntityStatus Status { get; set; }
    }
}
