using Microsoft.EntityFrameworkCore;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Infrastructure.Common;
using WeChooz.TechAssessment.Infrastructure.Persistence;

namespace WeChooz.TechAssessment.Infrastructure.Courses;

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Sessions)
            .ThenInclude(s => s.Users)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(c => c.Sessions)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}