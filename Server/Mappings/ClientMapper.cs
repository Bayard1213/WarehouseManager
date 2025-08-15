using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Client;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Mappings
{
    public static class ClientMapper
    {
        public static ClientDto ToDto(Client entity)
        {
            return new ClientDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Status = (EntityStatus)entity.Status,
                Address = entity.Address,
            };
        }

        public static Client ToEntity(ClientDto dto)
        {
            return new Client
            {
                Name = dto.Name,
                Status = (int)dto.Status,
                Address = dto.Address
            };
        }
        public static void PatchEntity(Client entity, UpdateClientDto dto)
        {
            entity.Name = dto.Name;
            entity.Status = (int)dto.Status;
            entity.Address = dto.Address;
        }
    }
}
