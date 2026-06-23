using EventManagementSystem.DataAccessLayer.Models;

namespace EventManagementSystem.DataAccessLayer.Interfaces
{
    public interface IParticipantEventRepository
    {
        Task<bool> RegisterAsync(ParticipantEventDetails entry);
        Task<bool> IsAlreadyRegisteredAsync(string email, Guid eventId);
        Task<IEnumerable<ParticipantEventDetails>> GetByParticipantAsync(string email);
        Task<bool> UpdateAttendanceAsync(Guid id, bool isAttended);
        Task<IEnumerable<ParticipantEventDetails>> GetAllAsync();
        Task<ParticipantEventDetails?> GetByIdAsync(Guid id);
        Task<IEnumerable<ParticipantEventDetails>> GetByEventIdAsync(Guid eventId);
    }
}
