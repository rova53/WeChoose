using Microsoft.EntityFrameworkCore;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Infrastructure.Common;
using WeChooz.TechAssessment.Infrastructure.Persistence;

namespace WeChooz.TechAssessment.Infrastructure.Participants;

public class ParticipantRepository: BaseRepository<Participant>, IParticipantRepository
{
    public ParticipantRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<Participant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Session)
            .ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Participant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(p => p.Session)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<Participant>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsParticipantRegisteredAsync(Guid sessionId, string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}