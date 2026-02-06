using System;
using PharmacyInventoryAndBillingSystem.BLL;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
       
            if (Request.QueryString["logout"] == "true" || !IsPostBack)
            {
                Session.Clear();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Please enter both username and password.", true);
                return;
            }

            UserBLL userBLL = new UserBLL();
            User user = userBLL.ValidateUser(username, password);

            if (user != null)
            {
                Session["UserId"] = user.UserId;
                Session["Username"] = user.Username;
                Session["FullName"] = user.FullName;
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                ShowMessage("Invalid username or password. Please try again.", true);
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isError ? "alert alert-danger" : "alert alert-success";
            lblMessage.Visible = true;
        }
    }
}
