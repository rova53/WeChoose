using System.Security.Claims;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Requests;

namespace WeChooz.TechAssessment.Web.Sessions;

[Route("_api/session")]
[Authorize]
public class EnrollSessionEndpoint : EndpointBaseAsync
    .WithRequest<EnrollSessionRequest>
    .WithActionResult
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<EnrollSessionEndpoint> _logger;
    private readonly ISessionEnrollRepository _sessionEnrollRepository;
    public EnrollSessionEndpoint(
        ISessionRepository sessionRepository,
        ISessionEnrollRepository sessionEnrollRepository,
        ILogger<EnrollSessionEndpoint> logger)
    {
        _sessionRepository = sessionRepository;
        _sessionEnrollRepository = sessionEnrollRepository;
        _logger = logger;
    }

    [HttpPost("{sessionId:guid}/enroll")]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute] EnrollSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return NotFound($"Session avec l'id '{request.SessionId}' non trouvée.");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            return Unauthorized();

        try
        {
            if (session.Enrollments?.Any(u => u.UserId == userGuid) == true)
                return BadRequest("Vous êtes déjà inscrit à cette session.");
            if (session.Enrollments?.Count < session.Course.MaxCapacity
                && !session.Enrollments.Any(u => u.UserId == new Guid(userId)))
                
                await _sessionEnrollRepository.AddAsync(
                    new()
                    {
                        SessionId = session.Id, 
                        UserId = userGuid,
                        EnrollmentDate = DateTime.UtcNow
                    }, 
                    cancellationToken);
            else
            {
                _logger.LogInformation(
                    "Utilisateur {UserId} pas inscrit sur {SessionId}",
                    userGuid,
                    session.Id);

                return BadRequest("Session complete. Impossible d'inscrire de nouveaux participants.");
            }

            _logger.LogInformation(
                "Utilisateur {UserId} inscrit à la session {SessionId}",
                userGuid,
                session.Id);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erreur lors de l'inscription de l'utilisateur {UserId} à la session {SessionId}",
                userGuid,
                session.Id);
            
            return StatusCode(500, "Une erreur est survenue lors de l'inscription.");
        }
    }
}
