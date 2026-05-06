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

        public UserRepository(EMSDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger  = logger;
        }

        public async Task<bool> RegisterAsync(UserInfo user)
        {
            try
            {
                var exists = await _context.Users.AnyAsync(u => u.EmailId == user.EmailId);
                if (exists) return false;

                user.Role     = "Participant";
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

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

        // Registers a user who signed in via Microsoft Entra ID.
        // No password is stored — an empty string sentinel is saved.
        public async Task<bool> RegisterEntraUserAsync(UserInfo user)
        {
            try
            {
                var exists = await _context.Users.AnyAsync(u => u.EmailId == user.EmailId);
                if (exists) return false;

                user.Role     = "Participant";
                user.Password = "";   // Entra users have no local password

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

        // Standard login using BCrypt password verification.
        public async Task<UserInfo?> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == email);

                if (user == null) return null;

                // Entra-only users have an empty password — block local login for them
                if (string.IsNullOrEmpty(user.Password)) return null;

                bool valid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                return valid ? user : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", email);
                return null;
            }
        }

        // Retrieves a user by email without any password check (used for Entra ID callback).
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
