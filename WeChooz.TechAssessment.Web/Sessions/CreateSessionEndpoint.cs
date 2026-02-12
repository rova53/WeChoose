using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Sessions.Requests;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Web.Sessions;

[Route("_api/session")]
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

    [HttpPost, Authorize(Roles = $", {nameof(PolicyRoles.Formation)}")]
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
            Enrollments = []
        };

        var created = await _sessionRepository.AddAsync(session, cancellationToken);

        var result = await _sessionRepository.GetByIdAsync(created.Id, cancellationToken);
        return Created($"_api/session/${created.Id }", SessionResponse.FromDomain(result!));
    }
}