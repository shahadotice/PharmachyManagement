<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PharmacyInventoryAndBillingSystem.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Login - Pharmacy Management System</title>
    <link href="Content/Pharmacy.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="login-box">
                <h2>Pharmacy Management System</h2>
                <h3 style="text-align: center; margin-bottom: 30px; color: #666;">Login</h3>
                
                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label>
                
                <div class="form-group">
                    <label for="txtUsername">Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter username" required="true"></asp:TextBox>
                </div>
                
                <div class="form-group">
                    <label for="txtPassword">Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter password" required="true"></asp:TextBox>
                </div>
                
                <div class="form-group">
                    <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary" style="width: 100%;" OnClick="btnLogin_Click" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
