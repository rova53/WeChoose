using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Web.Sessions;

[Route("_api/sessions")]
public class GetSessionByIdEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<Guid>
    .WithActionResult<SessionResponse>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionByIdEndpoint(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    [HttpGet("{id:guid}")]
    public override async Task<ActionResult<SessionResponse>> HandleAsync(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(id, cancellationToken);
        if (session is null)
            return NotFound();

        return Ok(SessionResponse.FromDomain(session));
    }
}