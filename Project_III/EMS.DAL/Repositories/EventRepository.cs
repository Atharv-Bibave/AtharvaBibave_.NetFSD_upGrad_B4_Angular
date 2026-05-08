using EventManagementSystem.DataAccessLayer.Data;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagementSystem.DataAccessLayer.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EMSDbContext _context;
        private readonly ILogger<EventRepository> _logger;

        public EventRepository(EMSDbContext context, ILogger<EventRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<EventDetails>> GetAllAsync()
        {
            return await _context.Events
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventDetails>> GetActiveEventsAsync()
        {
            return await _context.Events
                .Where(e => e.Status == "Active")
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<EventDetails?> GetByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Sessions)
                    .ThenInclude(s => s.Speaker)
                .FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task<bool> AddAsync(EventDetails eventDetails)
        {
            try
            {
                eventDetails.EventId = Guid.NewGuid();
                await _context.Events.AddAsync(eventDetails);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding event {EventName}", eventDetails.EventName);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(EventDetails eventDetails)
        {
            try
            {
                _context.Events.Update(eventDetails);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {EventId}", eventDetails.EventId);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var ev = await _context.Events.FindAsync(id);
                if (ev == null) return false;

                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event {EventId}", id);
                return false;
            }
        }

        public async Task<IEnumerable<EventDetails>> GetByCategoryAsync(string category)
        {
            return await _context.Events
                .Where(e => e.EventCategory == category && e.Status == "Active")
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _context.Events
                .Where(e => e.Status == "Active")
                .Select(e => e.EventCategory)
                .Distinct()
                .ToListAsync();
        }
    }
}