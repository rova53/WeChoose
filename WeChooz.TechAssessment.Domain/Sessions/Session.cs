using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Participants;

namespace WeChooz.TechAssessment.Domain.Sessions;

public record Session : Entity
{
    public Guid CourseId { get; set; }
    public DateOnly StarDate { get; set; }
    public DeliveryMode DeliveryMode { get; set; }
    public Course Course { get; set; }
    public ICollection<Participant> Participants { get; set; }
}