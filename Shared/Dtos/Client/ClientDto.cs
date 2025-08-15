using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Client
{
    public class ClientDto
    {
        public  int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public  EntityStatus Status { get; set; }
    }
}
