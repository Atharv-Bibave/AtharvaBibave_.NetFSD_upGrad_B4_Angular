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
    // ── Test Case 1/2/3 using Moq-mocked IEventRepository (spec requirement) ──

    [TestFixture]
    public class EventRepositoryMoqTests
    {
        private Mock<IEventRepository> _mockRepo = null!;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IEventRepository>();
        }

        // Test Case 1 — Get all events → should return list
        [Test]
        public async Task GetAllAsync_ShouldReturnList()
        {
            // Arrange
            var events = new List<EventDetails>
            {
                new() { EventId = Guid.NewGuid(), EventName = "Tech Summit",
                        EventCategory = "Tech & Innovation",
                        EventDate = DateTime.Today.AddDays(10), Status = "Active" },
                new() { EventId = Guid.NewGuid(), EventName = "Workshop 101",
                        EventCategory = "Workshop",
                        EventDate = DateTime.Today.AddDays(5), Status = "Active" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

            // Act
            var result = await _mockRepo.Object.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        // Test Case 2 — Add event → should insert successfully
        [Test]
        public async Task AddAsync_ShouldInsertSuccessfully()
        {
            // Arrange
            var newEvent = new EventDetails
            {
                EventName     = "Industrial Expo",
                EventCategory = "Industrial Event",
                EventDate     = DateTime.Today.AddDays(20),
                Status        = "Active"
            };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<EventDetails>())).ReturnsAsync(true);

            // Act
            var result = await _mockRepo.Object.AddAsync(newEvent);

            // Assert
            Assert.That(result, Is.True);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<EventDetails>()), Times.Once);
        }

        // Test Case 3 — Delete event → should remove record
        [Test]
        public async Task DeleteAsync_ShouldRemoveRecord()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _mockRepo.Setup(r => r.DeleteAsync(eventId)).ReturnsAsync(true);

            // Act
            var result = await _mockRepo.Object.DeleteAsync(eventId);

            // Assert
            Assert.That(result, Is.True);
            _mockRepo.Verify(r => r.DeleteAsync(eventId), Times.Once);
        }
    }

    // ── Integration-style tests using InMemory DbContext (concrete repository) ──

    [TestFixture]
    public class EventRepositoryTests : IDisposable
    {
        private EMSDbContext _context = null!;
        private EventRepository _repository = null!;
        private bool _disposed;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EMSDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new EMSDbContext(options);
            var loggerMock = new Mock<ILogger<EventRepository>>();
            _repository = new EventRepository(_context, loggerMock.Object);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        public void Dispose()
        {
            Dispose(disposing: true);
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
        public async Task GetAllAsync_ShouldReturnList_Integration()
        {
            _context.Events.Add(new EventDetails
            {
                EventId = Guid.NewGuid(), EventName = "Tech Summit",
                EventCategory = "Tech & Innovation", EventDate = DateTime.Today.AddDays(10), Status = "Active"
            });
            _context.Events.Add(new EventDetails
            {
                EventId = Guid.NewGuid(), EventName = "Workshop 101",
                EventCategory = "Workshop", EventDate = DateTime.Today.AddDays(5), Status = "Active"
            });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task AddAsync_ShouldInsertSuccessfully_Integration()
        {
            var newEvent = new EventDetails
            {
                EventName = "Industrial Expo", EventCategory = "Industrial Event",
                EventDate = DateTime.Today.AddDays(20), Status = "Active"
            };

            var result = await _repository.AddAsync(newEvent);

            Assert.That(result, Is.True);
            Assert.That(_context.Events.Count(), Is.EqualTo(1));
            var saved = await _context.Events.FirstOrDefaultAsync();
            Assert.That(saved?.EventName, Is.EqualTo("Industrial Expo"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveRecord_Integration()
        {
            var eventId = Guid.NewGuid();
            _context.Events.Add(new EventDetails
            {
                EventId = eventId, EventName = "Event To Delete",
                EventCategory = "Workshop", EventDate = DateTime.Today.AddDays(3), Status = "Active"
            });
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(eventId);

            Assert.That(result, Is.True);
            Assert.That(_context.Events.Count(), Is.EqualTo(0));
        }
    }
}
