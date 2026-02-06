using System.Collections.Generic;
using PharmacyInventoryAndBillingSystem.DAL;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.BLL.Interfaces
{
    public interface ISalesBLL
    {
        string SaveOrUpdateSales(SalesMaster salesMaster);
        string GetNextInvoiceNumber();
        List<SalesMaster> GetAllInvoices();
        SalesMaster GetInvoiceById(int salesId);
        bool DeleteInvoice(int salesId);
    }
}
