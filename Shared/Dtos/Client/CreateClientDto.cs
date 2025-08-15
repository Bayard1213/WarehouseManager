using System.ComponentModel.DataAnnotations;
using WarehouseManager.Shared.Enums;

namespace WarehouseManager.Shared.Dtos.Client
{
    public class CreateClientDto
    {
        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Адрес' обязательно для заполнения.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Статус' обязательно для заполнения.")]
        public EntityStatus Status { get; set; } = EntityStatus.Active;
    }
}
