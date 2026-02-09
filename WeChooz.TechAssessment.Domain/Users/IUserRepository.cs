using WeChooz.TechAssessment.Domain.Common;

namespace WeChooz.TechAssessment.Domain.Users;

public interface IUserRepository : IRepository<User>
{
    Task<IReadOnlyCollection<User>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<bool> IsUserRegisteredAsync(Guid sessionId, string email, CancellationToken cancellationToken = default);
}