using EventManagementSystem.DataAccessLayer.Data;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagementSystem.DataAccessLayer.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly EMSDbContext _context;
        private readonly ILogger<SessionRepository> _logger;

        public SessionRepository(EMSDbContext context, ILogger<SessionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<SessionInfo>> GetAllAsync()
        {
            return await _context.Sessions
                .Include(s => s.Event)
                .Include(s => s.Speaker)
                .ToListAsync();
        }

        public async Task<SessionInfo?> GetByIdAsync(Guid id)
        {
            return await _context.Sessions
                .Include(s => s.Event)
                .Include(s => s.Speaker)
                .FirstOrDefaultAsync(s => s.SessionId == id);
        }

        public async Task<IEnumerable<SessionInfo>> GetByEventIdAsync(Guid eventId)
        {
            return await _context.Sessions
                .Where(s => s.EventId == eventId)
                .Include(s => s.Speaker)
                .Include(s => s.Event)
                .OrderBy(s => s.SessionStart)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(SessionInfo session)
        {
            try
            {
                session.SessionId = Guid.NewGuid();
                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding session '{SessionTitle}' for EventId {EventId}. Inner: {Inner}",
                    session.SessionTitle, session.EventId, ex.InnerException?.Message ?? ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(SessionInfo session)
        {
            try
            {
                _context.Sessions.Update(session);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating session {SessionId}", session.SessionId);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var session = await _context.Sessions.FindAsync(id);
                if (session == null) return false;
                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session {SessionId}", id);
                return false;
            }
        }
    }
}
