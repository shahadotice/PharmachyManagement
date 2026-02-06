using PharmacyInventoryAndBillingSystem.Models;

namespace PharmacyInventoryAndBillingSystem.DAL.Interfaces
{
    public interface IUserDAL
    {
        User ValidateUser(string username, string password);
    }
}
