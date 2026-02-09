using Microsoft.EntityFrameworkCore;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Infrastructure.Common;
using WeChooz.TechAssessment.Infrastructure.Persistence;

namespace WeChooz.TechAssessment.Infrastructure.Users;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Session)
            .ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(p => p.Session)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<User>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsUserRegisteredAsync(Guid sessionId, string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FindByNameAndPassAsync(string modelUsername, string modelPassword, CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .AnyAsync(p => p.Email == modelUsername && p.Password == modelPassword, cancellationToken);
    }
}