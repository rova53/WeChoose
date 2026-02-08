using System.Diagnostics.CodeAnalysis;

namespace WeChooz.TechAssessment.Web.Participants.Requests;
[ExcludeFromCodeCoverage]
public class CreateParticipantRequest
{
    public required Guid SessionId { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public required string Email { get; set; }
    public required string CompanyName { get; set; }
}