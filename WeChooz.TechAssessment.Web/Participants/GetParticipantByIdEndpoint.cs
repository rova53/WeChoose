using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Web.Participants.Responses;

namespace WeChooz.TechAssessment.Web.Participants;

[Route("_api/participants")]
public class GetParticipantByIdEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<Guid>
    .WithActionResult<ParticipantResponse>
{
    private readonly IParticipantRepository _participantRepository;

    public GetParticipantByIdEndpoint(IParticipantRepository participantRepository)
    {
        _participantRepository = participantRepository;
    }

    [HttpGet("{id:guid}")]
    public override async Task<ActionResult<ParticipantResponse>> HandleAsync(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken = default)
    {
        var participant = await _participantRepository.GetByIdAsync(id, cancellationToken);
        if (participant is null)
            return NotFound();

        return Ok(ParticipantResponse.FromDomain(participant));
    }
}