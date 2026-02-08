using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Web.Sessions;

[Route("_api/sessions")]
public class GetAllSessionsEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<IEnumerable<SessionResponse>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetAllSessionsEndpoint(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    [HttpGet]
    public override async Task<ActionResult<IEnumerable<SessionResponse>>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionRepository.GetAllAsync(cancellationToken);
        var response = sessions.Select(SessionResponse.FromDomain);
        return Ok(response);
    }
}