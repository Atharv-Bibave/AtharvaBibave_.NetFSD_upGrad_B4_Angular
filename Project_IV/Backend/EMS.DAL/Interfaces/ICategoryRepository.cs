using EventManagementSystem.DataAccessLayer.Models;

namespace EventManagementSystem.DataAccessLayer.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDetails>> GetAllAsync();
        Task<CategoryDetails?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(CategoryDetails category);
        Task<bool> DeleteAsync(Guid id);
    }
}
