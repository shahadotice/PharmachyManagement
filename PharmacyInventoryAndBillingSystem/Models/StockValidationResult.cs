namespace PharmacyInventoryAndBillingSystem.Models
{
    public class StockValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
