using System.ComponentModel.DataAnnotations;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Measure
{
    public class UpdateMeasureDto
    {
        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения.")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Поле 'Статус' обязательно для заполнения.")]
        public required EntityStatus Status { get; set; }
    }
}
