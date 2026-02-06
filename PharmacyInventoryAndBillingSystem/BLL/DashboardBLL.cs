using System.Collections.Generic;
using PharmacyInventoryAndBillingSystem.BLL.Interfaces;
using PharmacyInventoryAndBillingSystem.DAL;
using PharmacyInventoryAndBillingSystem.DAL.Interfaces;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.BLL
{
    public class DashboardBLL : IDashboardBLL
    {
        private readonly IDashboardDAL dashboardDAL;

        public DashboardBLL()
        {
            this.dashboardDAL = new DashboardDAL();
        }

        public DashboardBLL(IDashboardDAL dashboardDAL)
        {
            this.dashboardDAL = dashboardDAL;
        }

        public DashboardStatistics GetDashboardStatistics()
        {
            return dashboardDAL.GetDashboardStatistics();
        }

        public List<StockDetailDTO> GetStockDetails()
        {
            return dashboardDAL.GetStockDetails();
        }

        public List<SalesDetailModalDTO> GetSalesDetails()
        {
            return dashboardDAL.GetSalesDetails();
        }
    }
}
