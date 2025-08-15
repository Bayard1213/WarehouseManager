namespace WarehouseManager.Shared.Dtos.Receipt
{
    public class ReceiptWithResourcesDto
    {
        public required int DocumentId { get; set; }
        public required string DocumentNumber { get; set; }
        public required DateOnly DateReceipt { get; set; }
        
        public List<ReceiptResourcesDto>? ReceiptResources { get; set; }

    }
}
