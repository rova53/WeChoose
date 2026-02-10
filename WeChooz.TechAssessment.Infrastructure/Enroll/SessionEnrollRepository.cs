using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Infrastructure.Common;
using WeChooz.TechAssessment.Infrastructure.Persistence;

namespace WeChooz.TechAssessment.Infrastructure.Enroll;

public class SessionEnrollRepository:BaseRepository<SessionEnroll>,ISessionEnrollRepository
{
    public SessionEnrollRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}