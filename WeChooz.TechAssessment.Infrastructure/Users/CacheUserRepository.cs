using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Users;

namespace WeChooz.TechAssessment.Infrastructure.Users;

public class CacheUserRepository: IUserRepository
{
    private readonly ICacheService _cache;
    private readonly IUserRepository _inner;
    private const string Prefix = "Users";

    public CacheUserRepository(IUserRepository inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var key = $"{Prefix}:{id}";
        var cached = await _cache.GetAsync<User>(key, ct);
        if (cached is not null) return cached;

        var entity = await _inner.GetByIdAsync(id, ct);
        await _cache.SetAsync(key, entity, TimeSpan.FromMinutes(5), ct);

        return entity;
    }

    public async Task<User> AddAsync(User entity, CancellationToken ct = default)
    {
        var result = await _inner.AddAsync(entity, ct);
        await _cache.RemoveAsync(Prefix, ct);
        return result;
    }

    public async Task<User> UpdateAsync(User entity, CancellationToken ct = default)
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

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
    {
        var key = $"{Prefix}:all";
        var cached = await _cache.GetAsync<IEnumerable<User>>(key, ct);
        if (cached is not null) return cached;

        var entities = await _inner.GetAllAsync(ct);
        await _cache.SetAsync(key, entities, TimeSpan.FromMinutes(5), ct);
        return entities;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _inner.SaveChangesAsync(ct);

    public async Task<IReadOnlyCollection<User>> GetBySessionIdAsync(Guid sessionId
        , CancellationToken ct = default)
    {
        var key = $"{Prefix}:GetBySessionId{sessionId}";
        var cached = await _cache.GetAsync<IEnumerable<User>>(key, ct);
        if (cached is not null) return (IReadOnlyCollection<User>)cached;

        var entity = await _inner.GetBySessionIdAsync(sessionId, ct);
        await _cache.SetAsync(key, entity, TimeSpan.FromMinutes(5), ct);

        return entity;
    }

    public async Task<bool> IsUserRegisteredAsync(Guid sessionId, string email
        , CancellationToken ct = default)
    {
        var key = $"{Prefix}:IsUserRegistered{sessionId}{email}";
        var cached = await _cache.GetAsync<bool?>(key, ct);
        if (cached is not null) return (bool)cached;

        var entity = await _inner.IsUserRegisteredAsync(sessionId, email, ct);
        await _cache.SetAsync(key, entity, TimeSpan.FromMinutes(5), ct);

        return entity;
    }

    public async Task<User?> FindByEmail(string email, CancellationToken ct = default)
    {
        var key = $"{Prefix}:FindByEmail{email}";
        var cached = await _cache.GetAsync<User?>(key, ct);
        if (cached is not null) return cached;

        var entity = await _inner.FindByEmail(email, ct);
        await _cache.SetAsync(key, entity, TimeSpan.FromMinutes(5), ct);

        return entity;
    }
}