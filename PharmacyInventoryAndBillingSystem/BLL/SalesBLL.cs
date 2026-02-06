using System.Collections.Generic;
using PharmacyInventoryAndBillingSystem.BLL.Interfaces;
using PharmacyInventoryAndBillingSystem.DAL;
using PharmacyInventoryAndBillingSystem.DAL.Interfaces;
using PharmacyInventoryAndBillingSystem.Models;
using MedicineDAL = PharmacyInventoryAndBillingSystem.DAL.MedicineDAL;

namespace PharmacyInventoryAndBillingSystem.BLL
{
    public class SalesBLL : ISalesBLL
    {
        private readonly ISalesDAL salesDAL;
        private readonly IMedicineBLL medicineBLL;

        public SalesBLL()
        {
            this.salesDAL = new SalesDAL();
            this.medicineBLL = new MedicineBLL();
        }

        public SalesBLL(ISalesDAL salesDAL, IMedicineBLL medicineBLL)
        {
            this.salesDAL = salesDAL;
            this.medicineBLL = medicineBLL;
        }

        public string SaveOrUpdateSales(SalesMaster salesMaster)
        {
            if (salesMaster == null || salesMaster.SalesDetails == null || salesMaster.SalesDetails.Count == 0)
            {
                return "Error: No sales details provided";
            }

            // Validate stock for all items
            foreach (var detail in salesMaster.SalesDetails)
            {
                var stockValidation = medicineBLL.ValidateStock(detail.MedicineId, detail.Quantity);
                if (!stockValidation.IsValid)
                {
                    return stockValidation.Message;
                }
            }

            return salesDAL.SaveOrUpdateSales(salesMaster);
        }

        public string GetNextInvoiceNumber()
        {
            return salesDAL.GetNextInvoiceNumber();
        }

        public List<SalesMaster> GetAllInvoices()
        {
            return salesDAL.GetAllInvoices();
        }

        public SalesMaster GetInvoiceById(int salesId)
        {
            return salesDAL.GetInvoiceById(salesId);
        }

        public bool DeleteInvoice(int salesId)
        {
            return salesDAL.DeleteInvoice(salesId);
        }
    }
}
