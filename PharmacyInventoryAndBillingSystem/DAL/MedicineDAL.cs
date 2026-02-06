using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using PharmacyInventoryAndBillingSystem.DAL.Interfaces;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.DAL
{
    public class MedicineDAL : IMedicineDAL
    {
        public List<Medicine> GetAllMedicines()
        {
            List<Medicine> medicines = new List<Medicine>();
            string query = "SELECT MedicineId, MedicineName, BatchNo, ExpiryDate, Quantity, UnitPrice, SellsPrice, Description, CreatedDate, ModifiedDate FROM Medicines ORDER BY MedicineName";
            
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            
            foreach (DataRow row in dt.Rows)
            {
                medicines.Add(new Medicine
                {
                    MedicineId = Convert.ToInt32(row["MedicineId"]),
                    MedicineName = row["MedicineName"].ToString(),
                    BatchNo = row["BatchNo"].ToString(),
                    ExpiryDate = Convert.ToDateTime(row["ExpiryDate"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                    SellsPrice = Convert.ToDecimal(row["SellsPrice"]),
                    Description = row["Description"]?.ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedDate = row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]) : (DateTime?)null
                });
            }
            
            return medicines;
        }

        public Medicine GetMedicineById(int medicineId)
        {
            Medicine medicine = null;
            string query = "SELECT MedicineId, MedicineName, BatchNo, ExpiryDate, Quantity, UnitPrice, SellsPrice, Description, CreatedDate, ModifiedDate FROM Medicines WHERE MedicineId = @MedicineId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MedicineId", medicineId)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                medicine = new Medicine
                {
                    MedicineId = Convert.ToInt32(row["MedicineId"]),
                    MedicineName = row["MedicineName"].ToString(),
                    BatchNo = row["BatchNo"].ToString(),
                    ExpiryDate = Convert.ToDateTime(row["ExpiryDate"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                    SellsPrice = Convert.ToDecimal(row["SellsPrice"]),
                    Description = row["Description"]?.ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedDate = row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]) : (DateTime?)null
                };
            }
            
            return medicine;
        }

        public int InsertMedicine(Medicine medicine)
        {
            string query = @"INSERT INTO Medicines (MedicineName, BatchNo, ExpiryDate, Quantity, UnitPrice, SellsPrice, Description, CreatedDate) 
                           VALUES (@MedicineName, @BatchNo, @ExpiryDate, @Quantity, @UnitPrice, @SellsPrice, @Description, @CreatedDate);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MedicineName", medicine.MedicineName),
                new SqlParameter("@BatchNo", medicine.BatchNo),
                new SqlParameter("@ExpiryDate", medicine.ExpiryDate),
                new SqlParameter("@Quantity", medicine.Quantity),
                new SqlParameter("@UnitPrice", medicine.UnitPrice),
                new SqlParameter("@SellsPrice", medicine.SellsPrice),
                new SqlParameter("@Description", (object)medicine.Description ?? DBNull.Value),
                new SqlParameter("@CreatedDate", DateTime.Now)
            };

            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
        }

        public bool UpdateMedicine(Medicine medicine)
        {
            string query = @"UPDATE Medicines SET MedicineName = @MedicineName, BatchNo = @BatchNo, ExpiryDate = @ExpiryDate, 
                           Quantity = @Quantity, UnitPrice = @UnitPrice, SellsPrice = @SellsPrice, Description = @Description, ModifiedDate = @ModifiedDate 
                           WHERE MedicineId = @MedicineId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MedicineId", medicine.MedicineId),
                new SqlParameter("@MedicineName", medicine.MedicineName),
                new SqlParameter("@BatchNo", medicine.BatchNo),
                new SqlParameter("@ExpiryDate", medicine.ExpiryDate),
                new SqlParameter("@Quantity", medicine.Quantity),
                new SqlParameter("@UnitPrice", medicine.UnitPrice),
                new SqlParameter("@SellsPrice", medicine.SellsPrice),
                new SqlParameter("@Description", (object)medicine.Description ?? DBNull.Value),
                new SqlParameter("@ModifiedDate", DateTime.Now)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteMedicine(int medicineId)
        {
            string query = "DELETE FROM Medicines WHERE MedicineId = @MedicineId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@MedicineId", medicineId)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public StockValidationResult ValidateStock(int medicineId, int requestedQuantity)
        {
            StockValidationResult result = new StockValidationResult();
            
            try
            {
                string query = "SELECT Quantity FROM Medicines WHERE MedicineId = @MedicineId";
                
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MedicineId", medicineId)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    object quantityObj = dt.Rows[0]["Quantity"];
                    if (quantityObj != null && quantityObj != DBNull.Value)
                    {
                        int availableQuantity = Convert.ToInt32(quantityObj);
                        result.AvailableQuantity = availableQuantity;
                        
                        if (availableQuantity >= requestedQuantity)
                        {
                            result.IsValid = true;
                            result.Message = "Stock available";
                        }
                        else
                        {
                            result.IsValid = false;
                            result.Message = $"Insufficient stock. Available: {availableQuantity}, Requested: {requestedQuantity}";
                        }
                    }
                    else
                    {
                        result.IsValid = false;
                        result.Message = "Medicine stock quantity is not available";
                        result.AvailableQuantity = 0;
                    }
                }
                else
                {
                    result.IsValid = false;
                    result.Message = "Medicine not found";
                    result.AvailableQuantity = 0;
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Message = "Error checking stock: " + ex.Message;
                result.AvailableQuantity = 0;
            }
            
            return result;
        }
    }
}
