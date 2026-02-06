using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PharmacyInventoryAndBillingSystem.DAL.Interfaces;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.DAL
{
    public class DashboardDAL : IDashboardDAL
    {
        public DashboardStatistics GetDashboardStatistics()
        {
            DashboardStatistics stats = new DashboardStatistics();
            
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetDashboardStatistics", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            
                          
                            stats.TotalSalesAmount = Convert.ToDecimal(row["TotalSalesAmount"]);
                            stats.TotalStokesAmount = Convert.ToDecimal(row["TotalStokesAmount"]);
                            
                         
                            
                        }
                    }
                }
            }
            
            return stats;
        }

        public List<StockDetailDTO> GetStockDetails()
        {
            List<StockDetailDTO> stockDetails = new List<StockDetailDTO>();
            
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT MedicineName, BatchNo, UnitPrice * Quantity as UnitPrice 
                                FROM Medicines 
                                ORDER BY MedicineName, BatchNo";
                
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stockDetails.Add(new StockDetailDTO
                            {
                                MedicineName = reader["MedicineName"].ToString(),
                                BatchNo = reader["BatchNo"].ToString(),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"])
                            });
                        }
                    }
                }
            }
            
            return stockDetails;
        }

        public List<SalesDetailModalDTO> GetSalesDetails()
        {
            List<SalesDetailModalDTO> salesDetails = new List<SalesDetailModalDTO>();
            
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT InvoiceNumber, InvoiceDate, GrandTotal 
                                FROM SalesMaster 
                                ORDER BY InvoiceDate DESC, InvoiceNumber";
                
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            salesDetails.Add(new SalesDetailModalDTO
                            {
                                InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]).ToString("dd MMM yyyy"),
                                GrandTotal = Convert.ToDecimal(reader["GrandTotal"])
                            });
                        }
                    }
                }
            }
            
            return salesDetails;
        }
    }
}
