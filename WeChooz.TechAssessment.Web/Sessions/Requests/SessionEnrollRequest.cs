namespace WeChooz.TechAssessment.Web.Sessions.Requests;

public class SessionEnrollRequest
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public DateTimeOffset EnrollmentDate { get; set; }
}