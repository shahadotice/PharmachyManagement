using System.Collections.Generic;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.DAL.Interfaces
{
    public interface IDashboardDAL
    {
        DashboardStatistics GetDashboardStatistics();
        List<StockDetailDTO> GetStockDetails();
        List<SalesDetailModalDTO> GetSalesDetails();
    }
}
