using WeChooz.TechAssessment.Domain.Common;

namespace WeChooz.TechAssessment.Domain.Enroll;

public interface ISessionEnrollRepository: IRepository<SessionEnroll>
{
    Task<List<SessionEnroll>> FindByUserAsync(Guid requestId);
}