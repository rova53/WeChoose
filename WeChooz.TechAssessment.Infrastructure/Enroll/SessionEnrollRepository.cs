using Microsoft.EntityFrameworkCore;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Common;
using WeChooz.TechAssessment.Infrastructure.Persistence;

namespace WeChooz.TechAssessment.Infrastructure.Enroll;

public class SessionEnrollRepository:BaseRepository<SessionEnroll>,ISessionEnrollRepository
{
    public SessionEnrollRepository(AppDbContext dbContext) : base(dbContext)
    {
        
    }

    public Task<List<SessionEnroll>> FindByUserAsync(Guid requestId, CancellationToken cancellationToken) => 
        DbSet.AsNoTracking()
            .Where(s=> s.UserId == requestId).ToListAsync(cancellationToken);
}