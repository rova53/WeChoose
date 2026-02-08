using WeChooz.TechAssessment.Domain.Participants;

namespace WeChooz.TechAssessment.Web.Participants.Responses;

public class ParticipantResponse
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    public static ParticipantResponse FromDomain(Participant participant) => new()
    {
        Id = participant.Id,
        SessionId = participant.SessionId,
        LastName = participant.LastName,
        FirstName = participant.FirstName,
        Email = participant.Email,
        CompanyName = participant.CompanyName
    };
}