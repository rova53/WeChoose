using WeChooz.TechAssessment.Domain.Common;

namespace WeChooz.TechAssessment.Domain.Participants;

public interface IParticipantRepository : IRepository<Participant>
{
    Task<IReadOnlyCollection<Participant>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<bool> IsParticipantRegisteredAsync(Guid sessionId, string email, CancellationToken cancellationToken = default);
}