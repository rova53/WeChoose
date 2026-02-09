using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Participants.Requests;
using WeChooz.TechAssessment.Web.Participants.Responses;

namespace WeChooz.TechAssessment.Web.Participants;

[Route("_api/participants")]
public class CreateParticipantEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<CreateParticipantRequest>
    .WithActionResult<ParticipantResponse>
{
    private readonly IParticipantRepository _participantRepository;
    private readonly ISessionRepository _sessionRepository;

    public CreateParticipantEndpoint(
        IParticipantRepository participantRepository,
        ISessionRepository sessionRepository
        )
    {
        _participantRepository = participantRepository;
        _sessionRepository = sessionRepository;
    }

    [HttpPost]
    public override async Task<ActionResult<ParticipantResponse>> HandleAsync(
        [FromBody] CreateParticipantRequest request, 
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(
            request.SessionId, 
            cancellationToken
            );
        if (session is null)
            return BadRequest($"Session with id '{request.SessionId}' not found.");
        
        if (session.Course is not null 
            && session.Participants.Count >= session.Course.MaxCapacity)
            return BadRequest("Session has reached its maximum capacity.");

        var participant = new Participant
        {
            SessionId = request.SessionId,
            LastName = request.LastName,
            FirstName = request.FirstName,
            Email = request.Email,
            CompanyName = request.CompanyName
        };

        var created = await _participantRepository.AddAsync(
            participant, cancellationToken);
        return CreatedAtAction(nameof(GetParticipantByIdEndpoint), 
            new { id = created.Id }, ParticipantResponse.FromDomain(created));
    }
}