using WarehouseManager.Server.Models;
using WarehouseManager.Shared.Dtos.Receipt;
using WarehouseManager.Shared.Dtos.Shipment;

namespace WarehouseManager.Server.Utils
{
    public static class ResourcePatchHelper
    {
        public static void PatchResourceEntity<TResourceEntity, TResourceDto>(TResourceEntity entity,
            TResourceDto dto) where TResourceEntity : class
        {
            switch (entity)
            {
                case ReceiptResource receipt when dto is UpdateReceiptResourcesDto rDto:
                    receipt.ResourceId = rDto.ResourceId;
                    receipt.MeasureId = rDto.MeasureId;
                    receipt.Quantity = rDto.Quantity;

                    break;

                case ShipmentResource shipment when dto is UpdateShipmentResourcesDto sDto:
                    shipment.ResourceId = sDto.ResourceId;
                    shipment.MeasureId = sDto.MeasureId;
                    shipment.Quantity = sDto.Quantity;

                    break;
            }
        }
    }
}
