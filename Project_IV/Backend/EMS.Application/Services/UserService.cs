using EMS.Application.DTOs;
using EMS.Application.Services.Interfaces;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;

namespace EMS.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtService _jwtService;

        public UserService(IUserRepository userRepo, JwtService jwtService)
        {
            _userRepo   = userRepo;
            _jwtService = jwtService;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var user = new UserInfo
            {
                EmailId  = dto.EmailId,
                UserName = dto.UserName,
                Role     = "Participant",
                Password = dto.Password   // plain text — UserRepository hashes it
            };
            return await _userRepo.RegisterAsync(user);
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.LoginAsync(dto.EmailId, dto.Password);
            if (user == null) return null;
            return _jwtService.GenerateToken(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.Select(u => new UserResponseDto
            {
                EmailId  = u.EmailId,
                UserName = u.UserName,
                Role     = u.Role
            });
        }
    }
}
