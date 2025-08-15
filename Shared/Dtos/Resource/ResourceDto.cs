using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Resource
{
    public class ResourceDto
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public EntityStatus Status { get; set; }
    }
}
