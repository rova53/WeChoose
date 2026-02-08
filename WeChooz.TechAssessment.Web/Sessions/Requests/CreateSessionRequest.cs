using System.Diagnostics.CodeAnalysis;
using WeChooz.TechAssessment.Domain.Sessions;

namespace WeChooz.TechAssessment.Web.Sessions.Requests;
[ExcludeFromCodeCoverage]
public class CreateSessionRequest
{
    public required Guid CourseId { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DeliveryMode DeliveryMode { get; set; }
}