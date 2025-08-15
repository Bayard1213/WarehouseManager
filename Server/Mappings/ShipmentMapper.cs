using WarehouseManager.Server.Models;
using WarehouseManager.Server.Utils;
using WarehouseManager.Shared.Dtos.Receipt;
using WarehouseManager.Shared.Dtos.Shipment;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Server.Mappings
{
    public static class ShipmentMapper
    {
        public static ShipmentWithResourcesDto ShipmentDocumentWithResourceEntityToDto(DocumentsShipment documentEntity,
           IEnumerable<VShipmentDocumentResource> resourcesEntity)
        {
            return new ShipmentWithResourcesDto
            {
                DocumentId = documentEntity.Id,
                DocumentNumber = documentEntity.Number,
                Client = ClientMapper.ToDto(documentEntity.Client),
                DateShipment = documentEntity.DateShipment,
                Status = (DocumentStatus)documentEntity.Status,
                ShipmentResources = resourcesEntity.Select(ResourceViewEntityToDto).ToList()
            };
        }

        public static ShipmentResourcesDto ResourceViewEntityToDto(VShipmentDocumentResource resourceView)
        {
            return new ShipmentResourcesDto
            {
                Id = resourceView.ShipmentResourceId ?? 0,
                ResourceId = resourceView.ResourceId ?? 0,
                ResourceName = resourceView.ResourceName ?? string.Empty,
                MeasureId = resourceView.MeasureId ?? 0,
                MeasureName = resourceView.MeasureName ?? string.Empty,
                Quantity = resourceView.Quantity ?? 0
            };
        }

        public static DocumentsShipment ShipmentDtoToEntity(ShipmentWithResourcesDto dto)
        {
            return new DocumentsShipment
            {
                Id = dto.DocumentId,
                Number = dto.DocumentNumber,
                ClientId = dto.Client.Id,
                DateShipment = dto.DateShipment,
                Status = (int)dto.Status,
                ShipmentResources = dto.ShipmentResources?
                    .Select(s => ResourcesDtoToEntity(s))
                    .ToList()
                    ?? new List<ShipmentResource>()
            };
        }

        public static ShipmentResource ResourcesDtoToEntity(ShipmentResourcesDto dto, int? documentShipmentId = null)
        {
            return new ShipmentResource
            {
                Id = dto.Id,
                DocumentShipmentId = documentShipmentId ?? 0,
                ResourceId = dto.ResourceId,
                MeasureId = dto.MeasureId,
                Quantity = dto.Quantity,
            };
        }
        public static void PatchEntity(DocumentsShipment entity, UpdateShipmentDto dto)
        {
            entity.Number = dto.DocumentNumber;
            entity.ClientId = dto.ClientId;
            entity.DateShipment = dto.DateShipment;

            if (dto.ShipmentResources is not null)
            {
                foreach (var rDto in dto.ShipmentResources)
                {
                    var existing = entity.ShipmentResources
                        .FirstOrDefault(rr =>
                            rr.ResourceId == rDto.ResourceId &&
                            rr.MeasureId == rDto.MeasureId);

                    if (existing is null)
                        continue;

                    ResourcePatchHelper.PatchResourceEntity(existing, rDto);
                }

            }

        }
    }
}
