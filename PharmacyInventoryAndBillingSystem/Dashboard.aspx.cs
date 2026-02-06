using System;
using System.Web.Script.Serialization;
using PharmacyInventoryAndBillingSystem.BLL;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private DashboardBLL dashboardBLL;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            dashboardBLL = new DashboardBLL();

            if (!IsPostBack)
            {
                lblCurrentDate.Text = DateTime.Now.ToString("dd MMM yyyy");
                LoadDashboard();
                LoadModalData();
            }
        }

        private void LoadDashboard()
        {
            try
            {
                DashboardStatistics stats = dashboardBLL.GetDashboardStatistics();

              
                lblTodaySalesCount.Text = stats.TotalStokesAmount.ToString("N2");
                lblTodayRevenue.Text = stats.TotalSalesAmount.ToString("N2");

             
                
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine("Error loading dashboard statistics: " + ex.Message);
            }
        }

        private void LoadModalData()
        {
            try
            {
                // Load Stock Details
                var stockDetails = dashboardBLL.GetStockDetails();
                var stockJson = new JavaScriptSerializer().Serialize(stockDetails);
                string stockScript = "var stockDetails = " + stockJson + ";";
                ClientScript.RegisterStartupScript(this.GetType(), "LoadStockDetails", stockScript, true);

                // Load Sales Details
                var salesDetails = dashboardBLL.GetSalesDetails();
                var salesJson = new JavaScriptSerializer().Serialize(salesDetails);
                string salesScript = "var salesDetails = " + salesJson + ";";
                ClientScript.RegisterStartupScript(this.GetType(), "LoadSalesDetails", salesScript, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading modal data: " + ex.Message);
            }
        }
    }
}
