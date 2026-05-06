using EventManagementSystem.DataAccessLayer.Models;

namespace EventManagementSystem.DataAccessLayer.Interfaces
{
    public interface ISpeakerRepository
    {
        Task<IEnumerable<SpeakersDetails>> GetAllAsync();
        Task<SpeakersDetails?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(SpeakersDetails speaker);
        Task<bool> UpdateAsync(SpeakersDetails speaker);
        Task<bool> DeleteAsync(Guid id);
        Task<int> GetAssignedSessionCountAsync(Guid speakerId);
    }
}