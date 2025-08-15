using System.ComponentModel.DataAnnotations;

namespace WarehouseManager.Shared.Dtos.Receipt
{
    public class UpdateReceiptResourcesDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле 'Ресурс' обязательно для заполнения.")]
        public int ResourceId { get; set; }
        [Required(ErrorMessage = "Поле 'Единица измерения' обязательно для заполнения.")]
        public int MeasureId { get; set; }
        [Required(ErrorMessage = "Поле 'Количество' обязательно для заполнения.")]
        public int Quantity { get; set; }
    }
}
