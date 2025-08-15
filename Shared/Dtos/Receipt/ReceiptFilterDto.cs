namespace WarehouseManager.Shared.Dtos.Receipt
{
    public class ReceiptFilterDto
    {
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }

        public List<string>? DocumentNumbers { get; set; }

        public List<int>? ResourceIds { get; set; }

        public List<int>? MeasureIds { get; set; }
    }
}
