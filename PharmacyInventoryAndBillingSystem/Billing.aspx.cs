using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Serialization;
using PharmacyInventoryAndBillingSystem.BLL;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem
{
    public partial class Billing : System.Web.UI.Page
    {
        private SalesBLL salesBLL;
        private MedicineBLL medicineBLL;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            salesBLL = new SalesBLL();
            medicineBLL = new MedicineBLL();

            if (!IsPostBack)
            {
                
                string salesIdParam = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(salesIdParam))
                {
                    int salesId = Convert.ToInt32(salesIdParam);
                    LoadInvoiceForEdit(salesId);
                }
                else
                {
                    ShowInvoiceList();
                }
            }
        }

        private void ShowInvoiceList()
        {
            invoiceListView.Visible = true;
            invoiceFormView.Visible = false;
           
            lblMessage.Visible = false;
            lblMessage.Text = "";
            LoadInvoices();
        }

        private void ShowInvoiceForm()
        {
            invoiceListView.Visible = false;
            invoiceFormView.Visible = true;
            
            
            lblMessage.Visible = false;
            lblMessage.Text = "";
            
            if (string.IsNullOrEmpty(hdnSalesId.Value))
            {
               
                txtInvoiceNumber.Text = salesBLL.GetNextInvoiceNumber();
                txtInvoiceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtCustomerName.Text = "";
                txtCustomerContact.Text = "";
                txtDiscount.Text = "0";
            }
            
          
            LoadMedicinesForJavaScript();
            
          
            string script = "setTimeout(function() { if (typeof medicines !== 'undefined' && medicines.length > 0) { initializeMedicinesIfNeeded(); } }, 100);";
            ClientScript.RegisterStartupScript(this.GetType(), "InitializeMedicines", script, true);
        }

        private void LoadMedicinesForJavaScript()
        {
            try
            {
                List<Medicine> medicines = medicineBLL.GetAllMedicines();
                List<MedicineDTO> medicineDTOs = new List<MedicineDTO>();

                foreach (var med in medicines)
                {
                    if (med != null)
                    {
                        medicineDTOs.Add(new MedicineDTO
                        {
                            MedicineId = med.MedicineId,
                            MedicineName = med.MedicineName ?? "",
                            BatchNo = med.BatchNo ?? "",
                            ExpiryDate = med.ExpiryDate.ToString("yyyy-MM-dd"),
                            UnitPrice = med.UnitPrice,
                            SellsPrice = med.SellsPrice,
                            Quantity = med.Quantity
                        });
                    }
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string medicinesJson = serializer.Serialize(medicineDTOs);
                
                
                string script = "var medicines = " + medicinesJson + ";";
                ClientScript.RegisterStartupScript(this.GetType(), "LoadMedicines", script, true);
            }
            catch (Exception ex)
            {
                
                string script = "var medicines = []; console.error('Error loading medicines:', '" + ex.Message.Replace("'", "\\'") + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "LoadMedicinesError", script, true);
            }
        }

        private void LoadInvoices()
        {
            List<SalesMaster> invoices = salesBLL.GetAllInvoices();
            gvInvoices.DataSource = invoices;
            gvInvoices.DataBind();
        }

        protected void btnAddNewInvoice_Click(object sender, EventArgs e)
        {
            hdnSalesId.Value = "";
            ShowInvoiceForm();
        }

        protected void btnBackToList_Click(object sender, EventArgs e)
        {
            ShowInvoiceList();
        }

        protected void gvInvoices_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditInvoice")
            {
                int salesId = Convert.ToInt32(e.CommandArgument);
                LoadInvoiceForEdit(salesId);
            }
            else if (e.CommandName == "DeleteInvoice")
            {
                int salesId = Convert.ToInt32(e.CommandArgument);
                if (salesBLL.DeleteInvoice(salesId))
                {
                    ShowMessage("Invoice deleted successfully!", false);
                    LoadInvoices();
                }
                else
                {
                    ShowMessage("Error deleting invoice. Please try again.", true);
                }
            }
        }

        private void LoadInvoiceForEdit(int salesId)
        {
            SalesMaster invoice = salesBLL.GetInvoiceById(salesId);
            if (invoice != null)
            {
                hdnSalesId.Value = salesId.ToString();
                txtInvoiceNumber.Text = invoice.InvoiceNumber;
                txtInvoiceDate.Text = invoice.InvoiceDate.ToString("yyyy-MM-dd");
                txtCustomerName.Text = invoice.CustomerName;
                txtCustomerContact.Text = invoice.CustomerContact;
                txtDiscount.Text = invoice.Discount.ToString("F2");
                
      
                if (invoice.SalesDetails != null && invoice.SalesDetails.Count > 0)
                {
                    List<SalesDetailDTO> detailDTOs = new List<SalesDetailDTO>();
                    foreach (var detail in invoice.SalesDetails)
                    {
                        detailDTOs.Add(new SalesDetailDTO
                        {
                            MedicineId = detail.MedicineId,
                            MedicineName = detail.MedicineName,
                            BatchNo = detail.BatchNo,
                            ExpiryDate = detail.ExpiryDate.ToString("yyyy-MM-dd"),
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice,
                            LineTotal = detail.LineTotal
                        });
                    }
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    hdnDetailsJSON.Value = serializer.Serialize(detailDTOs);
                }
              
                LoadMedicinesForJavaScript();
                
                ShowInvoiceForm();
                
               
                string script = "loadInvoiceDetails();";
                ClientScript.RegisterStartupScript(this.GetType(), "LoadInvoiceDetails", script, true);
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isError ? "alert alert-danger" : "alert alert-success";
            lblMessage.Visible = true;
        }

        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json, UseHttpGet = false)]
        public static List<MedicineDTO> GetMedicines()
        {
            try
            {
          
                MedicineBLL medicineBLL = new MedicineBLL();
                List<Medicine> medicines = medicineBLL.GetAllMedicines();
                
                if (medicines == null)
                {
                   
                    return new List<MedicineDTO>();
                }
                
                List<MedicineDTO> medicineDTOs = new List<MedicineDTO>();

                foreach (var med in medicines)
                {
                    if (med != null)
                    {
                        medicineDTOs.Add(new MedicineDTO
                        {
                            MedicineId = med.MedicineId,
                            MedicineName = med.MedicineName ?? "",
                            BatchNo = med.BatchNo ?? "",
                            ExpiryDate = med.ExpiryDate.ToString("yyyy-MM-dd"),
                            UnitPrice = med.UnitPrice,
                            SellsPrice = med.SellsPrice,
                            Quantity = med.Quantity
                        });
                    }
                }

                return medicineDTOs;
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {

                return new List<MedicineDTO>();
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                

                return new List<MedicineDTO>();
            }
        }

        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public static StockValidationResult ValidateStock(int medicineId, int quantity)
        {
            try
            {

                MedicineBLL medicineBLL = new MedicineBLL();
                return medicineBLL.ValidateStock(medicineId, quantity);
            }
            catch (Exception ex)
            {
                
                return new StockValidationResult
                {
                    IsValid = false,
                    Message = "Error validating stock: " + ex.Message,
                    AvailableQuantity = 0
                };
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string detailsJson = hdnDetailsJSON.Value;
                if (string.IsNullOrEmpty(detailsJson))
                {
                    ShowMessage("Please add at least one medicine to the invoice.", true);
                    return;
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<SalesDetailDTO> detailDTOs = serializer.Deserialize<List<SalesDetailDTO>>(detailsJson);

                if (detailDTOs == null || detailDTOs.Count == 0)
                {
                    ShowMessage("Please add at least one medicine to the invoice.", true);
                    return;
                }


                foreach (var detail in detailDTOs)
                {
                    var stockValidation = medicineBLL.ValidateStock(detail.MedicineId, detail.Quantity);
                    if (!stockValidation.IsValid)
                    {
                        ShowMessage(stockValidation.Message, true);
                        return;
                    }
                }

     
                decimal subTotal = CalculateSubTotal(detailDTOs);
                decimal discountValue = Convert.ToDecimal(txtDiscount.Text);
                decimal discount = 0;
                
                
                if (chkDiscountPercentage.Checked)
                {
                   
                    discount = (subTotal * discountValue) / 100;
                }
                else
                {
                    discount = discountValue;
                }
                
                decimal grandTotal = subTotal - discount;
                if (grandTotal < 0) grandTotal = 0;
                
                SalesMaster salesMaster = new SalesMaster
                {
                    SalesId = string.IsNullOrEmpty(hdnSalesId.Value) ? 0 : Convert.ToInt32(hdnSalesId.Value),
                    InvoiceNumber = txtInvoiceNumber.Text,
                    InvoiceDate = Convert.ToDateTime(txtInvoiceDate.Text),
                    CustomerName = txtCustomerName.Text,
                    CustomerContact = txtCustomerContact.Text,
                    SubTotal = subTotal,
                    Discount = discount,
                    GrandTotal = grandTotal,
                    SalesDetails = new List<SalesDetail>()
                };

           
                foreach (var dto in detailDTOs)
                {
                    salesMaster.SalesDetails.Add(new SalesDetail
                    {
                        MedicineId = dto.MedicineId,
                        BatchNo = dto.BatchNo,
                        ExpiryDate = Convert.ToDateTime(dto.ExpiryDate),
                        Quantity = dto.Quantity,
                        UnitPrice = dto.UnitPrice,
                        LineTotal = dto.LineTotal
                    });
                }

                string result = salesBLL.SaveOrUpdateSales(salesMaster);
                if (!string.IsNullOrEmpty(result) && result.StartsWith("INV-"))
                {
                    ShowMessage("Invoice saved successfully! Invoice Number: " + result, false);
   
                    Response.AddHeader("REFRESH", "2;URL=Billing.aspx");
                }
                else
                {
                    ShowMessage("Error saving invoice: " + result, true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, true);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            hdnSalesId.Value = "";
            txtInvoiceNumber.Text = salesBLL.GetNextInvoiceNumber();
            txtInvoiceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtCustomerName.Text = "";
            txtCustomerContact.Text = "";
            txtDiscount.Text = "0";
            hdnDetailsJSON.Value = "";
            
         
            string script = "$('#tbodyDetails').empty(); rowIndex = 0; calculateTotals();";
            ClientScript.RegisterStartupScript(this.GetType(), "ClearForm", script, true);
        }

        private decimal CalculateSubTotal(List<SalesDetailDTO> details)
        {
            decimal subTotal = 0;
            foreach (var detail in details)
            {
                subTotal += detail.LineTotal;
            }
            return subTotal;
        }
    }
}
