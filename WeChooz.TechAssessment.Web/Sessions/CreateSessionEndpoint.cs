using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Requests;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Web.Sessions;

[Route("_api/sessions")]
public class CreateSessionEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<CreateSessionRequest>
    .WithActionResult<SessionResponse>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICourseRepository _courseRepository;

    public CreateSessionEndpoint(
        ISessionRepository sessionRepository, 
        ICourseRepository courseRepository
        )
    {
        _sessionRepository = sessionRepository;
        _courseRepository = courseRepository;
    }

    [HttpPost]
    public override async Task<ActionResult<SessionResponse>> HandleAsync(
        [FromBody] CreateSessionRequest request, 
        CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
            return BadRequest($"Course with id '{request.CourseId}' not found.");

        var session = new Session
        {
            CourseId = request.CourseId,
            StarDate = request.StartDate,
            DeliveryMode = request.DeliveryMode,
            Participants = []
        };

        var created = await _sessionRepository.AddAsync(session, cancellationToken);
        
        var result = await _sessionRepository.GetByIdAsync(created.Id, cancellationToken);
        return CreatedAtAction(nameof(GetSessionByIdEndpoint), new { id = created.Id }, SessionResponse.FromDomain(result!));
    }
}