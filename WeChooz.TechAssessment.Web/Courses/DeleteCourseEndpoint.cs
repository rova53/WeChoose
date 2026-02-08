using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Courses;

namespace WeChooz.TechAssessment.Web.Courses;
[Route("_api/courses")]
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

    [HttpDelete("{id:guid}")]
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