using EventManagementSystem.DataAccessLayer.Models;

namespace EventManagementSystem.DataAccessLayer.Interfaces
{
    public interface ISessionRepository
    {
        Task<IEnumerable<SessionInfo>> GetAllAsync();
        Task<SessionInfo?> GetByIdAsync(Guid id);
        Task<IEnumerable<SessionInfo>> GetByEventIdAsync(Guid eventId);
        Task<bool> AddAsync(SessionInfo session);
        Task<bool> UpdateAsync(SessionInfo session);
        Task<bool> DeleteAsync(Guid id);

    }
}
