using System.Diagnostics.CodeAnalysis;
using WeChooz.TechAssessment.Domain.Sessions;

namespace WeChooz.TechAssessment.Web.Sessions.Requests;
[ExcludeFromCodeCoverage]
public class CreateSessionRequest
{
    public Guid CourseId { get; set; }
    public DateOnly StartDate { get; set; }
    public DeliveryMode DeliveryMode { get; set; }
}