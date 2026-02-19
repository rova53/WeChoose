using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Courses;

namespace WeChooz.TechAssessment.Infrastructure.Courses;

public class CachedCourseRepository : ICourseRepository
{
    private readonly ICourseRepository _inner;
    private readonly ICacheService _cache;
    private const string Prefix = "Courses";

    public CachedCourseRepository(ICourseRepository inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var key = $"{Prefix}:{id}";
        var cached = await _cache.GetAsync<Course>(key, ct);
        if (cached is not null) return cached;

        var entity = await _inner.GetByIdAsync(id, ct);
        await _cache.SetAsync(key, entity, TimeSpan.FromMinutes(5), ct);

        return entity;
    }

    public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct = default)
    {
        var key = $"{Prefix}:all";
        var cached = await _cache.GetAsync<IEnumerable<Course>>(key, ct);
        if (cached is not null) return cached;

        var entities = await _inner.GetAllAsync(ct);
        await _cache.SetAsync(key, entities, TimeSpan.FromMinutes(5), ct);
        return entities;
    }

    public async Task<Course> AddAsync(Course entity, CancellationToken ct = default)
    {
        var result = await _inner.AddAsync(entity, ct);
        await _cache.RemoveAsync($"{Prefix}:all", ct);
        return result;
    }

    public async Task<Course> UpdateAsync(Course entity, CancellationToken ct = default)
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

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _inner.SaveChangesAsync(ct);
}