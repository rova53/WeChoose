using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Domain.Users;

namespace WeChooz.TechAssessment.Web.Sessions;

[Route("_api/session")]
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

    [HttpDelete("{id:guid}"), Authorize(Roles = $", {nameof(PolicyRoles.Formation)}")]
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