using System.Collections.Generic;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.DAL.Interfaces
{
    public interface ISalesDAL
    {
        string SaveOrUpdateSales(SalesMaster salesMaster);
        string GetNextInvoiceNumber();
        List<SalesMaster> GetAllInvoices();
        SalesMaster GetInvoiceById(int salesId);
        bool DeleteInvoice(int salesId);
    }
}
