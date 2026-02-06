<%@ Page Title="Inventory Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inventory.aspx.cs" Inherits="PharmacyInventoryAndBillingSystem.Inventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h1 style="color: #2c3e50; margin-bottom: 20px;">Inventory Management</h1>
        
        <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
        
        <div style="margin-bottom: 20px; display: flex; gap: 10px; align-items: center;">
            <asp:Button ID="btnAddMedicine" runat="server" Text="Add Medicine" CssClass="btn btn-primary" OnClick="btnAddMedicine_Click" />
            <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" CssClass="btn btn-success" OnClick="btnExportExcel_Click" />
            <asp:Button ID="btnExportPDF" runat="server" Text="Export to PDF" CssClass="btn btn-danger" OnClick="btnExportPDF_Click" />
        </div>
        
        <div class="table-container">
            <asp:GridView ID="gvMedicines" runat="server" CssClass="table" AutoGenerateColumns="false" 
                OnRowCommand="gvMedicines_RowCommand" DataKeyNames="MedicineId">
                <Columns>
                    <asp:BoundField DataField="MedicineId" HeaderText="ID" />
                    <asp:BoundField DataField="MedicineName" HeaderText="Medicine Name" />
                    <asp:BoundField DataField="BatchNo" HeaderText="Batch No" />
                    <asp:BoundField DataField="ExpiryDate" HeaderText="Expiry Date" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                    <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:C}" />
                    <asp:BoundField DataField="SellsPrice" HeaderText="Sells Price" DataFormatString="{0:C}" />
                    <asp:BoundField DataField="Description" HeaderText="Description" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-info btn-sm" 
                                CommandName="ViewRow" CommandArgument='<%# Eval("MedicineId") %>' />
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-warning btn-sm" 
                                CommandName="EditRow" CommandArgument='<%# Eval("MedicineId") %>' />
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" 
                                CommandName="DeleteRow" CommandArgument='<%# Eval("MedicineId") %>' 
                                OnClientClick="return confirm('Are you sure you want to delete this medicine?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        
        <!-- View/Edit Modal -->
        <div id="modalMedicine" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 1000;">
            <div style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); background: white; padding: 30px; border-radius: 8px; max-width: 500px; width: 90%;">
                <h3 id="modalTitle" style="margin-bottom: 20px;">Medicine Details</h3>
                <div class="form-group">
                    <label>Medicine Name</label>
                    <asp:TextBox ID="txtModalMedicineName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Batch No</label>
                    <asp:TextBox ID="txtModalBatchNo" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Expiry Date</label>
                    <asp:TextBox ID="txtModalExpiryDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Quantity</label>
                    <asp:TextBox ID="txtModalQuantity" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Unit Price</label>
                    <asp:TextBox ID="txtModalUnitPrice" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Sells Price</label>
                    <asp:TextBox ID="txtModalSellsPrice" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Description</label>
                    <asp:TextBox ID="txtModalDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                </div>
                <div style="text-align: right; margin-top: 20px;">
                    <asp:Button ID="btnSaveEdit" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveEdit_Click" style="display: none;" />
                    <asp:Button ID="btnSaveAdd" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveAdd_Click" style="display: none;" />
                    <button type="button" class="btn btn-secondary" onclick="closeModal()">Close</button>
                </div>
            </div>
        </div>
    </div>
    
    <script type="text/javascript">
        function openModal(mode) {
            document.getElementById('modalMedicine').style.display = 'block';
            var txtMedicineName = document.getElementById('<%= txtModalMedicineName.ClientID %>');
            var btnSaveEdit = document.getElementById('<%= btnSaveEdit.ClientID %>');
            var btnSaveAdd = document.getElementById('<%= btnSaveAdd.ClientID %>');
            
            if (mode === 'edit') {
                txtMedicineName.readOnly = false;
                document.getElementById('<%= txtModalBatchNo.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalExpiryDate.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalQuantity.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalUnitPrice.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalSellsPrice.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalDescription.ClientID %>').readOnly = false;
                btnSaveEdit.style.display = 'inline-block';
                btnSaveAdd.style.display = 'none';
                document.getElementById('modalTitle').innerText = 'Edit Medicine';
            } else if (mode === 'add') {
                // Clear all fields
                txtMedicineName.value = '';
                txtMedicineName.readOnly = false;
                document.getElementById('<%= txtModalBatchNo.ClientID %>').value = '';
                document.getElementById('<%= txtModalBatchNo.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalExpiryDate.ClientID %>').value = '';
                document.getElementById('<%= txtModalExpiryDate.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalQuantity.ClientID %>').value = '';
                document.getElementById('<%= txtModalQuantity.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalUnitPrice.ClientID %>').value = '';
                document.getElementById('<%= txtModalUnitPrice.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalSellsPrice.ClientID %>').value = '';
                document.getElementById('<%= txtModalSellsPrice.ClientID %>').readOnly = false;
                document.getElementById('<%= txtModalDescription.ClientID %>').value = '';
                document.getElementById('<%= txtModalDescription.ClientID %>').readOnly = false;
                btnSaveEdit.style.display = 'none';
                btnSaveAdd.style.display = 'inline-block';
                document.getElementById('modalTitle').innerText = 'Add Medicine';
            } else {
                // View mode
                txtMedicineName.readOnly = true;
                document.getElementById('<%= txtModalBatchNo.ClientID %>').readOnly = true;
                document.getElementById('<%= txtModalExpiryDate.ClientID %>').readOnly = true;
                document.getElementById('<%= txtModalQuantity.ClientID %>').readOnly = true;
                document.getElementById('<%= txtModalUnitPrice.ClientID %>').readOnly = true;
                document.getElementById('<%= txtModalSellsPrice.ClientID %>').readOnly = true;
                document.getElementById('<%= txtModalDescription.ClientID %>').readOnly = true;
                btnSaveEdit.style.display = 'none';
                btnSaveAdd.style.display = 'none';
                document.getElementById('modalTitle').innerText = 'View Medicine';
            }
        }
        
        function closeModal() {
            document.getElementById('modalMedicine').style.display = 'none';
        }
    </script>
</asp:Content>
