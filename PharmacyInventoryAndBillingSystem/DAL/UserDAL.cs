using System;
using System.Data;
using System.Data.SqlClient;
using PharmacyInventoryAndBillingSystem.DAL.Interfaces;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.DAL
{
    public class UserDAL : IUserDAL
    {
        public User ValidateUser(string username, string password)
        {
            User user = null;
            string query = "SELECT UserId, Username, Password, FullName, IsActive, CreatedDate FROM Users WHERE Username = @Username AND Password = @Password AND IsActive = 1";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                user = new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    Username = row["Username"].ToString(),
                    Password = row["Password"].ToString(),
                    FullName = row["FullName"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                };
            }
            
            return user;
        }
    }
}
