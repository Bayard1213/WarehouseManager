using WarehouseManager.Server.Models;
using WarehouseManager.Server.Utils;
using WarehouseManager.Shared.Dtos.Receipt;

namespace WarehouseManager.Server.Mappings
{
    public static class ReceiptMapper
    {
        public static ReceiptWithResourcesDto ReceiptDocumentWithResourceEntityToDto(DocumentsReceipt documentEntity,
            IEnumerable<VReceiptDocumentResource> resourcesEntity)
        {
            return new ReceiptWithResourcesDto
            {
                DocumentId = documentEntity.Id,
                DocumentNumber = documentEntity.Number,
                DateReceipt = documentEntity.DateReceipt,
                ReceiptResources = resourcesEntity.Select(ResourceViewEntityToDto).ToList()
            };
        }

        public static ReceiptResourcesDto ResourceViewEntityToDto(VReceiptDocumentResource resourceView)
        {
            return new ReceiptResourcesDto
            {
                Id = resourceView.ReceiptResourceId ?? 0,
                ResourceId = resourceView.ResourceId ?? 0,
                ResourceName = resourceView.ResourceName ?? string.Empty,
                MeasureId = resourceView.MeasureId ?? 0,
                MeasureName = resourceView.MeasureName ?? string.Empty,
                Quantity = resourceView.Quantity ?? 0
            };
        }

        public static DocumentsReceipt ReceiptDtoToEntity(ReceiptWithResourcesDto dto)
        {
            return new DocumentsReceipt
            {
                Id = dto.DocumentId,
                Number = dto.DocumentNumber,
                DateReceipt = dto.DateReceipt,
                ReceiptResources = dto.ReceiptResources?
                    .Select(r => ResourcesDtoToEntity(r))
                    .ToList()
                    ?? new List<ReceiptResource>()
            };
        }

        public static ReceiptResource ResourcesDtoToEntity(ReceiptResourcesDto dto, int? documentReceiptId = null)
        {
            return new ReceiptResource
            {
                Id = dto.Id,
                ResourceId = dto.ResourceId,
                MeasureId = dto.MeasureId,
                Quantity = dto.Quantity,
                DocumentReceiptId = documentReceiptId ?? 0
            };
        }

        public static void PatchEntity(DocumentsReceipt entity, UpdateReceiptDto dto)
        {
                entity.Number = dto.DocumentNumber;
                entity.DateReceipt = dto.DateReceipt;

            if (dto.ReceiptResources is not null)
            {
                foreach (var rDto in dto.ReceiptResources)
                {
                    var existing = entity.ReceiptResources
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