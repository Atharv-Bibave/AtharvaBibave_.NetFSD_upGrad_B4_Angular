using EventManagementSystem.DataAccessLayer.Data;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagementSystem.DataAccessLayer.Repositories
{
    public class SpeakerRepository : ISpeakerRepository
    {
        private readonly EMSDbContext _context;
        private readonly ILogger<SpeakerRepository> _logger;

        public SpeakerRepository(EMSDbContext context, ILogger<SpeakerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<SpeakersDetails>> GetAllAsync()
        {
            return await _context.Speakers.ToListAsync();
        }

        public async Task<SpeakersDetails?> GetByIdAsync(Guid id)
        {
            return await _context.Speakers.FindAsync(id);
        }

        public async Task<bool> AddAsync(SpeakersDetails speaker)
        {
            try
            {
                speaker.SpeakerId = Guid.NewGuid();
                await _context.Speakers.AddAsync(speaker);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding speaker {SpeakerName}", speaker.SpeakerName);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(SpeakersDetails speaker)
        {
            try
            {
                _context.Speakers.Update(speaker);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating speaker {SpeakerId}", speaker.SpeakerId);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var sp = await _context.Speakers.FindAsync(id);
                if (sp == null) return false;

                _context.Speakers.Remove(sp);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting speaker {SpeakerId}", id);
                return false;
            }
        }

        public async Task<int> GetAssignedSessionCountAsync(Guid speakerId)
        {
            return await _context.Sessions
                .CountAsync(s => s.SpeakerId == speakerId);
        }
    }
}