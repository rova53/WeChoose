using Microsoft.EntityFrameworkCore;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Common;
using WeChooz.TechAssessment.Infrastructure.Persistence;

namespace WeChooz.TechAssessment.Infrastructure.Sessions;

public class SessionRepository : BaseRepository<Session>, ISessionRepository
{
    public SessionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Course)
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Session>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Users)
            .OrderBy(s => s.StarDate)
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<Session>> GetAvailableSessionsAsync(TargetAudience? targetAudience = null, DeliveryMode? deliveryMode = null,
        DateOnly? startDate = null, DateOnly? endDate = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Session?> GetByIdWithUsersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}