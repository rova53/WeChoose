using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Web.Participants.Responses;

namespace WeChooz.TechAssessment.Web.Participants;

[Route("_api/participants")]
public class GetAllParticipantsEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<IEnumerable<ParticipantResponse>>
{
    private readonly IParticipantRepository _participantRepository;

    public GetAllParticipantsEndpoint(IParticipantRepository participantRepository)
    {
        _participantRepository = participantRepository;
    }

    [HttpGet]
    public override async Task<ActionResult<IEnumerable<ParticipantResponse>>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var participants = await _participantRepository.GetAllAsync(
            cancellationToken);
        var response = participants.Select(
            ParticipantResponse.FromDomain);
        return Ok(response);
    }
}