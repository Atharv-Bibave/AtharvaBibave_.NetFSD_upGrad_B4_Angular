using EventManagementSystem.DataAccessLayer.Models;

namespace EventManagementSystem.DataAccessLayer.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventDetails>> GetAllAsync();
        Task<IEnumerable<EventDetails>> GetActiveEventsAsync();
        Task<EventDetails?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(EventDetails eventDetails);
        Task<bool> UpdateAsync(EventDetails eventDetails);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<EventDetails>> GetByCategoryAsync(string category);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}