using WarehouseManager.Shared.Dtos.Client;
using WarehouseManager.Shared.Dtos.Measure;
using WarehouseManager.Shared.Dtos.Receipt;
using WarehouseManager.Shared.Dtos.Resource;
using WarehouseManager.Shared.Dtos.Shipment;

namespace WarehouseManager.Client.Services
{
    public class ModalFormService
    {
        public event Action? OnChange;

        public bool IsVisible { get; private set; }
        public string? Title { get; private set; }

        public object? Model { get; private set; }
        public Func<object, Task>? OnSubmitAsync { get; private set; }
        public string? OriginalName { get; private set; }
        public List<int>? DocumentResourcesIds { get; private set; }
        public List<int>? DocumentMeasuresIds { get; private set; }
        public int? DocumentClientsId { get; private set; }


        public void Open<TDto>(string title, Func<TDto, Task> onSubmitAsync, TDto? model = default)
        {
            Title = title;
            Model = model ?? Activator.CreateInstance<TDto>()!;
            OnSubmitAsync = obj => onSubmitAsync((TDto)obj);
            IsVisible = true;
            
            if (Model is UpdateClientDto clientModel)
                OriginalName = clientModel.Name;
            if (Model is UpdateMeasureDto measureModel)
                OriginalName = measureModel.Name;
            if (Model is UpdateResourceDto resourceModel)
                OriginalName = resourceModel.Name;

            if (Model is UpdateReceiptDto receiptModel) 
            {
                OriginalName = receiptModel.DocumentNumber;
                DocumentResourcesIds = receiptModel.ReceiptResources?.Select(r => r.ResourceId).ToList();
                DocumentMeasuresIds = receiptModel.ReceiptResources?.Select(r => r.MeasureId).ToList();
            }
            if (Model is UpdateShipmentDto shipmentModel) 
            {
                OriginalName = shipmentModel.DocumentNumber;
                DocumentResourcesIds = shipmentModel.ShipmentResources?.Select(r => r.ResourceId).ToList();
                DocumentMeasuresIds = shipmentModel.ShipmentResources?.Select(r => r.MeasureId).ToList();
                DocumentClientsId = shipmentModel.ClientId;
            }

            NotifyChanged();
        }

        public void Close()
        {
            IsVisible = false;
            Title = null;
            Model = null;
            OnSubmitAsync = null;
            NotifyChanged();
        }

        private void NotifyChanged() => OnChange?.Invoke();
    }


}

