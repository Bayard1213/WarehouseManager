using System.ComponentModel.DataAnnotations;

namespace WarehouseManager.Shared.Dtos.Receipt
{
    public class CreateReceiptDto
    {
        [Required(ErrorMessage = "Поле 'Номер документа' обязательно для заполнения.")]
        public string DocumentNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Поле 'Дата получения' обязательно для заполнения.")]
        public DateOnly DateReceipt { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public List<CreateReceiptResourcesDto>? ReceiptResources { get; set; }
    }
}
