using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PharmacyInventoryAndBillingSystem
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Show navigation only if logged in
            if (Session["UserId"] != null)
            {
                mainNavbar.Visible = true;
            }
            else
            {
                mainNavbar.Visible = false;
                // Redirect to login if not on login page
                if (!Request.Path.ToLower().Contains("login.aspx"))
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
    }
}