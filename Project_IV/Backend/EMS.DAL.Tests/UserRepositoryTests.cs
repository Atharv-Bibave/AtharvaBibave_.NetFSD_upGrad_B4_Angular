using EventManagementSystem.DataAccessLayer.Data;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using EventManagementSystem.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EMS.DAL.Tests
{
    [TestFixture]
    public class UserRepositoryTests : IDisposable
    {
        private EMSDbContext _context = null!;
        private UserRepository _repository = null!;
        private bool _disposed;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EMSDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new EMSDbContext(options);

            var loggerMock = new Mock<ILogger<UserRepository>>();

            var passwordMock = new Mock<IPasswordService>();
            passwordMock.Setup(p => p.Hash(It.IsAny<string>()))
                        .Returns<string>(p => "$2a$fake$" + p);
            passwordMock.Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns<string, string>((plain, hash) => hash == "$2a$fake$" + plain);

            _repository = new UserRepository(_context, loggerMock.Object, passwordMock.Object);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) _context?.Dispose();
                _disposed = true;
            }
        }

        [Test]
        public async Task RegisterAsyncShouldInsertNewUser()
        {
            // Arrange
            var user = new UserInfo
            {
                EmailId = "test@example.com",
                UserName = "Test User",
                Role = "Participant",
                Password = "P@ssword1"
            };

            // Act
            var result = await _repository.RegisterAsync(user);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_context.Users.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task RegisterAsyncShouldReturnFalseForDuplicateEmail()
        {
            // Arrange — pre-insert same email using mock hash format
            _context.Users.Add(new UserInfo
            {
                EmailId = "dup@example.com",
                UserName = "Existing",
                Role = "Participant",
                Password = "$2a$fake$P@ss1"
            });
            await _context.SaveChangesAsync();

            var user = new UserInfo
            {
                EmailId = "dup@example.com",
                UserName = "Duplicate",
                Role = "Participant",
                Password = "P@ssword1"
            };

            // Act
            var result = await _repository.RegisterAsync(user);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_context.Users.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task LoginAsyncShouldReturnUserOnValidCredentials()
        {
            // Arrange — seed with mock hash format so Verify mock matches
            var plainPassword = "Secure@99";
            _context.Users.Add(new UserInfo
            {
                EmailId = "login@example.com",
                UserName = "Login User",
                Role = "Participant",
                Password = "$2a$fake$" + plainPassword
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.LoginAsync("login@example.com", plainPassword);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.EmailId, Is.EqualTo("login@example.com"));
        }

        [Test]
        public async Task LoginAsyncShouldReturnNullOnWrongPassword()
        {
            // Arrange — seed with mock hash format
            _context.Users.Add(new UserInfo
            {
                EmailId = "wrong@example.com",
                UserName = "Wrong Pass",
                Role = "Participant",
                Password = "$2a$fake$Correct@1"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.LoginAsync("wrong@example.com", "BadPass@1");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllAsyncShouldReturnAllUsers()
        {
            // Arrange
            _context.Users.AddRange(
                new UserInfo { EmailId = "a@x.com", UserName = "Alpha", Role = "Participant", Password = "h1" },
                new UserInfo { EmailId = "b@x.com", UserName = "Beta", Role = "Admin", Password = "h2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}