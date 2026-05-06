using EventManagementSystem.DataAccessLayer.Data;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagementSystem.DataAccessLayer.Repositories
{
    public class ParticipantEventRepository : IParticipantEventRepository
    {
        private readonly EMSDbContext _context;
        private readonly ILogger<ParticipantEventRepository> _logger;

        public ParticipantEventRepository(EMSDbContext context, ILogger<ParticipantEventRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> RegisterAsync(ParticipantEventDetails entry)
        {
            try
            {
                entry.Id = Guid.NewGuid();
                await _context.ParticipantEvents.AddAsync(entry);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering participant {Email} for event {EventId}",
                    entry.ParticipantEmailId, entry.EventId);
                return false;
            }
        }

        public async Task<bool> IsAlreadyRegisteredAsync(string email, Guid eventId)
        {
            return await _context.ParticipantEvents
                .AnyAsync(p => p.ParticipantEmailId == email && p.EventId == eventId);
        }

        public async Task<IEnumerable<ParticipantEventDetails>> GetByParticipantAsync(string email)
        {
            return await _context.ParticipantEvents
                .Where(p => p.ParticipantEmailId == email)
                .Include(p => p.Event)
                .ToListAsync();
        }

        public async Task<IEnumerable<ParticipantEventDetails>> GetAllAsync()
        {
            return await _context.ParticipantEvents
                .Include(p => p.Event)
                .Include(p => p.User)
                .OrderBy(p => p.Event!.EventName)
                .ThenBy(p => p.ParticipantEmailId)
                .ToListAsync();
        }

        public async Task<bool> UpdateAttendanceAsync(Guid id, bool isAttended)
        {
            try
            {
                var entry = await _context.ParticipantEvents.FindAsync(id);
                if (entry == null) return false;
                entry.IsAttended = isAttended;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating attendance for registration {Id}", id);
                return false;
            }
        }

        public async Task<ParticipantEventDetails?> GetByIdAsync(Guid id)
        {
            return await _context.ParticipantEvents
                .Include(p => p.Event)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
