using EMS.Application.DTOs;
using EMS.Application.Services.Interfaces;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepo;
        private readonly ISpeakerRepository _speakerRepo;
        private readonly IEventRepository _eventRepo;
        private readonly IMemoryCache _cache;
        private const string EventSessionsCachePrefix = "sessions_event_";
        private const string ItemCachePrefix = "session_";

        public SessionService(
            ISessionRepository sessionRepo,
            ISpeakerRepository speakerRepo,
            IEventRepository eventRepo,
            IMemoryCache cache)
        {
            _sessionRepo = sessionRepo;
            _speakerRepo = speakerRepo;
            _eventRepo   = eventRepo;
            _cache       = cache;
        }

        public async Task<PagedResult<SessionResponseDto>> GetByEventAsync(
            Guid eventId, int page, int pageSize)
        {
            var cacheKey = $"{EventSessionsCachePrefix}{eventId}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<SessionInfo>? sessions))
            {
                sessions = await _sessionRepo.GetByEventIdAsync(eventId);
                _cache.Set(cacheKey, sessions, TimeSpan.FromMinutes(5));
            }

            var list = sessions!.ToList();
            var dtos = list.Select(MapToDto).ToList();

            return new PagedResult<SessionResponseDto>
            {
                TotalCount = dtos.Count,
                Page       = page,
                PageSize   = pageSize,
                Items      = dtos.Skip((page - 1) * pageSize).Take(pageSize)
            };
        }

        public async Task<SessionResponseDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"{ItemCachePrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out SessionInfo? session))
            {
                session = await _sessionRepo.GetByIdAsync(id);
                if (session != null)
                    _cache.Set(cacheKey, session, TimeSpan.FromMinutes(5));
            }
            return session == null ? null : MapToDto(session);
        }

        public async Task<bool> AddAsync(SessionDto dto)
        {
            // Spec rule: SessionStart must be on or after the parent event's date.
            var parentEvent = await _eventRepo.GetByIdAsync(dto.EventId);
            if (parentEvent == null) return false;
            if (dto.SessionStart.Date < parentEvent.EventDate.Date) return false;

            if (dto.SpeakerId.HasValue)
            {
                var speaker = await _speakerRepo.GetByIdAsync(dto.SpeakerId.Value);
                if (speaker == null) return false;
            }

            var session = new SessionInfo
            {
                EventId      = dto.EventId,
                SessionTitle = dto.SessionTitle,
                SpeakerId    = dto.SpeakerId,
                Description  = dto.Description,
                SessionStart = dto.SessionStart,
                SessionEnd   = dto.SessionEnd,
                SessionUrl   = dto.SessionUrl
            };

            var result = await _sessionRepo.AddAsync(session);
            if (result) InvalidateEventCache(dto.EventId);
            return result;
        }

        public async Task<bool> UpdateAsync(Guid id, SessionDto dto)
        {
            var existing = await _sessionRepo.GetByIdAsync(id);
            if (existing == null) return false;

            // Spec rule: SessionStart must be on or after the parent event's date.
            var parentEvent = await _eventRepo.GetByIdAsync(existing.EventId);
            if (parentEvent == null) return false;
            if (dto.SessionStart.Date < parentEvent.EventDate.Date) return false;

            if (dto.SpeakerId.HasValue)
            {
                var speaker = await _speakerRepo.GetByIdAsync(dto.SpeakerId.Value);
                if (speaker == null) return false;
            }

            existing.SessionTitle = dto.SessionTitle;
            existing.SpeakerId    = dto.SpeakerId;
            existing.Description  = dto.Description;
            existing.SessionStart = dto.SessionStart;
            existing.SessionEnd   = dto.SessionEnd;
            existing.SessionUrl   = dto.SessionUrl;

            var result = await _sessionRepo.UpdateAsync(existing);
            if (result)
            {
                InvalidateEventCache(existing.EventId);
                _cache.Remove($"{ItemCachePrefix}{id}");
            }
            return result;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var session = await _sessionRepo.GetByIdAsync(id);
            if (session == null) return false;

            var result = await _sessionRepo.DeleteAsync(id);
            if (result)
            {
                InvalidateEventCache(session.EventId);
                _cache.Remove($"{ItemCachePrefix}{id}");
            }
            return result;
        }

        public async Task<bool> AssignSpeakerAsync(Guid sessionId, Guid speakerId)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null) return false;

            var speaker = await _speakerRepo.GetByIdAsync(speakerId);
            if (speaker == null) return false;

            session.SpeakerId = speakerId;
            var result = await _sessionRepo.UpdateAsync(session);
            if (result)
            {
                InvalidateEventCache(session.EventId);
                _cache.Remove($"{ItemCachePrefix}{sessionId}");
            }
            return result;
        }

        // ── Helpers

        private static SessionResponseDto MapToDto(SessionInfo s) => new()
        {
            SessionId    = s.SessionId,
            EventId      = s.EventId,
            SessionTitle = s.SessionTitle,
            SpeakerId    = s.SpeakerId,
            SpeakerName  = s.Speaker?.SpeakerName,
            Description  = s.Description,
            SessionStart = s.SessionStart,
            SessionEnd   = s.SessionEnd,
            SessionUrl   = s.SessionUrl
        };

        private void InvalidateEventCache(Guid eventId)
            => _cache.Remove($"{EventSessionsCachePrefix}{eventId}");
    }
}
