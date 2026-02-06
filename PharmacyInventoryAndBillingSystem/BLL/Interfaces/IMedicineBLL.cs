using System.Collections.Generic;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.BLL.Interfaces
{
    public interface IMedicineBLL
    {
        List<Medicine> GetAllMedicines();
        Medicine GetMedicineById(int medicineId);
        int InsertMedicine(Medicine medicine);
        bool UpdateMedicine(Medicine medicine);
        bool DeleteMedicine(int medicineId);
        StockValidationResult ValidateStock(int medicineId, int requestedQuantity);
    }
}
