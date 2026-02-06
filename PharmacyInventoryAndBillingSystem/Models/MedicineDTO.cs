namespace PharmacyInventoryAndBillingSystem.Models
{
    public class MedicineDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string BatchNo { get; set; }
        public int Quantity { get; set; }
        public string ExpiryDate { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SellsPrice { get; set; }
    }
}
