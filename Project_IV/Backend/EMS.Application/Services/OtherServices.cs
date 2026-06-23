using EMS.Application.DTOs;
using EMS.Application.Services.Interfaces;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.Application.Services
{
    // ── Speaker
    public class SpeakerService : ISpeakerService
    {
        private readonly ISpeakerRepository _speakerRepo;
        private readonly IMemoryCache _cache;
        private const string ListCacheKey = "all_speakers";
        private const string ItemCachePrefix = "speaker_";

        public SpeakerService(ISpeakerRepository speakerRepo, IMemoryCache cache)
        {
            _speakerRepo = speakerRepo;
            _cache = cache;
        }

        public async Task<IEnumerable<SpeakerResponseDto>> GetAllAsync()
        {
            if (!_cache.TryGetValue(ListCacheKey, out IEnumerable<SpeakersDetails>? speakers))
            {
                speakers = await _speakerRepo.GetAllAsync();
                _cache.Set(ListCacheKey, speakers, TimeSpan.FromMinutes(5));
            }

            return speakers!.Select(s => new SpeakerResponseDto
            {
                SpeakerId = s.SpeakerId,
                SpeakerName = s.SpeakerName
            });
        }

        public async Task<SpeakerResponseDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"{ItemCachePrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out SpeakersDetails? speaker))
            {
                speaker = await _speakerRepo.GetByIdAsync(id);
                if (speaker != null)
                    _cache.Set(cacheKey, speaker, TimeSpan.FromMinutes(5));
            }

            return speaker == null ? null : new SpeakerResponseDto
            {
                SpeakerId = speaker.SpeakerId,
                SpeakerName = speaker.SpeakerName
            };
        }

        public async Task<bool> AddAsync(SpeakerDto dto)
        {
            var speaker = new SpeakersDetails { SpeakerName = dto.SpeakerName };
            var result = await _speakerRepo.AddAsync(speaker);
            if (result) _cache.Remove(ListCacheKey);
            return result;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _speakerRepo.DeleteAsync(id);
            if (result)
            {
                _cache.Remove(ListCacheKey);
                _cache.Remove($"{ItemCachePrefix}{id}");
            }
            return result;
        }
    }

    // ── Category

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "all_categories";

        public CategoryService(ICategoryRepository categoryRepo, IMemoryCache cache)
        {
            _categoryRepo = categoryRepo;
            _cache = cache;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<CategoryDetails>? categories))
            {
                categories = await _categoryRepo.GetAllAsync();
                _cache.Set(CacheKey, categories, TimeSpan.FromMinutes(10));
            }

            return categories!.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName
            });
        }

        public async Task<bool> AddAsync(CategoryDto dto)
        {
            var cat = new CategoryDetails { CategoryName = dto.CategoryName };
            var result = await _categoryRepo.AddAsync(cat);
            if (result) _cache.Remove(CacheKey);
            return result;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _categoryRepo.DeleteAsync(id);
            if (result) _cache.Remove(CacheKey);
            return result;
        }
    }

    // ── Participant

    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantEventRepository _participantRepo;
        private readonly IEventRepository _eventRepo;

        public ParticipantService(
            IParticipantEventRepository participantRepo,
            IEventRepository eventRepo)
        {
            _participantRepo = participantRepo;
            _eventRepo = eventRepo;
        }

        public async Task<bool> RegisterForEventAsync(string email, Guid eventId)
        {
            if (await _participantRepo.IsAlreadyRegisteredAsync(email, eventId))
                return false;

            var entry = new ParticipantEventDetails
            {
                ParticipantEmailId = email,
                EventId = eventId,
                IsAttended = false
            };
            return await _participantRepo.RegisterAsync(entry);
        }

        public async Task<IEnumerable<ParticipantRegistrationResponseDto>> GetRegisteredEventsAsync(
            string email)
        {
            var registrations = await _participantRepo.GetByParticipantAsync(email);
            var dtos = new List<ParticipantRegistrationResponseDto>();

            foreach (var r in registrations)
            {
                var ev = await _eventRepo.GetByIdAsync(r.EventId);
                dtos.Add(new ParticipantRegistrationResponseDto
                {
                    Id = r.Id,
                    ParticipantEmailId = r.ParticipantEmailId,
                    EventId = r.EventId,
                    EventName = ev?.EventName ?? string.Empty,
                    IsAttended = r.IsAttended
                });
            }
            return dtos;
        }

        public async Task<bool?> MarkAttendanceAsync(
            Guid registrationId, bool attended, string? callerEmail)
        {
            if (callerEmail != null)
            {
                var registration = await _participantRepo.GetByIdAsync(registrationId);
                if (registration == null) return false;

                if (registration.ParticipantEmailId != callerEmail)
                    return null;
            }

            return await _participantRepo.UpdateAttendanceAsync(registrationId, attended);
        }

        public async Task<ParticipantRegistrationResponseDto?> GetRegistrationByIdAsync(
            Guid registrationId)
        {
            var r = await _participantRepo.GetByIdAsync(registrationId);
            if (r == null) return null;

            var ev = await _eventRepo.GetByIdAsync(r.EventId);
            return new ParticipantRegistrationResponseDto
            {
                Id = r.Id,
                ParticipantEmailId = r.ParticipantEmailId,
                EventId = r.EventId,
                EventName = ev?.EventName ?? string.Empty,
                IsAttended = r.IsAttended
            };
        }

        // ── NEW: Get all registrations for a specific event (Admin only)
        public async Task<IEnumerable<ParticipantRegistrationResponseDto>> GetByEventIdAsync(
            Guid eventId)
        {
            var registrations = await _participantRepo.GetByEventIdAsync(eventId);
            return registrations.Select(r => new ParticipantRegistrationResponseDto
            {
                Id = r.Id,
                ParticipantEmailId = r.ParticipantEmailId,
                EventId = r.EventId,
                EventName = r.Event?.EventName ?? string.Empty,
                IsAttended = r.IsAttended
            });
        }
    }
}