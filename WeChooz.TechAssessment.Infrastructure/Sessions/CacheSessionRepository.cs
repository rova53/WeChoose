using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;

namespace WeChooz.TechAssessment.Infrastructure.Sessions;

public class CacheSessionRepository: ISessionRepository
{
    private readonly ISessionRepository _inner;
    private readonly ICacheService _cache;
    private const string Prefix = "Sessions";

    public CacheSessionRepository(ISessionRepository inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }
    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var key = $"{Prefix}:{id}";
        var cached = await _cache.GetAsync<Session>(key, ct);
        if (cached is not null) return cached;

        var entity = await _inner.GetByIdAsync(id, ct);
        await _cache.SetAsync(key, entity, TimeSpan.FromMinutes(5), ct);

        return entity;
    }

    public async Task<Session> AddAsync(Session entity, CancellationToken ct = default)
    {
        var result = await _inner.AddAsync(entity, ct);
        await _cache.RemoveAsync(Prefix, ct);
        return result;
    }

    public async Task<Session> UpdateAsync(Session entity, CancellationToken ct = default)
    {
        var result = await _inner.UpdateAsync(entity, ct);
        await _cache.RemoveAsync($"{Prefix}:{entity.Id}", ct);
        await _cache.RemoveAsync($"{Prefix}:all", ct);
        return result;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _inner.DeleteAsync(id, ct);
        await _cache.RemoveAsync($"{Prefix}:{id}", ct);
        await _cache.RemoveAsync($"{Prefix}:all", ct);
    }

    public async Task<IEnumerable<Session>> GetAllAsync(CancellationToken ct = default)
    {
        var key = $"{Prefix}:all";
        var cached = await _cache.GetAsync<IEnumerable<Session>>(key, ct);
        if (cached is not null) return cached;

        var entities = await _inner.GetAllAsync(ct);
        var allAsync = entities as Session[] ?? entities.ToArray();
        await _cache.SetAsync(key, allAsync, TimeSpan.FromMinutes(5), ct);
        return allAsync;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _inner.SaveChangesAsync(ct);

    public async Task<IReadOnlyCollection<Session>> GetAvailableSessionsAsync(TargetAudience? targetAudience = null,
        DeliveryMode? deliveryMode = null,
        DateOnly? startDate = null, 
        DateOnly? endDate = null, 
        CancellationToken ct = default)
    {
        var key = $"{Prefix}:GetAvailableSessions{targetAudience}{deliveryMode}{startDate}{endDate}";
        var cached = await _cache.GetAsync<IEnumerable<Session>>(key, ct);
        if (cached is not null) return (IReadOnlyCollection<Session>)cached;
        
        var entities = await _inner.GetAvailableSessionsAsync(
            targetAudience, 
            deliveryMode,
            startDate,
            endDate,
            ct);
        var allAsync = entities as Session[] ?? entities.ToArray();
        await _cache.SetAsync(key, allAsync, TimeSpan.FromMinutes(5), ct);
        return allAsync;
    }

    public async Task<Session?> GetByIdWithUsersAsync(Guid id, CancellationToken ct = default)
    {
        var key = $"{Prefix}:GetByIdWithUsers{id}";
        var cached = await _cache.GetAsync<Session>(key, ct);
        if (cached is not null) return cached;
        
        var entity = await _inner.GetByIdWithUsersAsync(id, ct);
        if (entity is not null)
            await _cache.SetAsync(key, entity, TimeSpan.FromMinutes(5), ct);

        return entity;
    }
}