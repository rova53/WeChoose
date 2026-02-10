using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Users;

namespace WeChooz.TechAssessment.Domain.Sessions;

public record Session : Entity
{
    public Guid CourseId { get; set; }
    public DateOnly StarDate { get; set; }
    public DeliveryMode DeliveryMode { get; set; }
    public Course Course { get; set; }

    public ICollection<SessionEnroll> Enrollments { get; set; } = new List<SessionEnroll>();
}