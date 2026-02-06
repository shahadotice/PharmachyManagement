<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="PharmacyInventoryAndBillingSystem.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h1 style="color: #2c3e50; margin-bottom: 30px;">Welcome to Pharmacy Management System</h1>


        <div class="dashboard" style="margin-top: 40px;">
            <div class="dashboard-card">
                <h3>Inventory Management</h3>
                <p>Manage medicine stocks, view, edit, and delete inventory items.</p>
                <a href="Inventory.aspx" class="btn btn-primary">Go to Inventory</a>
            </div>

            <div class="dashboard-card">
                <h3>Billing System</h3>
                <p>Create sales invoices with multiple medicines in a single transaction.</p>
                <a href="Billing.aspx" class="btn btn-success">Go to Billing</a>
            </div>
        </div>

        <div class="statistics-section">



            <div class="statistics-group">
                <h3 style="color: #34495e; margin-bottom: 15px; padding-bottom: 10px; border-bottom: 2px solid #ecf0f1;">As on
                    <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
                </h3>
                <div class="dashboard">
                    <div class="stat-card stat-card-today" id="statStockAmount" style="cursor: pointer;" onclick="openStockModal()">

                        <div class="stat-content">
                            <div class="stat-label">Total Stock Amount</div>
                            <div class="stat-value">
                                <asp:Label ID="lblTodaySalesCount" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="stat-card stat-card-today" id="statSalesAmount" style="cursor: pointer;" onclick="openSalesModal()">

                        <div class="stat-content">
                            <div class="stat-label">Total Sales Amount</div>
                            <div class="stat-value">
                                <asp:Label ID="lblTodayRevenue" runat="server" Text="0.00"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>



        </div>


    </div>

    <!-- Stock Details Modal -->
    <div id="modalStockDetails" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 1000;">
        <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); background: white; padding: 30px; border-radius: 8px; max-width: 800px; width: 90%; max-height: 90vh; overflow-y: auto;">
            <h3 style="margin-bottom: 20px; color: #2c3e50;">Stock Details</h3>
            <div class="table-container">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Medicine Name</th>
                            <th>Batch No</th>
                            <th style="text-align: right;">Total Price</th>
                        </tr>
                    </thead>
                    <tbody id="tblStockDetails">
                        <!-- Data will be populated by JavaScript -->
                    </tbody>
                    <tfoot>
                        <tr style="font-weight: bold; background-color: #f8f9fa;">
                            <td colspan="2" style="text-align: right;">Total:</td>
                            <td style="text-align: right;" id="stockTotalAmount">0.00</td>
                        </tr>
                    </tfoot>
                </table>
            </div>
            <div style="text-align: right; margin-top: 20px;">
                <button type="button" class="btn btn-secondary" onclick="closeStockModal()">Close</button>
            </div>
        </div>
    </div>

    <!-- Sales Details Modal -->
    <div id="modalSalesDetails" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 1000;">
        <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); background: white; padding: 30px; border-radius: 8px; max-width: 800px; width: 90%; max-height: 90vh; overflow-y: auto;">
            <h3 style="margin-bottom: 20px; color: #2c3e50;">Sales Details</h3>
            <div class="table-container">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Invoice No</th>
                            <th>Invoice Date</th>
                            <th style="text-align: right;">Total Amount</th>
                        </tr>
                    </thead>
                    <tbody id="tblSalesDetails">
                        <!-- Data will be populated by JavaScript -->
                    </tbody>
                    <tfoot>
                        <tr style="font-weight: bold; background-color: #f8f9fa;">
                            <td colspan="2" style="text-align: right;">Total:</td>
                            <td style="text-align: right;" id="salesTotalAmount">0.00</td>
                        </tr>
                    </tfoot>
                </table>
            </div>
            <div style="text-align: right; margin-top: 20px;">
                <button type="button" class="btn btn-secondary" onclick="closeSalesModal()">Close</button>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // Initialize arrays (will be populated by server-side script)
        if (typeof stockDetails === 'undefined') {
            var stockDetails = [];
        }
        if (typeof salesDetails === 'undefined') {
            var salesDetails = [];
        }

        function openStockModal() {
            populateStockTable();
            document.getElementById('modalStockDetails').style.display = 'block';
        }

        function closeStockModal() {
            document.getElementById('modalStockDetails').style.display = 'none';
        }

        function openSalesModal() {
            populateSalesTable();
            document.getElementById('modalSalesDetails').style.display = 'block';
        }

        function closeSalesModal() {
            document.getElementById('modalSalesDetails').style.display = 'none';
        }

        function populateStockTable() {
            var tbody = document.getElementById('tblStockDetails');
            tbody.innerHTML = '';
            var total = 0;

            if (typeof stockDetails !== 'undefined' && stockDetails && stockDetails.length > 0) {
                for (var i = 0; i < stockDetails.length; i++) {
                    var item = stockDetails[i];
                    var row = document.createElement('tr');
                    row.innerHTML = '<td>' + (item.MedicineName || '') + '</td>' +
                                   '<td>' + (item.BatchNo || '') + '</td>' +
                                   '<td style="text-align: right;">' + parseFloat(item.UnitPrice || 0).toFixed(2) + '</td>';
                    tbody.appendChild(row);
                    total += parseFloat(item.UnitPrice || 0);
                }
            } else {
                var row = document.createElement('tr');
                row.innerHTML = '<td colspan="3" style="text-align: center;">No stock details available</td>';
                tbody.appendChild(row);
            }

            document.getElementById('stockTotalAmount').textContent = total.toFixed(2);
        }

        function populateSalesTable() {
            var tbody = document.getElementById('tblSalesDetails');
            tbody.innerHTML = '';
            var total = 0;

            if (typeof salesDetails !== 'undefined' && salesDetails && salesDetails.length > 0) {
                for (var i = 0; i < salesDetails.length; i++) {
                    var item = salesDetails[i];
                    var row = document.createElement('tr');
                    row.innerHTML = '<td>' + (item.InvoiceNumber || '') + '</td>' +
                                   '<td>' + (item.InvoiceDate || '') + '</td>' +
                                   '<td style="text-align: right;">' + parseFloat(item.GrandTotal || 0).toFixed(2) + '</td>';
                    tbody.appendChild(row);
                    total += parseFloat(item.GrandTotal || 0);
                }
            } else {
                var row = document.createElement('tr');
                row.innerHTML = '<td colspan="3" style="text-align: center;">No sales details available</td>';
                tbody.appendChild(row);
            }

            document.getElementById('salesTotalAmount').textContent = total.toFixed(2);
        }

        // Close modal when clicking outside
        window.onclick = function(event) {
            var stockModal = document.getElementById('modalStockDetails');
            var salesModal = document.getElementById('modalSalesDetails');
            if (event.target == stockModal) {
                closeStockModal();
            }
            if (event.target == salesModal) {
                closeSalesModal();
            }
        }
    </script>
</asp:Content>
