using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Users;

namespace WeChooz.TechAssessment.Web.Courses;
[Route("_api/course")]
public class DeleteCourseEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<Guid>
    .WithActionResult
{
    private readonly ICourseRepository _courseRepository;
    public DeleteCourseEndpoint(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    [HttpDelete("{id:guid}"), Authorize(Roles = $", {nameof(PolicyRoles.Formation)}")]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken = default)
    {
        var existing = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        await _courseRepository.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}