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
    public class SpeakerRepositoryTests : IDisposable
    {
        private EMSDbContext _context = null!;
        private SpeakerRepository _repository = null!;
        private bool _disposed;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EMSDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new EMSDbContext(options);
            _repository = new SpeakerRepository(_context, new Mock<ILogger<SpeakerRepository>>().Object);
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
        public async Task GetAllAsyncShouldReturnAllSpeakers()
        {
            // Arrange
            _context.Speakers.AddRange(
                new SpeakersDetails { SpeakerId = Guid.NewGuid(), SpeakerName = "Alice" },
                new SpeakersDetails { SpeakerId = Guid.NewGuid(), SpeakerName = "Bob" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task AddAsyncShouldInsertSpeakerSuccessfully()
        {
            // Arrange
            var speaker = new SpeakersDetails { SpeakerName = "Charlie" };

            // Act
            var result = await _repository.AddAsync(speaker);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_context.Speakers.Count(), Is.EqualTo(1));
            var saved = await _context.Speakers.FirstOrDefaultAsync();
            Assert.That(saved?.SpeakerName, Is.EqualTo("Charlie"));
        }

        [Test]
        public async Task DeleteAsyncShouldRemoveSpeaker()
        {
            // Arrange
            var id = Guid.NewGuid();
            _context.Speakers.Add(new SpeakersDetails { SpeakerId = id, SpeakerName = "To Delete" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(id);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_context.Speakers.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetByIdAsyncShouldReturnCorrectSpeaker()
        {
            // Arrange
            var id = Guid.NewGuid();
            _context.Speakers.Add(new SpeakersDetails { SpeakerId = id, SpeakerName = "Diana" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.SpeakerName, Is.EqualTo("Diana"));
        }

        [Test]
        public async Task DeleteAsyncShouldReturnFalseForNonExistentSpeaker()
        {
            // Arrange — empty DB

            // Act
            var result = await _repository.DeleteAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
