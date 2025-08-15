using System.ComponentModel.DataAnnotations;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Measure
{
    public class CreateMeasureDto
    {
        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Статус' обязательно для заполнения.")]
        public EntityStatus Status { get; set; } = EntityStatus.Active;
    }
}
