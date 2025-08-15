using System.ComponentModel.DataAnnotations;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Client
{
    public class UpdateClientDto
    {
        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения.")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Поле 'Адрес' обязательно для заполнения.")]
        public required string Address { get; set; }
        [Required(ErrorMessage = "Поле 'Статус' обязательно для заполнения.")]
        public required EntityStatus Status { get; set; }
    }
}
