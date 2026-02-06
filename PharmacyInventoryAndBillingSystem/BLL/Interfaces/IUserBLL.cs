using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.BLL.Interfaces
{
    public interface IUserBLL
    {
        User ValidateUser(string username, string password);
    }
}
