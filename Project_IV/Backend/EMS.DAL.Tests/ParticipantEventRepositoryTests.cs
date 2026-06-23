using EventManagementSystem.DataAccessLayer.Data;
using EventManagementSystem.DataAccessLayer.Models;
using EventManagementSystem.DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EMS.DAL.Tests
{
    [TestFixture]
    public class ParticipantEventRepositoryTests : IDisposable
    {
        private EMSDbContext _context = null!;
        private ParticipantEventRepository _repository = null!;
        private Guid _eventId;
        private bool _disposed;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EMSDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new EMSDbContext(options);
            _repository = new ParticipantEventRepository(
                _context,
                new Mock<ILogger<ParticipantEventRepository>>().Object);

            // Seed required FK data
            _eventId = Guid.NewGuid();
            _context.Events.Add(new EventDetails
            {
                EventId       = _eventId,
                EventName     = "Sample Event",
                EventCategory = "Workshop",
                EventDate     = DateTime.Today.AddDays(5),
                Status        = "Active"
            });
            _context.Users.Add(new UserInfo
            {
                EmailId  = "participant@example.com",
                UserName = "Test Participant",
                Role     = "Participant",
                Password = "hashed"
            });
            _context.SaveChanges();
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
        public async Task RegisterAsyncShouldInsertRegistration()
        {
            // Arrange
            var entry = new ParticipantEventDetails
            {
                ParticipantEmailId = "participant@example.com",
                EventId            = _eventId,
                IsAttended         = false
            };

            // Act
            var result = await _repository.RegisterAsync(entry);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_context.ParticipantEvents.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task IsAlreadyRegisteredAsyncShouldReturnTrueIfExists()
        {
            // Arrange
            _context.ParticipantEvents.Add(new ParticipantEventDetails
            {
                Id                 = Guid.NewGuid(),
                ParticipantEmailId = "participant@example.com",
                EventId            = _eventId,
                IsAttended         = false
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsAlreadyRegisteredAsync("participant@example.com", _eventId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsAlreadyRegisteredAsyncShouldReturnFalseIfNotExists()
        {
            // Arrange — empty participant events

            // Act
            var result = await _repository.IsAlreadyRegisteredAsync("nobody@example.com", _eventId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateAttendanceAsyncShouldMarkAttendance()
        {
            // Arrange
            var regId = Guid.NewGuid();
            _context.ParticipantEvents.Add(new ParticipantEventDetails
            {
                Id                 = regId,
                ParticipantEmailId = "participant@example.com",
                EventId            = _eventId,
                IsAttended         = false
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.UpdateAttendanceAsync(regId, true);

            // Assert
            Assert.That(result, Is.True);
            var updated = await _context.ParticipantEvents.FindAsync(regId);
            Assert.That(updated!.IsAttended, Is.True);
        }

        [Test]
        public async Task GetByParticipantAsyncShouldReturnRegistrationsForEmail()
        {
            // Arrange
            _context.ParticipantEvents.Add(new ParticipantEventDetails
            {
                Id                 = Guid.NewGuid(),
                ParticipantEmailId = "participant@example.com",
                EventId            = _eventId,
                IsAttended         = false
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByParticipantAsync("participant@example.com");

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
        }
    }
}
