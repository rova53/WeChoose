using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Sessions;

namespace WeChooz.TechAssessment.Web.Sessions;

[Route("_api/sessions")]
public class DeleteSessionEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<Guid>
    .WithActionResult
{
    private readonly ISessionRepository _sessionRepository;

    public DeleteSessionEndpoint(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    [HttpDelete("{id:guid}")]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken = default)
    {
        var existing = await _sessionRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        await _sessionRepository.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}