using WarehouseManager.Server.Abstractions.Services;
using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Measure;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Mappings
{
    public static class MeasureMapper 
    {
        public static MeasureDto ToDto(Measure entity)
        {
            return new MeasureDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Status = (EntityStatus)entity.Status
            };
        }

        public static Measure ToEntity(MeasureDto dto) 
        {
            return new Measure
            {
                Name = dto.Name,
                Status = (int)dto.Status,
            };
        }
        public static void PatchEntity(Measure entity, UpdateMeasureDto dto)
        {
            entity.Name = dto.Name;
            entity.Status = (int)dto.Status;
        }
    }
}
