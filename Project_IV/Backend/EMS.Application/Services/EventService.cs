using EMS.Application.DTOs;
using EMS.Application.Services.Interfaces;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMemoryCache _cache;
        private const string ListCacheKey = "all_events";
        private const string ItemCachePrefix = "event_";

        public EventService(
            IEventRepository eventRepo,
            ICategoryRepository categoryRepo,
            IMemoryCache cache)
        {
            _eventRepo    = eventRepo;
            _categoryRepo = categoryRepo;
            _cache        = cache;
        }

        public async Task<PagedResult<EventResponseDto>> GetAllAsync(int page, int pageSize)
        {
            if (!_cache.TryGetValue(ListCacheKey, out IEnumerable<EventDetails>? events))
            {
                events = await _eventRepo.GetAllAsync();
                _cache.Set(ListCacheKey, events, TimeSpan.FromMinutes(5));
            }

            var categories = await _categoryRepo.GetAllAsync();
            var list = events!.ToList();
            var dtos = list.Select(e => MapToDto(e, categories)).ToList();

            return new PagedResult<EventResponseDto>
            {
                TotalCount = dtos.Count,
                Page       = page,
                PageSize   = pageSize,
                Items      = dtos.Skip((page - 1) * pageSize).Take(pageSize)
            };
        }

        public async Task<EventResponseDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"{ItemCachePrefix}{id}";
            if (!_cache.TryGetValue(cacheKey, out EventDetails? ev))
            {
                ev = await _eventRepo.GetByIdAsync(id);
                if (ev != null)
                    _cache.Set(cacheKey, ev, TimeSpan.FromMinutes(5));
            }

            if (ev == null) return null;
            var categories = await _categoryRepo.GetAllAsync();
            return MapToDto(ev, categories);
        }

        public async Task<bool> CreateAsync(EventDto dto)
        {
            var category = await ResolveCategoryAsync(dto.CategoryId);
            if (category == null) return false;

            var ev = new EventDetails
            {
                EventName     = dto.EventName,
                EventCategory = category.CategoryName,
                CategoryId    = category.Id,         
                EventDate     = dto.EventDate,
                Description   = dto.Description,
                Status        = dto.Status
            };

            var result = await _eventRepo.AddAsync(ev);
            if (result) InvalidateListCache();
            return result;
        }

        public async Task<bool> UpdateAsync(Guid id, EventDto dto)
        {
            var existing = await _eventRepo.GetByIdAsync(id);
            if (existing == null) return false;

            var category = await ResolveCategoryAsync(dto.CategoryId);
            if (category == null) return false;

            existing.EventName     = dto.EventName;
            existing.EventCategory = category.CategoryName;
            existing.CategoryId    = category.Id;  
            existing.EventDate     = dto.EventDate;
            existing.Description   = dto.Description;
            existing.Status        = dto.Status;

            var result = await _eventRepo.UpdateAsync(existing);
            if (result)
            {
                InvalidateListCache();
                _cache.Remove($"{ItemCachePrefix}{id}");
            }
            return result;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _eventRepo.DeleteAsync(id);
            if (result)
            {
                InvalidateListCache();
                _cache.Remove($"{ItemCachePrefix}{id}");
            }
            return result;
        }

        // ── Private helpers

        private async Task<CategoryDetails?> ResolveCategoryAsync(Guid categoryId)
        {
            var all = await _categoryRepo.GetAllAsync();
            return all.FirstOrDefault(c => c.Id == categoryId);
        }

        private static EventResponseDto MapToDto(
            EventDetails e,
            IEnumerable<CategoryDetails> categories)
        {
            // Try to find category by the persisted FK first; fall back to name match
            var cat = categories.FirstOrDefault(c => c.Id == e.CategoryId)
                   ?? categories.FirstOrDefault(c => c.CategoryName == e.EventCategory);

            return new EventResponseDto
            {
                EventId       = e.EventId,
                EventName     = e.EventName,
                EventCategory = e.EventCategory,
                CategoryId    = cat?.Id ?? Guid.Empty,
                EventDate     = e.EventDate,
                Description   = e.Description,
                Status        = e.Status
            };
        }

        private void InvalidateListCache() => _cache.Remove(ListCacheKey);
    }
}
