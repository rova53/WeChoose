using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Domain.Users;

namespace WeChooz.TechAssessment.Domain.Enroll;

public record SessionEnroll : Entity
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public DateTime EnrollmentDate { get; set; }

    public Session Session { get; set; } = null!;
    public User User { get; set; } = null!;
}
