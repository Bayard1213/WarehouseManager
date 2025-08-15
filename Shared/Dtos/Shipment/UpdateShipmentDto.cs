using System.ComponentModel.DataAnnotations;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Shipment
{
    public class UpdateShipmentDto
    {
        [Required(ErrorMessage = "Поле 'Номер документа' обязательно для заполнения.")]
        public string DocumentNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Поле 'Клиент' обязательно для заполнения.")]
        public int ClientId { get; set; } = 0;
        [Required(ErrorMessage = "Поле 'Дата отгрузки' обязательно для заполнения.")]
        public DateOnly DateShipment { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        [Required(ErrorMessage = "Поле 'Статус' обязательно для заполнения.")]
        public DocumentStatus Status { get; set; } = 0;
        [Required(ErrorMessage = "Поле 'Ресурсы отгрузки' обязательно для заполнения.")]
        public List<UpdateShipmentResourcesDto> ShipmentResources { get; set; } = new();
    }
}
