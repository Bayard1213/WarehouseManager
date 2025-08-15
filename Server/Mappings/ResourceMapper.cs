using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Resource;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Mappings
{
    public class ResourceMapper
    {
        public static ResourceDto ToDto(Resource entity)
        {
            return new ResourceDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Status =(EntityStatus)entity.Status
            };
        }

        public static Resource ToEntity(ResourceDto dto)
        {
            return new Resource
            {
                Name = dto.Name,
                Status = (int)dto.Status,
            };
        }
        public static void PatchEntity(Resource entity, UpdateResourceDto dto)
        {
            entity.Name = dto.Name;
            entity.Status = (int)dto.Status;
        }
    }
}
