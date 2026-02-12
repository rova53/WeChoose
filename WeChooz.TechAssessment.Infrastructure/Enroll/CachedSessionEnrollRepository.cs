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

    public Task<SessionEnroll> AddAsync(SessionEnroll entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SessionEnroll> UpdateAsync(SessionEnroll entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SessionEnroll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
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