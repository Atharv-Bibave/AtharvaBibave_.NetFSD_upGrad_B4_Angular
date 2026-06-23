using EventManagementSystem.DataAccessLayer.Data;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagementSystem.DataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EMSDbContext _context;
        private readonly ILogger<UserRepository> _logger;
        private readonly IPasswordService _passwordService;

        public UserRepository(
            EMSDbContext context,
            ILogger<UserRepository> logger,
            IPasswordService passwordService)
        {
            _context = context;
            _logger = logger;
            _passwordService = passwordService;
        }

        public async Task<bool> RegisterAsync(UserInfo user)
        {
            try
            {
                var exists = await _context.Users.AnyAsync(u => u.EmailId == user.EmailId);
                if (exists) return false;

                user.Role = "Participant";
                user.Password = _passwordService.Hash(user.Password); 

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Email}", user.EmailId);
                return false;
            }
        }

        public async Task<bool> RegisterEntraUserAsync(UserInfo user)
        {
            try
            {
                var exists = await _context.Users.AnyAsync(u => u.EmailId == user.EmailId);
                if (exists) return false;

                user.Role = "Participant";
                user.Password = "";

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering Entra user {Email}", user.EmailId);
                return false;
            }
        }

        public async Task<UserInfo?> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == email);

                if (user == null) return null;
                if (string.IsNullOrEmpty(user.Password)) return null;

                bool valid = _passwordService.Verify(password, user.Password); 
                return valid ? user : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", email);
                return null;
            }
        }

        public async Task<IEnumerable<UserInfo>> GetAllAsync()
        {
            return await _context.Users.OrderBy(u => u.UserName).ToListAsync();
        }

        public async Task<UserInfo?> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {Email}", email);
                return null;
            }
        }
    }
}