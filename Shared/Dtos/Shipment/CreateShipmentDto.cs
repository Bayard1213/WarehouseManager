using System.ComponentModel.DataAnnotations;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Shipment
{
    public class CreateShipmentDto
    {
        [Required(ErrorMessage = "Поле 'Номер документа' обязательно для заполнения.")]
        public string DocumentNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Поле 'Клиент' обязательно для заполнения.")]
        public int ClientId { get; set; } = 0;
        [Required(ErrorMessage = "Поле 'Дата отгрузки' обязательно для заполнения.")]
        public DateOnly DateShipment {  get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
        [Required(ErrorMessage = "Поле 'Ресурсы отгрузки' обязательно для заполнения.")]
        public List<CreateShipmentResourcesDto> ShipmentResources { get; set; } = new List<CreateShipmentResourcesDto>();
    }
}
