using PharmacyInventoryAndBillingSystem.BLL.Interfaces;
using PharmacyInventoryAndBillingSystem.DAL;
using PharmacyInventoryAndBillingSystem.DAL.Interfaces;
using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.BLL
{
    public class UserBLL : IUserBLL
    {
        private readonly IUserDAL userDAL;

        public UserBLL()
        {
            this.userDAL = new DAL.UserDAL();
        }

        public UserBLL(IUserDAL userDAL)
        {
            this.userDAL = userDAL;
        }

        public User ValidateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            return userDAL.ValidateUser(username, password);
        }
    }
}
