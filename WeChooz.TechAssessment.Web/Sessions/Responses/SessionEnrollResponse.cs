using WeChooz.TechAssessment.Domain.Enroll;

namespace WeChooz.TechAssessment.Web.Sessions.Responses;

public class SessionEnrollResponse
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public DateTimeOffset EnrollmentDate { get; set; }

    public static SessionEnrollResponse FromDomain(SessionEnroll enroll) => new()
    {
        Id = enroll.Id,
        SessionId = enroll.SessionId,
        EnrollmentDate = enroll.EnrollmentDate
    };
}
