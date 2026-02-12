using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Courses;

namespace WeChooz.TechAssessment.Domain.Sessions;

public interface ISessionRepository : IRepository<Session>
{
    Task<IReadOnlyCollection<Session>> GetAvailableSessionsAsync(
        TargetAudience? targetAudience = null,
        DeliveryMode? deliveryMode = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default);

    Task<Session?> GetByIdWithUsersAsync(Guid id, CancellationToken cancellationToken = default);
}