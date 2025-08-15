using System.ComponentModel.DataAnnotations;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Resource
{
    public class UpdateResourceDto
    {
        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения.")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Поле 'Статус' обязательно для заполнения.")]
        public required EntityStatus Status { get; set; }
    }
}
