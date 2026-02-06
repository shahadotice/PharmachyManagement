using System.Collections.Generic;
using PharmacyInventoryAndBillingSystem.BLL.Interfaces;
using PharmacyInventoryAndBillingSystem.DAL;
using PharmacyInventoryAndBillingSystem.DAL.Interfaces;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.BLL
{
    public class MedicineBLL : IMedicineBLL
    {
        private readonly IMedicineDAL medicineDAL;

        public MedicineBLL()
        {
            this.medicineDAL = new DAL.MedicineDAL();
        }

        public MedicineBLL(IMedicineDAL medicineDAL)
        {
            this.medicineDAL = medicineDAL;
        }

        public List<Medicine> GetAllMedicines()
        {
            return medicineDAL.GetAllMedicines();
        }

        public Medicine GetMedicineById(int medicineId)
        {
            return medicineDAL.GetMedicineById(medicineId);
        }

        public int InsertMedicine(Medicine medicine)
        {
            if (medicine == null || string.IsNullOrWhiteSpace(medicine.MedicineName))
            {
                return 0;
            }

            return medicineDAL.InsertMedicine(medicine);
        }

        public bool UpdateMedicine(Medicine medicine)
        {
            if (medicine == null || medicine.MedicineId <= 0 || string.IsNullOrWhiteSpace(medicine.MedicineName))
            {
                return false;
            }

            return medicineDAL.UpdateMedicine(medicine);
        }

        public bool DeleteMedicine(int medicineId)
        {
            if (medicineId <= 0)
            {
                return false;
            }

            return medicineDAL.DeleteMedicine(medicineId);
        }

        public StockValidationResult ValidateStock(int medicineId, int requestedQuantity)
        {
            if (medicineId <= 0 || requestedQuantity <= 0)
            {
                return new StockValidationResult
                {
                    IsValid = false,
                    Message = "Invalid medicine ID or quantity"
                };
            }

            return medicineDAL.ValidateStock(medicineId, requestedQuantity);
        }
    }
}
