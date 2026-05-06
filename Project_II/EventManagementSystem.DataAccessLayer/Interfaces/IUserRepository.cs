using EventManagementSystem.DataAccessLayer.Models;

namespace EventManagementSystem.DataAccessLayer.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> RegisterAsync(UserInfo user);
        Task<UserInfo?> LoginAsync(string email, string password);
        Task<UserInfo?> GetByEmailAsync(string email);
        Task<bool> RegisterEntraUserAsync(UserInfo user);
    }
}
