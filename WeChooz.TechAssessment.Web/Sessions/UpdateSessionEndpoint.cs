using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Sessions.Requests;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Web.Sessions;

[Route("_api/session")]
public class UpdateSessionEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<UpdateSessionRequest>
    .WithActionResult<SessionResponse>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICourseRepository _courseRepository;

    public UpdateSessionEndpoint(
        ISessionRepository sessionRepository, 
        ICourseRepository courseRepository
        )
    {
        _sessionRepository = sessionRepository;
        _courseRepository = courseRepository;
    }

    [HttpPut("{id:guid}"), Authorize(Roles = $", {nameof(PolicyRoles.Formation)}")]
    public override async Task<ActionResult<SessionResponse>> HandleAsync(
        [FromBody] UpdateSessionRequest request, 
        CancellationToken cancellationToken = default
        )
    {
        var existing = await _sessionRepository.GetByIdAsync(
            request.Id, cancellationToken);
        if (existing is null)
            return NotFound();

        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
            return BadRequest($"Course with id '{request.CourseId}' not found.");

        var updated = existing with
        {
            CourseId = request.CourseId,
            StarDate = request.StartDate,
            DeliveryMode = request.DeliveryMode,
            Course = null  
        };

        var result = await _sessionRepository.UpdateAsync(updated, cancellationToken);
        result.Course = course;
        return Ok(SessionResponse.FromDomain(result));
    }
}