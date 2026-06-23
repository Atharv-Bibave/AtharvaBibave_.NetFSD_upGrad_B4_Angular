using EventManagementSystem.DataAccessLayer.Data;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagementSystem.DataAccessLayer.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EMSDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(EMSDbContext context, ILogger<CategoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDetails>> GetAllAsync()
            => await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();

        public async Task<CategoryDetails?> GetByIdAsync(Guid id)
            => await _context.Categories.FindAsync(id);

        public async Task<bool> AddAsync(CategoryDetails category)
        {
            try
            {
                category.Id = Guid.NewGuid();
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding category {Name}", category.CategoryName);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var cat = await _context.Categories.FindAsync(id);
                if (cat == null) return false;
                _context.Categories.Remove(cat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {Id}", id);
                return false;
            }
        }
    }
}
