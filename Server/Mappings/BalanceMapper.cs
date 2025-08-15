using System.Reflection.Metadata.Ecma335;
using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Balance;

namespace WarehouseManager.Server.Mappings
{
    public class BalanceMapper
    {
        public static VBalanceDto ViewToDto(VBalance entity)
        {
            return new VBalanceDto
            {
                ResourceId = entity.ResourceId,
                ResourceName = entity.ResourceName,
                ResourceStatus = entity.ResourceStatus,
                MeasureId = entity.MeasureId,
                MeasureName = entity.MeasureName,
                MeasureStatus = entity.MeasureStatus,
                BalanceQuantity = entity.BalanceQuantity
            };
        }

    }
}
