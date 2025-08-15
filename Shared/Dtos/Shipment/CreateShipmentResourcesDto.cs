using System.ComponentModel.DataAnnotations;

namespace WarehouseManager.Shared.Dtos.Shipment
{
    public class CreateShipmentResourcesDto
    {
        [Required(ErrorMessage = "Поле 'Ресурс' обязательно для заполнения.")]
        public int ResourceId { get; set; } = 0;
        [Required(ErrorMessage = "Поле 'Единица измерения' обязательно для заполнения.")]
        public int MeasureId { get; set; } = 0;
        [Required(ErrorMessage = "Поле 'Количество' обязательно для заполнения.")]
        public int Quantity { get; set; } = 0;
    }
}
