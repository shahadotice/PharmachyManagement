using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using PharmacyInventoryAndBillingSystem.DAL.Interfaces;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.DAL
{
    public class SalesDAL : ISalesDAL
    {
        public string SaveOrUpdateSales(SalesMaster salesMaster)
        {
            string xmlData = ConvertToXML(salesMaster);
            
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_SaveOrUpdateSales", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.Add(new SqlParameter("@SalesId", salesMaster.SalesId > 0 ? (object)salesMaster.SalesId : DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@InvoiceNumber", salesMaster.InvoiceNumber));
                    cmd.Parameters.Add(new SqlParameter("@InvoiceDate", salesMaster.InvoiceDate));
                    cmd.Parameters.Add(new SqlParameter("@CustomerName", salesMaster.CustomerName));
                    cmd.Parameters.Add(new SqlParameter("@CustomerContact", salesMaster.CustomerContact));
                    cmd.Parameters.Add(new SqlParameter("@SubTotal", salesMaster.SubTotal));
                    cmd.Parameters.Add(new SqlParameter("@Discount", salesMaster.Discount));
                    cmd.Parameters.Add(new SqlParameter("@GrandTotal", salesMaster.GrandTotal));
                    cmd.Parameters.Add(new SqlParameter("@SalesDetailsXML", xmlData));
                    
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? string.Empty;
                }
            }
        }

        private string ConvertToXML(SalesMaster salesMaster)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("SalesDetails");
            xmlDoc.AppendChild(root);

            if (salesMaster.SalesDetails != null)
            {
                foreach (var detail in salesMaster.SalesDetails)
                {
                    XmlElement detailElement = xmlDoc.CreateElement("SalesDetail");
                    
                    XmlElement medicineId = xmlDoc.CreateElement("MedicineId");
                    medicineId.InnerText = detail.MedicineId.ToString();
                    detailElement.AppendChild(medicineId);

                    XmlElement batchNo = xmlDoc.CreateElement("BatchNo");
                    batchNo.InnerText = detail.BatchNo;
                    detailElement.AppendChild(batchNo);

                    XmlElement expiryDate = xmlDoc.CreateElement("ExpiryDate");
                    expiryDate.InnerText = detail.ExpiryDate.ToString("yyyy-MM-dd");
                    detailElement.AppendChild(expiryDate);

                    XmlElement quantity = xmlDoc.CreateElement("Quantity");
                    quantity.InnerText = detail.Quantity.ToString();
                    detailElement.AppendChild(quantity);

                    XmlElement unitPrice = xmlDoc.CreateElement("UnitPrice");
                    unitPrice.InnerText = detail.UnitPrice.ToString();
                    detailElement.AppendChild(unitPrice);

                    XmlElement lineTotal = xmlDoc.CreateElement("LineTotal");
                    lineTotal.InnerText = detail.LineTotal.ToString();
                    detailElement.AppendChild(lineTotal);

                    root.AppendChild(detailElement);
                }
            }

            return xmlDoc.OuterXml;
        }

        public string GetNextInvoiceNumber()
        {
            string query = "SELECT 'INV-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-' + RIGHT('0000' + CAST(ISNULL(MAX(CAST(RIGHT(InvoiceNumber, 4) AS INT)), 0) + 1 AS VARCHAR), 4) FROM SalesMaster WHERE CAST(InvoiceDate AS DATE) = CAST(GETDATE() AS DATE)";
            
            object result = DatabaseHelper.ExecuteScalar(query);
            
            if (result == null || result == DBNull.Value)
            {
                return "INV-" + DateTime.Now.ToString("yyyyMMdd") + "-0001";
            }
            
            return result.ToString();
        }

        public List<SalesMaster> GetAllInvoices()
        {
            List<SalesMaster> invoices = new List<SalesMaster>();
            string query = "SELECT SalesId, InvoiceNumber, InvoiceDate, CustomerName, CustomerContact, SubTotal, Discount, GrandTotal, CreatedDate FROM SalesMaster ORDER BY CreatedDate DESC";
            
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            
            foreach (DataRow row in dt.Rows)
            {
                invoices.Add(new SalesMaster
                {
                    SalesId = Convert.ToInt32(row["SalesId"]),
                    InvoiceNumber = row["InvoiceNumber"].ToString(),
                    InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                    CustomerName = row["CustomerName"].ToString(),
                    CustomerContact = row["CustomerContact"]?.ToString() ?? "",
                    SubTotal = Convert.ToDecimal(row["SubTotal"]),
                    Discount = Convert.ToDecimal(row["Discount"]),
                    GrandTotal = Convert.ToDecimal(row["GrandTotal"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }
            
            return invoices;
        }

        public SalesMaster GetInvoiceById(int salesId)
        {
            SalesMaster invoice = null;
            string query = @"SELECT SalesId, InvoiceNumber, InvoiceDate, CustomerName, CustomerContact, SubTotal, Discount, GrandTotal, CreatedDate 
                           FROM SalesMaster WHERE SalesId = @SalesId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@SalesId", salesId)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                invoice = new SalesMaster
                {
                    SalesId = Convert.ToInt32(row["SalesId"]),
                    InvoiceNumber = row["InvoiceNumber"].ToString(),
                    InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                    CustomerName = row["CustomerName"].ToString(),
                    CustomerContact = row["CustomerContact"]?.ToString() ?? "",
                    SubTotal = Convert.ToDecimal(row["SubTotal"]),
                    Discount = Convert.ToDecimal(row["Discount"]),
                    GrandTotal = Convert.ToDecimal(row["GrandTotal"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    SalesDetails = new List<SalesDetail>()
                };

          
                string detailQuery = @"SELECT SD.SalesDetailId, SD.MedicineId, M.MedicineName, SD.BatchNo, SD.ExpiryDate, 
                                     SD.Quantity, SD.UnitPrice, SD.LineTotal 
                                     FROM SalesDetail SD 
                                     INNER JOIN Medicines M ON SD.MedicineId = M.MedicineId 
                                     WHERE SD.SalesId = @SalesId";
                
          
                SqlParameter[] detailParameters = new SqlParameter[]
                {
                    new SqlParameter("@SalesId", salesId)
                };
                
                DataTable detailDt = DatabaseHelper.ExecuteQuery(detailQuery, detailParameters);
                
                foreach (DataRow detailRow in detailDt.Rows)
                {
                    invoice.SalesDetails.Add(new SalesDetail
                    {
                        SalesDetailId = Convert.ToInt32(detailRow["SalesDetailId"]),
                        MedicineId = Convert.ToInt32(detailRow["MedicineId"]),
                        MedicineName = detailRow["MedicineName"].ToString(),
                        BatchNo = detailRow["BatchNo"].ToString(),
                        ExpiryDate = Convert.ToDateTime(detailRow["ExpiryDate"]),
                        Quantity = Convert.ToInt32(detailRow["Quantity"]),
                        UnitPrice = Convert.ToDecimal(detailRow["UnitPrice"]),
                        LineTotal = Convert.ToDecimal(detailRow["LineTotal"])
                    });
                }
            }
            
            return invoice;
        }

        public bool DeleteInvoice(int salesId)
        {
            string query = "DELETE FROM SalesMaster WHERE SalesId = @SalesId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@SalesId", salesId)
            };

            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
