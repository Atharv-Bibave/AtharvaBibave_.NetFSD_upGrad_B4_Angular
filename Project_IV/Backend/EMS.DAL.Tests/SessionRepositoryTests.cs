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
    public class SessionRepositoryTests : IDisposable
    {
        private EMSDbContext _context = null!;
        private SessionRepository _repository = null!;
        private Guid _eventId;
        private bool _disposed;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EMSDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new EMSDbContext(options);
            _repository = new SessionRepository(_context, new Mock<ILogger<SessionRepository>>().Object);

            // Seed a parent event required by FK
            _eventId = Guid.NewGuid();
            _context.Events.Add(new EventDetails
            {
                EventId       = _eventId,
                EventName     = "Parent Event",
                EventCategory = "Workshop",
                EventDate     = DateTime.Today.AddDays(5),
                Status        = "Active"
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
        public async Task GetByEventIdAsyncShouldReturnSessionsForEvent()
        {
            // Arrange
            _context.Sessions.AddRange(
                new SessionInfo
                {
                    SessionId = Guid.NewGuid(), EventId = _eventId,
                    SessionTitle = "Keynote", SessionStart = DateTime.Today.AddDays(5).AddHours(9),
                    SessionEnd = DateTime.Today.AddDays(5).AddHours(10)
                },
                new SessionInfo
                {
                    SessionId = Guid.NewGuid(), EventId = _eventId,
                    SessionTitle = "Panel", SessionStart = DateTime.Today.AddDays(5).AddHours(11),
                    SessionEnd = DateTime.Today.AddDays(5).AddHours(12)
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEventIdAsync(_eventId);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task AddAsyncShouldInsertSessionSuccessfully()
        {
            // Arrange
            var session = new SessionInfo
            {
                EventId      = _eventId,
                SessionTitle = "Opening Talk",
                SessionStart = DateTime.Today.AddDays(5).AddHours(9),
                SessionEnd   = DateTime.Today.AddDays(5).AddHours(10)
            };

            // Act
            var result = await _repository.AddAsync(session);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_context.Sessions.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteAsyncShouldRemoveSession()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            _context.Sessions.Add(new SessionInfo
            {
                SessionId    = sessionId,
                EventId      = _eventId,
                SessionTitle = "To Delete",
                SessionStart = DateTime.Today.AddDays(5).AddHours(9),
                SessionEnd   = DateTime.Today.AddDays(5).AddHours(10)
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(sessionId);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_context.Sessions.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetByIdAsyncShouldReturnCorrectSession()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            _context.Sessions.Add(new SessionInfo
            {
                SessionId    = sessionId,
                EventId      = _eventId,
                SessionTitle = "Specific Session",
                SessionStart = DateTime.Today.AddDays(5).AddHours(14),
                SessionEnd   = DateTime.Today.AddDays(5).AddHours(15)
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(sessionId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.SessionTitle, Is.EqualTo("Specific Session"));
        }
    }
}
