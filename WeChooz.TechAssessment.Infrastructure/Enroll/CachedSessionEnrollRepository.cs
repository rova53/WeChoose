using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Enroll;

namespace WeChooz.TechAssessment.Infrastructure.Enroll;

public class CachedSessionEnrollRepository: ISessionEnrollRepository
{
    private readonly ISessionEnrollRepository _inner;
    private readonly ICacheService _cache;
    private const string Prefix = "SessionEnrolls";

    public CachedSessionEnrollRepository(ISessionEnrollRepository repository
        , ICacheService cacheService)
    {
        _inner = repository;
        _cache = cacheService;
    }
    public async Task<SessionEnroll?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<SessionEnroll> AddAsync(SessionEnroll entity, CancellationToken ct = default)
    {
        var result = await _inner.AddAsync(entity, ct);
        await _cache.RemoveAsync(Prefix, ct);
        return result;
    }

    public async Task<SessionEnroll> UpdateAsync(SessionEnroll entity, CancellationToken ct = default)
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

    public async Task<IEnumerable<SessionEnroll>> GetAllAsync(CancellationToken ct = default)
    {
        var key = $"{Prefix}:all";
        var cached = await _cache.GetAsync<IEnumerable<SessionEnroll>>(key, ct);
        if (cached is not null) return cached;

        var entities = await _inner.GetAllAsync(ct);
        var allAsync = entities as SessionEnroll[] ?? entities.ToArray();
        await _cache.SetAsync(key, allAsync, TimeSpan.FromMinutes(5), ct);
        return allAsync;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _inner.SaveChangesAsync(ct);
    public async Task<List<SessionEnroll>> FindByUserAsync(Guid requestId, CancellationToken ct = default)
    {
        var key = $"{Prefix}:findByUser{requestId}";
        var cached = await _cache.GetAsync<IEnumerable<SessionEnroll>>(key, ct);
        if (cached is not null) return (List<SessionEnroll>)cached;

        var entities = await _inner.FindByUserAsync(requestId,ct);
        await _cache.SetAsync(key, entities, TimeSpan.FromMinutes(5), ct);
        return entities;
    }
}