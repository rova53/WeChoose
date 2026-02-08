using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Participants.Requests;
using WeChooz.TechAssessment.Web.Participants.Responses;

namespace WeChooz.TechAssessment.Web.Participants;

[Route("_api/participants")]
public class UpdateParticipantEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync.WithRequest<UpdateParticipantRequest>
    .WithActionResult<ParticipantResponse>
{
    private readonly IParticipantRepository _participantRepository;
    private readonly ISessionRepository _sessionRepository;

    public UpdateParticipantEndpoint(
        IParticipantRepository participantRepository, 
        ISessionRepository sessionRepository)
    {
        _participantRepository = participantRepository;
        _sessionRepository = sessionRepository;
    }

    [HttpPut("{id:guid}")]
    public override async Task<ActionResult<ParticipantResponse>> HandleAsync(
        [FromBody] UpdateParticipantRequest request, 
        CancellationToken cancellationToken = default)
    {
        var existing = await _participantRepository.GetByIdAsync(
            request.Id, cancellationToken);
        if (existing is null)
            return NotFound();

        if (request.SessionId != existing.SessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(
                request.SessionId, cancellationToken);
            if (session is null)
                return BadRequest($"Session with id '{request.SessionId}' not found.");
        }

        var updated = existing with
        {
            SessionId = request.SessionId,
            LastName = request.LastName,
            FirstName = request.FirstName,
            Email = request.Email,
            CompanyName = request.CompanyName
        };

        var result = await _participantRepository.UpdateAsync(
            updated, cancellationToken);
        return Ok(ParticipantResponse.FromDomain(result));
    }
}