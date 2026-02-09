using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Participants;

namespace WeChooz.TechAssessment.Web.Participants;

[Route("_api/participants")]
public class DeleteParticipantEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<Guid>
    .WithActionResult
{
    private readonly IParticipantRepository _participantRepository;

    public DeleteParticipantEndpoint(IParticipantRepository participantRepository)
    {
        _participantRepository = participantRepository;
    }

    [HttpDelete("{id:guid}")]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken = default)
    {
        var existing = await _participantRepository.GetByIdAsync(
            id, cancellationToken);
        if (existing is null)
            return NotFound();

        await _participantRepository.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}