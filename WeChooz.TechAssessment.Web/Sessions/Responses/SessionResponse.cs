using WeChooz.TechAssessment.Domain.Sessions;

namespace WeChooz.TechAssessment.Web.Sessions.Responses;

public class SessionResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DeliveryMode DeliveryMode { get; set; }
    public int UserCount { get; set; }

    public static SessionResponse FromDomain(Session session) => new()
    {
        Id = session.Id,
        CourseId = session.CourseId,
        CourseName = session.Course?.Name ?? string.Empty,
        StartDate = session.StarDate,
        DeliveryMode = session.DeliveryMode,
        UserCount = session.Users?.Count ?? 0
    };
}