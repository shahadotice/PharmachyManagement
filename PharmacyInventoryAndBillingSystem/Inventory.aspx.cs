using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PharmacyInventoryAndBillingSystem.BLL;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem
{
    public partial class Inventory : System.Web.UI.Page
    {
        private MedicineBLL medicineBLL;
        private int currentMedicineId = 0;
        private bool isExportingPDF = false;

        protected override void Render(HtmlTextWriter writer)
        {
            if (!isExportingPDF)
            {
                base.Render(writer);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            medicineBLL = new MedicineBLL();

            if (!IsPostBack)
            {
                LoadMedicines();
            }
        }

        private void LoadMedicines()
        {
            List<Medicine> medicines = medicineBLL.GetAllMedicines();
            gvMedicines.DataSource = medicines;
            gvMedicines.DataBind();
        }


        private void ViewMedicine(int medicineId, bool isEdit)
        {
            Medicine medicine = medicineBLL.GetMedicineById(medicineId);
            if (medicine != null)
            {
                currentMedicineId = medicine.MedicineId;
                ViewState["CurrentMedicineId"] = medicineId;
                ViewState["IsEditMode"] = isEdit;
                
                txtModalMedicineName.Text = medicine.MedicineName;
                txtModalBatchNo.Text = medicine.BatchNo;
                txtModalBatchNo.ReadOnly = !isEdit;
                txtModalExpiryDate.Text = medicine.ExpiryDate.ToString("yyyy-MM-dd");
                txtModalExpiryDate.ReadOnly = !isEdit;
                txtModalQuantity.Text = medicine.Quantity.ToString();
                txtModalQuantity.ReadOnly = !isEdit;
                txtModalUnitPrice.Text = medicine.UnitPrice.ToString();
                txtModalUnitPrice.ReadOnly = !isEdit;
                txtModalSellsPrice.Text = medicine.SellsPrice.ToString();
                txtModalSellsPrice.ReadOnly = !isEdit;
                txtModalDescription.Text = medicine.Description ?? "";
                txtModalDescription.ReadOnly = !isEdit;

                string mode = isEdit ? "'edit'" : "'view'";
                string script = $"openModal({mode});";
                ClientScript.RegisterStartupScript(this.GetType(), "OpenModal", script, true);
            }
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            if (ViewState["CurrentMedicineId"] != null)
            {
                int medicineId = Convert.ToInt32(ViewState["CurrentMedicineId"]);
                
               
                string medicineName = txtModalMedicineName.Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(medicineName))
                {
                    ShowMessage("Please enter a medicine name.", true);
                    return;
                }
                if (medicineName.Length > 200)
                {
                    ShowMessage("Medicine name cannot exceed 200 characters.", true);
                    return;
                }
                
               
                string batchNo = txtModalBatchNo.Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(batchNo))
                {
                    ShowMessage("Please enter batch number.", true);
                    return;
                }
                if (batchNo.Length > 50)
                {
                    ShowMessage("Batch number cannot exceed 50 characters.", true);
                    return;
                }
                
              
                string expiryDateStr = txtModalExpiryDate.Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(expiryDateStr))
                {
                    ShowMessage("Please enter expiry date.", true);
                    return;
                }
                
                DateTime expiryDate;
                if (!DateTime.TryParse(expiryDateStr, out expiryDate))
                {
                    ShowMessage("Please enter a valid expiry date.", true);
                    return;
                }
                
               
                string quantityStr = txtModalQuantity.Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(quantityStr))
                {
                    ShowMessage("Please enter quantity.", true);
                    return;
                }
                
                int quantity = 0;
                if (!int.TryParse(quantityStr, out quantity))
                {
                    ShowMessage("Please enter a valid quantity (must be a whole number).", true);
                    return;
                }
                if (quantity < 0)
                {
                    ShowMessage("Quantity cannot be negative.", true);
                    return;
                }
                
                
                string unitPriceStr = txtModalUnitPrice.Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(unitPriceStr))
                {
                    ShowMessage("Please enter unit price.", true);
                    return;
                }
                
                decimal unitPrice = 0;
                if (!decimal.TryParse(unitPriceStr, out unitPrice))
                {
                    ShowMessage("Please enter a valid unit price.", true);
                    return;
                }
                if (unitPrice <= 0)
                {
                    ShowMessage("Unit price must be greater than 0.", true);
                    return;
                }
                
               
                string sellsPriceStr = txtModalSellsPrice.Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(sellsPriceStr))
                {
                    ShowMessage("Please enter sells price.", true);
                    return;
                }
                
                decimal sellsPrice = 0;
                if (!decimal.TryParse(sellsPriceStr, out sellsPrice))
                {
                    ShowMessage("Please enter a valid sells price.", true);
                    return;
                }
                if (sellsPrice <= 0)
                {
                    ShowMessage("Sells price must be greater than 0.", true);
                    return;
                }
                
                Medicine medicine = new Medicine
                {
                    MedicineId = medicineId,
                    MedicineName = medicineName,
                    BatchNo = batchNo,
                    ExpiryDate = expiryDate,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    SellsPrice = sellsPrice,
                    Description = txtModalDescription.Text?.Trim() ?? ""
                };

                if (medicineBLL.UpdateMedicine(medicine))
                {
                    ShowMessage("Medicine updated successfully!", false);
                    LoadMedicines();
                    closeModal();
                }
                else
                {
                    ShowMessage("Error updating medicine. Please try again.", true);
                }
            }
        }

        protected void btnAddMedicine_Click(object sender, EventArgs e)
        {
            
            ViewState["CurrentMedicineId"] = null;
            ViewState["IsEditMode"] = false;
            txtModalMedicineName.Text = "";
            txtModalBatchNo.Text = "";
            txtModalBatchNo.ReadOnly = false;
            txtModalExpiryDate.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            txtModalExpiryDate.ReadOnly = false;
            txtModalQuantity.Text = "1";
            txtModalQuantity.ReadOnly = false;
            txtModalUnitPrice.Text = "0";
            txtModalUnitPrice.ReadOnly = false;
            txtModalSellsPrice.Text = "0";
            txtModalSellsPrice.ReadOnly = false;
            txtModalDescription.Text = "";
            txtModalDescription.ReadOnly = false;
            
            string script = "openModal('add');";
            ClientScript.RegisterStartupScript(this.GetType(), "OpenAddModal", script, true);
        }

        protected void btnSaveAdd_Click(object sender, EventArgs e)
        {
            
            string medicineName = txtModalMedicineName.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(medicineName))
            {
                ShowMessage("Please enter a medicine name.", true);
                return;
            }
            if (medicineName.Length > 200)
            {
                ShowMessage("Medicine name cannot exceed 200 characters.", true);
                return;
            }
            
           
            string batchNo = txtModalBatchNo.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(batchNo))
            {
                ShowMessage("Please enter batch number.", true);
                return;
            }
            if (batchNo.Length > 50)
            {
                ShowMessage("Batch number cannot exceed 50 characters.", true);
                return;
            }
            
            
            string expiryDateStr = txtModalExpiryDate.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(expiryDateStr))
            {
                ShowMessage("Please enter expiry date.", true);
                return;
            }
            
            DateTime expiryDate;
            if (!DateTime.TryParse(expiryDateStr, out expiryDate))
            {
                ShowMessage("Please enter a valid expiry date.", true);
                return;
            }
            
           
            string quantityStr = txtModalQuantity.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(quantityStr))
            {
                ShowMessage("Please enter quantity.", true);
                return;
            }
            
            int quantity = 0;
            if (!int.TryParse(quantityStr, out quantity))
            {
                ShowMessage("Please enter a valid quantity (must be a whole number).", true);
                return;
            }
            if (quantity < 1)
            {
                ShowMessage("Quantity must be 1 or greater.", true);
                return;
            }
            
           
            string unitPriceStr = txtModalUnitPrice.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(unitPriceStr))
            {
                ShowMessage("Please enter unit price.", true);
                return;
            }
            
            decimal unitPrice = 0;
            if (!decimal.TryParse(unitPriceStr, out unitPrice))
            {
                ShowMessage("Please enter a valid unit price.", true);
                return;
            }
            if (unitPrice <= 0)
            {
                ShowMessage("Unit price must be greater than 0.", true);
                return;
            }
            
            
            string sellsPriceStr = txtModalSellsPrice.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(sellsPriceStr))
            {
                ShowMessage("Please enter sells price.", true);
                return;
            }
            
            decimal sellsPrice = 0;
            if (!decimal.TryParse(sellsPriceStr, out sellsPrice))
            {
                ShowMessage("Please enter a valid sells price.", true);
                return;
            }
            if (sellsPrice <= 0)
            {
                ShowMessage("Sells price must be greater than 0.", true);
                return;
            }
            
            Medicine medicine = new Medicine
            {
                MedicineName = medicineName,
                BatchNo = batchNo,
                ExpiryDate = expiryDate,
                Quantity = quantity,
                UnitPrice = unitPrice,
                SellsPrice = sellsPrice,
                Description = txtModalDescription.Text?.Trim() ?? ""
            };

            int newId = medicineBLL.InsertMedicine(medicine);
            if (newId > 0)
            {
                ShowMessage("Medicine added successfully!", false);
                LoadMedicines();
                closeModal();
            }
            else
            {
                ShowMessage("Error adding medicine. Please try again.", true);
            }
        }

        protected void gvMedicines_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int medicineId = Convert.ToInt32(e.CommandArgument);
                
                if (medicineBLL.DeleteMedicine(medicineId))
                {
                    ShowMessage("Medicine deleted successfully!", false);
                    LoadMedicines();
                }
                else
                {
                    ShowMessage("Error deleting medicine. Please try again.", true);
                }
            }
            else if (e.CommandName == "ViewRow")
            {
                int medicineId = Convert.ToInt32(e.CommandArgument);
                ViewMedicine(medicineId, false);
            }
            else if (e.CommandName == "EditRow")
            {
                int medicineId = Convert.ToInt32(e.CommandArgument);
                ViewMedicine(medicineId, true);
            }
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            List<Medicine> medicines = medicineBLL.GetAllMedicines();
            
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "";
            Response.AddHeader("content-disposition", "attachment;filename=Inventory_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");

            StringBuilder sb = new StringBuilder();
            sb.Append("<table border='1'>");
            sb.Append("<tr><th>ID</th><th>Medicine Name</th><th>Batch No</th><th>Expiry Date</th><th>Quantity</th><th>Unit Price</th><th>Description</th></tr>");

            foreach (Medicine med in medicines)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + med.MedicineId + "</td>");
                sb.Append("<td>" + med.MedicineName + "</td>");
                sb.Append("<td>" + med.BatchNo + "</td>");
                sb.Append("<td>" + med.ExpiryDate.ToString("dd/MM/yyyy") + "</td>");
                sb.Append("<td>" + med.Quantity + "</td>");
                sb.Append("<td>" + med.UnitPrice.ToString("C") + "</td>");
                sb.Append("<td>" + (med.Description ?? "") + "</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            Response.Write(sb.ToString());
            Response.End();
        }

       


        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            try
            {
               

                List<Medicine> inventoryList = medicineBLL.GetAllMedicines();

                
                Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                MemoryStream memoryStream = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);

               
                Paragraph title = new Paragraph("Pharmacy Inventory Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                
                Paragraph reportDate = new Paragraph("Generated on: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY));
                reportDate.Alignment = Element.ALIGN_RIGHT;
                reportDate.SpacingAfter = 10f;
                document.Add(reportDate);

                
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 2f, 2f, 1.5f, 1.5f });

               
                string[] headers = { "Medicine Name",  "Batch No",
                    "Expiry Date", "Total Qty",  "Unit Price" };

                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = new BaseColor(70, 130, 180); // Steel blue
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    table.AddCell(cell);
                }

                
                foreach (var item in inventoryList)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.MedicineName ?? "", cellFont)) { Padding = 5f });
                
                  
                    table.AddCell(new PdfPCell(new Phrase(item.BatchNo ?? "", cellFont)) { Padding = 5f });
                    table.AddCell(new PdfPCell(new Phrase(item.ExpiryDate.ToString("dd/MM/yyyy"), cellFont)) { Padding = 5f });
                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 5f });
                   
                    table.AddCell(new PdfPCell(new Phrase(item.UnitPrice.ToString("F2"), cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 5f });
                }

                document.Add(table);

               
                Paragraph summary = new Paragraph();
                summary.SpacingBefore = 20f;
                summary.Add(new Chunk("Total Records: ", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK)));
                summary.Add(new Chunk(inventoryList.Count.ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK)));
                document.Add(summary);

                document.Close();

               
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=Inventory_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf");
                Response.BinaryWrite(memoryStream.ToArray());
                Response.End();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PDF Export Error: " + ex.ToString());
            }
        }

       

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isError ? "alert alert-danger" : "alert alert-success";
            lblMessage.Visible = true;
        }

        private void closeModal()
        {
            string script = "closeModal();";
            ClientScript.RegisterStartupScript(this.GetType(), "CloseModal", script, true);
        }
    }
}
