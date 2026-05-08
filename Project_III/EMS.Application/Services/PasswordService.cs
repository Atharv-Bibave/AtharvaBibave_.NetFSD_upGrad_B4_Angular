using EventManagementSystem.DataAccessLayer.Interfaces;

namespace EMS.Application.Services
{
    public class PasswordService : IPasswordService
    {
        public string Hash(string plainText)
            => BCrypt.Net.BCrypt.HashPassword(plainText);

        public bool Verify(string plainText, string hash)
            => BCrypt.Net.BCrypt.Verify(plainText, hash);
    }
}