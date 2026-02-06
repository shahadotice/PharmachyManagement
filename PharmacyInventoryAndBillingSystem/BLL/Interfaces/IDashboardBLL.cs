using System.Collections.Generic;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.BLL.Interfaces
{
    public interface IDashboardBLL
    {
        DashboardStatistics GetDashboardStatistics();
        List<StockDetailDTO> GetStockDetails();
        List<SalesDetailModalDTO> GetSalesDetails();
    }
}
