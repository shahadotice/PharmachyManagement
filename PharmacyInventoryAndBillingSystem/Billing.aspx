<%@ Page Title="Billing System" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Billing.aspx.cs" Inherits="PharmacyInventoryAndBillingSystem.Billing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
        
        <!-- Invoice List View -->
        <div id="invoiceListView" runat="server">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
                <h1 style="color: #2c3e50; margin: 0;">Invoice List</h1>
                <asp:Button ID="btnAddNewInvoice" runat="server" Text="Add New Invoice" CssClass="btn btn-primary" OnClick="btnAddNewInvoice_Click" />
            </div>
            
            <div class="table-container">
                <asp:GridView ID="gvInvoices" runat="server" CssClass="table" AutoGenerateColumns="false" 
                    OnRowCommand="gvInvoices_RowCommand" DataKeyNames="SalesId">
                    <Columns>
                        <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice Number" />
                        <asp:BoundField DataField="InvoiceDate" HeaderText="Invoice Date" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" />
                        <asp:BoundField DataField="CustomerContact" HeaderText="Customer Contact" />
                        <asp:BoundField DataField="SubTotal" HeaderText="Sub Total" DataFormatString="{0:F2}" />
                        <asp:BoundField DataField="Discount" HeaderText="Discount" DataFormatString="{0:F2}" />
                        <asp:BoundField DataField="GrandTotal" HeaderText="Grand Total" DataFormatString="{0:F2}" ItemStyle-Font-Bold="true" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-info btn-sm" 
                                    CommandName="EditInvoice" CommandArgument='<%# Eval("SalesId") %>' />
                                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" 
                                    CommandName="DeleteInvoice" CommandArgument='<%# Eval("SalesId") %>' 
                                    OnClientClick="return confirm('Are you sure you want to delete this invoice?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        
        <!-- Invoice Form View -->
        <div id="invoiceFormView" runat="server" visible="false">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
                <h1 style="color: #2c3e50; margin: 0;">Billing System</h1>
                <asp:Button ID="btnBackToList" runat="server" Text="Back to List" CssClass="btn btn-secondary" OnClick="btnBackToList_Click" CausesValidation="false" OnClientClick="return true;" />
            </div>
            
            <div class="billing-form">
            <!-- Master Section (Invoice Header) -->
            <div class="billing-section">
                <h3>Invoice Information</h3>
                <div class="form-row">
                    <div class="form-group">
                        <label for="txtInvoiceNumber">Invoice Number</label>
                        <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtInvoiceDate">Invoice Date</label>
                        <asp:TextBox ID="txtInvoiceDate" runat="server" CssClass="form-control" TextMode="Date" required="true"></asp:TextBox>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group">
                        <label for="txtCustomerName">Customer Name</label>
                        <asp:TextBox ID="txtCustomerName" runat="server" CssClass="form-control" placeholder="Enter customer name"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtCustomerContact">Customer Contact</label>
                        <asp:TextBox ID="txtCustomerContact" runat="server" CssClass="form-control" placeholder="Enter contact number"></asp:TextBox>
                    </div>
                </div>
            </div>
            
            <!-- Detail Section (Dynamic Item Grid) -->
            <div class="billing-section">
                <h3>Medicine Details</h3>
                
                <!-- Input Section for Adding/Editing Medicine -->
                <div style="background: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px;">
                    <div class="form-row" style="display: flex; gap: 10px; align-items: flex-end; flex-wrap: wrap; align-content: flex-end;">
                        <div class="form-group" style="flex: 1; min-width: 150px;">
                            <label>Medicine</label>
                            <select id="ddlMedicine" class="form-control">
                                <option value="">Select Medicine</option>
                            </select>
                        </div>
                        <div class="form-group" style="flex: 0 0 120px;">
                            <label>Batch No</label>
                            <input type="text" id="txtBatchNo" class="form-control" readonly />
                        </div>
                        <div class="form-group" style="flex: 0 0 140px;">
                            <label>Expiry Date</label>
                            <input type="date" id="txtExpiryDate" class="form-control" readonly />
                        </div>
                        <div class="form-group" style="flex: 0 0 100px;">
                            <label>Quantity</label>
                            <input type="number" id="txtQuantity" class="form-control" min="1" value="1" />
                        </div>
                        <div class="form-group" style="flex: 0 0 120px;">
                            <label>Unit Price</label>
                            <input type="number" id="txtUnitPrice" class="form-control" step="0.01" />
                        </div>
                        <div class="form-group" style="flex: 0 0 120px;">
                            <label>Line Total</label>
                            <input type="text" id="txtLineTotal" class="form-control" readonly />
                            <input type="hidden" id="txtStockQty" />
                        </div>
                        <div style="flex: 0 0 auto; margin-bottom: 20px; display: flex; flex-direction: column;">
                            <label style="display: block; margin-bottom: 5px; font-weight: 600; color: #555; line-height: 1.6;">&nbsp;</label>
                            <div style="display: flex; gap: 5px;">
                                <button type="button" id="btnAddMedicine" class="btn btn-success" style="height: 38px; padding: 10px 20px; box-sizing: border-box; line-height: 1.6; display: inline-flex; align-items: center; justify-content: center;">Add</button>
                                <button type="button" id="btnUpdateMedicine" class="btn btn-primary" style="display: none; height: 38px; padding: 10px 20px; box-sizing: border-box; line-height: 1.6; display: inline-flex; align-items: center; justify-content: center;">Update</button>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Medicine List Table -->
                <div class="table-container">
                    <table id="tblDetails" class="details-table">
                        <thead>
                            <tr>
                                <th>Medicine Name</th>
                                <th>Batch No</th>
                                <th>Expiry Date</th>
                                <th>Quantity</th>
                                <th>Unit Price</th>
                                <th>Line Total</th>
                                <th>Action</th>
                            </tr>
                        </thead>
                        <tbody id="tbodyDetails">
                            <tr id="emptyRow">
                                <td colspan="7" style="text-align: center; padding: 20px; color: #999;">
                                    No medicine added. Select a medicine and click "Add" to add items.
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            
            <!-- Totals Section -->
            <div class="totals-section">
                <div class="totals-row">
                    <span>Sub Total:</span>
                    <span id="lblSubTotal">0.00</span>
                </div>
                <div class="totals-row" style="display: flex; justify-content: flex-end; align-items: center; gap: 10px; margin-top: 15px;">
                    <label for="txtDiscount" style="margin: 0;">Discount:</label>
                    <asp:TextBox ID="txtDiscount" runat="server" CssClass="form-control" TextMode="Number" step="0.01" value="0" onchange="calculateTotals()" style="width: 120px; display: inline-block; margin: 0;"></asp:TextBox>
                    <div style="display: flex; align-items: center; margin: 0;">
                        <asp:CheckBox ID="chkDiscountPercentage" runat="server" onchange="calculateTotals()" style="margin: 0; vertical-align: middle;" />
                        <label for="<%= chkDiscountPercentage.ClientID %>" style="margin: 0 0 0 5px; vertical-align: middle; cursor: pointer;">Percentage</label>
                    </div>
                </div>
                <div class="totals-row">
                    <span><strong>Grand Total:</strong></span>
                    <span id="lblGrandTotal"><strong>0.00</strong></span>
                </div>
            </div>
            
            <div style="margin-top: 30px; text-align: center;">
                <asp:Button ID="btnSave" runat="server" Text="Save Invoice" CssClass="btn btn-primary" OnClientClick="return saveInvoice();" OnClick="btnSave_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning" OnClick="btnClear_Click" CausesValidation="false" OnClientClick="return true;" />
            </div>
        </div>
        </div>
    </div>
    
    <asp:HiddenField ID="hdnDetailsJSON" runat="server" />
    <asp:HiddenField ID="hdnSalesId" runat="server" />
    
    <script src="Scripts/jquery-3.4.1.min.js"></script>
    <script type="text/javascript">
        var medicines = [];
        var editingRowId = null;
        var medicineRows = {}; 
        
        $(document).ready(function () {
            // Initialize button visibility - Add visible, Update hidden
            $('#btnAddMedicine').css('display', 'inline-flex');
            $('#btnUpdateMedicine').css('display', 'none');
            editingRowId = null;
          
            var formView = document.getElementById('<%= invoiceFormView.ClientID %>');
            if (formView && formView.style.display !== 'none' && formView.offsetParent !== null) {
               
                setTimeout(function() {
                    if (typeof medicines !== 'undefined' && Array.isArray(medicines)) {
                        initializeMedicinesIfNeeded();
                    } else {
                        console.error('Medicines not loaded yet');
                    }
                }, 200);
            }
            
          
            $('#ddlMedicine').change(function() {
                loadMedicineDetails();
            });
            
          
            $('#txtQuantity').on('input change', function() {
                calculateLineTotal();
            });
            
            // Add event handler for unit price changes
            $('#txtUnitPrice').on('input change', function() {
                calculateLineTotal();
            });
            
            // Add event handler for discount percentage checkbox
            $('#<%= chkDiscountPercentage.ClientID %>').on('change', function() {
                calculateTotals();
            });
            
           
            $('#btnAddMedicine').click(function() {
                addMedicineToTable();
            });
            
        
            $('#btnUpdateMedicine').click(function() {
                updateMedicineInTable();
            });
            
            
            var invoiceDate = $('#<%= txtInvoiceDate.ClientID %>').val();
            if (!invoiceDate) {
                var today = new Date().toISOString().split('T')[0];
                $('#<%= txtInvoiceDate.ClientID %>').val(today);
            }
            
          
            $('#<%= btnClear.ClientID %>').attr('formnovalidate', 'formnovalidate');
            $('#<%= btnBackToList.ClientID %>').attr('formnovalidate', 'formnovalidate');
        });
        
       
        function initializeMedicinesIfNeeded() {
           
            if (typeof medicines === 'undefined' || !Array.isArray(medicines)) {
                console.warn('Medicines array not initialized, initializing as empty array');
                medicines = [];
            }
            
            console.log('Initializing medicines. Count:', medicines.length);
            
            
            populateMedicineDropdown();
            
          
            var salesId = $('#<%= hdnSalesId.ClientID %>').val();
            var detailsJson = $('#<%= hdnDetailsJSON.ClientID %>').val();
            if (salesId && detailsJson) {
                loadInvoiceDetails();
            }
        }
        
        function populateMedicineDropdown() {
            var ddl = $('#ddlMedicine');
            ddl.empty();
            ddl.append('<option value="">Select Medicine</option>');
            
            if (!medicines || medicines.length === 0) {
                console.warn('No medicines available to populate dropdown');
                return;
            }
            
            for (var i = 0; i < medicines.length; i++) {
                var med = medicines[i];
                if (med && med.MedicineId && med.MedicineName) {
                    ddl.append('<option value="' + med.MedicineId + '" data-batch="' + (med.BatchNo || '') + '" data-expiry="' + (med.ExpiryDate || '') + '" data-price="' + (med.SellsPrice || 0) + '" data-stockqty="' + (med.Quantity || 0) + '">' + med.MedicineName + '</option>');
                }
            }
            
            console.log('Medicine dropdown populated with', medicines.length, 'items');
        }
        
        function loadMedicineDetails() {
            var selectedOption = $('#ddlMedicine option:selected');
            if (selectedOption.val()) {
                var batchNo = selectedOption.data('batch') || '';
                var expiryDate = selectedOption.data('expiry') || '';
                var unitPrice = selectedOption.data('price') || 0;
                var stockQty = selectedOption.data('stockqty') || 0;
                
                $('#txtBatchNo').val(batchNo);
                $('#txtExpiryDate').val(expiryDate);
                $('#txtUnitPrice').val(unitPrice);
                $('#txtQuantity').val(1);
                $('#txtStockQty').val(stockQty);
                calculateLineTotal();
            } else {
                clearMedicineInputs();
            }
        }
        
        function calculateLineTotal() {
            var quantity = parseFloat($('#txtQuantity').val()) || 0;
            var unitPrice = parseFloat($('#txtUnitPrice').val()) || 0;
            var lineTotal = quantity * unitPrice;
            $('#txtLineTotal').val(lineTotal.toFixed(2));
            calculateTotals();
        }
        
        function clearMedicineInputs() {
            $('#ddlMedicine').val('');
            $('#txtBatchNo').val('');
            $('#txtExpiryDate').val('');
            $('#txtQuantity').val(1);
            $('#txtUnitPrice').val('');
            $('#txtLineTotal').val('');
            // Reset to add mode
            $('#btnAddMedicine').css('display', 'inline-flex');
            $('#btnUpdateMedicine').css('display', 'none');
            editingRowId = null;
        }
        
        function addMedicineToTable() {
            var medicineId = $('#ddlMedicine').val();
            if (!medicineId) {
                alert('Please select a medicine');
                return;
            }
            
            var selectedOption = $('#ddlMedicine option:selected');
            var medicineName = selectedOption.text();
            var batchNo = $('#txtBatchNo').val();
            var expiryDate = $('#txtExpiryDate').val();
            var quantity = parseInt($('#txtQuantity').val()) || 0;
            var unitPrice = parseFloat($('#txtUnitPrice').val()) || 0;
            var lineTotal = parseFloat($('#txtLineTotal').val()) || 0;
            var stockQty = $('#txtStockQty').val();
            
            if (quantity <= 0) {
                alert('Please enter a valid quantity');
                return;
            }
            if (quantity > stockQty) {
                alert('Stock Quantity is ' + stockQty +'. Quantity can not greater then stock Quantity.');
                return;
            }
            
            
            if (medicineRows[medicineId]) {
                
                var existingRow = medicineRows[medicineId];
                var existingQuantity = parseInt(existingRow.find('.row-quantity').text());
                var newQuantity = existingQuantity + quantity;
                var newLineTotal = newQuantity * unitPrice;
                
                existingRow.find('.row-quantity').text(newQuantity);
                existingRow.find('.row-line-total').text(newLineTotal.toFixed(2));
                existingRow.data('quantity', newQuantity);
                existingRow.data('line-total', newLineTotal);
            } else {
               
                $('#emptyRow').hide();
                
                var rowId = 'row_' + medicineId;
                var row = $('<tr id="' + rowId + '"></tr>');
                row.data('medicine-id', medicineId);
                row.data('quantity', quantity);
                row.data('line-total', lineTotal);
                
                row.append('<td class="row-medicine-name">' + medicineName + '</td>');
                row.append('<td class="row-batch-no">' + batchNo + '</td>');
                row.append('<td class="row-expiry-date">' + expiryDate + '</td>');
                row.append('<td class="row-quantity">' + quantity + '</td>');
                row.append('<td class="row-unit-price">' + unitPrice.toFixed(2) + '</td>');
                row.append('<td class="row-line-total">' + lineTotal.toFixed(2) + '</td>');
                row.append('<td>' +
                    '<button type="button" class="btn btn-warning btn-sm btn-edit-row">Edit</button> ' +
                    '<button type="button" class="btn btn-danger btn-sm btn-delete-row">Delete</button>' +
                    '</td>');
                
                $('#tbodyDetails').append(row);
                medicineRows[medicineId] = row;
                
               
                row.find('.btn-edit-row').click(function() {
                    editRow(row);
                });
                row.find('.btn-delete-row').click(function() {
                    deleteRow(row);
                });
            }
            
            clearMedicineInputs();
            calculateTotals();
        }
        
        function editRow(row) {
            var medicineId = row.data('medicine-id');
            var medicineName = row.find('.row-medicine-name').text();
            var batchNo = row.find('.row-batch-no').text();
            var expiryDate = row.find('.row-expiry-date').text();
            var quantity = row.find('.row-quantity').text();
            var unitPrice = row.find('.row-unit-price').text();
            
          
            $('#ddlMedicine').val(medicineId).trigger('change');
            $('#txtBatchNo').val(batchNo);
            $('#txtExpiryDate').val(expiryDate);
            $('#txtQuantity').val(quantity);
            $('#txtUnitPrice').val(unitPrice);
            calculateLineTotal();
            
            // Switch to edit mode - hide Add, show Update
            $('#btnAddMedicine').css('display', 'none');
            $('#btnUpdateMedicine').css('display', 'inline-flex');
            
            editingRowId = row.attr('id');
        }
        
        function updateMedicineInTable() {
            if (!editingRowId) {
                return;
            }
            
            var row = $('#' + editingRowId);
            var medicineId = row.data('medicine-id');
            var selectedOption = $('#ddlMedicine option:selected');
            var medicineName = selectedOption.text();
            var batchNo = $('#txtBatchNo').val();
            var expiryDate = $('#txtExpiryDate').val();
            var quantity = parseInt($('#txtQuantity').val()) || 0;
            var unitPrice = parseFloat($('#txtUnitPrice').val()) || 0;
            var lineTotal = parseFloat($('#txtLineTotal').val()) || 0;
            var stockQty = $('#txtStockQty').val();
            if (quantity <= 0) {
                alert('Please enter a valid quantity');
                return;
            }

            if (quantity > stockQty) {
                alert('Stock Quantity is ' + stockQty + '. Quantity can not greater then stock Quantity.');
                return;
            }
         
            row.find('.row-medicine-name').text(medicineName);
            row.find('.row-batch-no').text(batchNo);
            row.find('.row-expiry-date').text(expiryDate);
            row.find('.row-quantity').text(quantity);
            row.find('.row-unit-price').text(unitPrice.toFixed(2));
            row.find('.row-line-total').text(lineTotal.toFixed(2));
            row.data('quantity', quantity);
            row.data('line-total', lineTotal);
            
            cancelEdit();
            calculateTotals();
        }
        
        function cancelEdit() {
            clearMedicineInputs();
            // clearMedicineInputs already handles button visibility, but ensure it's set
            $('#btnAddMedicine').css('display', 'inline-flex');
            $('#btnUpdateMedicine').css('display', 'none');
            editingRowId = null;
        }
        
        function deleteRow(row) {
            if (confirm('Are you sure you want to delete this medicine?')) {
                var medicineId = row.data('medicine-id');
                delete medicineRows[medicineId];
                row.remove();
                
                
                if ($('#tbodyDetails tr:not(#emptyRow)').length === 0) {
                    $('#emptyRow').show();
                }
                
                calculateTotals();
            }
        }
        
        function loadInvoiceDetails() {
            var detailsJson = $('#<%= hdnDetailsJSON.ClientID %>').val();
            if (detailsJson) {
                try {
                    var details = JSON.parse(detailsJson);
                    $('#tbodyDetails').empty();
                    $('#emptyRow').hide();
                    medicineRows = {};
                    
                    for (var i = 0; i < details.length; i++) {
                        addRowFromInvoiceDetail(details[i]);
                    }
                    
                    calculateTotals();
                } catch (e) {
                    console.error("Error parsing invoice details:", e);
                }
            }
        }
        
        function addRowFromInvoiceDetail(detail) {
            if (!detail || !detail.MedicineId) {
                return;
            }
            
            var medicineId = detail.MedicineId;
            var medicineName = detail.MedicineName || '';
            var batchNo = detail.BatchNo || '';
            var expiryDate = detail.ExpiryDate || '';
            var quantity = detail.Quantity || 0;
            var unitPrice = detail.UnitPrice || 0;
            var lineTotal = detail.LineTotal || (quantity * unitPrice);
            
            $('#emptyRow').hide();
            
            var rowId = 'row_' + medicineId;
            var row = $('<tr id="' + rowId + '"></tr>');
            row.data('medicine-id', medicineId);
            row.data('quantity', quantity);
            row.data('line-total', lineTotal);
            
            row.append('<td class="row-medicine-name">' + medicineName + '</td>');
            row.append('<td class="row-batch-no">' + batchNo + '</td>');
            row.append('<td class="row-expiry-date">' + expiryDate + '</td>');
            row.append('<td class="row-quantity">' + quantity + '</td>');
            row.append('<td class="row-unit-price">' + unitPrice.toFixed(2) + '</td>');
            row.append('<td class="row-line-total">' + lineTotal.toFixed(2) + '</td>');
            row.append('<td>' +
                '<button type="button" class="btn btn-warning btn-sm btn-edit-row">Edit</button> ' +
                '<button type="button" class="btn btn-danger btn-sm btn-delete-row">Delete</button>' +
                '</td>');
            
            $('#tbodyDetails').append(row);
            medicineRows[medicineId] = row;
            
          
            row.find('.btn-edit-row').click(function() {
                editRow(row);
            });
            row.find('.btn-delete-row').click(function() {
                deleteRow(row);
            });
        }
        
        function calculateTotals() {
            var subTotal = 0;
            $('#tbodyDetails tr:not(#emptyRow)').each(function() {
                var lineTotal = parseFloat($(this).find('.row-line-total').text()) || 0;
                subTotal += lineTotal;
            });
            
            var discountValue = parseFloat($('#<%= txtDiscount.ClientID %>').val()) || 0;
            var isPercentage = $('#<%= chkDiscountPercentage.ClientID %>').is(':checked');
            var discount = 0;
            
            if (isPercentage) {
                // Calculate discount as percentage of subtotal
                discount = (subTotal * discountValue) / 100;
            } else {
                // Use discount as fixed amount
                discount = discountValue;
            }
            
            var grandTotal = subTotal - discount;
            
            // Ensure grand total doesn't go negative
            if (grandTotal < 0) {
                grandTotal = 0;
            }
            
            $('#lblSubTotal').text(subTotal.toFixed(2));
            $('#lblGrandTotal').text(grandTotal.toFixed(2));
        }
        
        function collectDetails() {
            var details = [];
            $('#tbodyDetails tr:not(#emptyRow)').each(function() {
                var medicineId = $(this).data('medicine-id');
                if (medicineId) {
                    details.push({
                        MedicineId: parseInt(medicineId),
                        BatchNo: $(this).find('.row-batch-no').text(),
                        ExpiryDate: $(this).find('.row-expiry-date').text(),
                        Quantity: parseInt($(this).find('.row-quantity').text()),
                        UnitPrice: parseFloat($(this).find('.row-unit-price').text()),
                        LineTotal: parseFloat($(this).find('.row-line-total').text())
                    });
                }
            });
            return details;
        }
        
        function saveInvoice() {
            var details = collectDetails();
            
            if (details.length === 0) {
                alert('Please add at least one medicine to the invoice.');
                return false;
            }
            
            
            if (!$('#<%= txtCustomerName.ClientID %>').val()) {
                alert('Please enter customer name.');
                return false;
            }
            
          
            $('#<%= hdnDetailsJSON.ClientID %>').val(JSON.stringify(details));
            
            return true;
        }
    </script>
</asp:Content>
