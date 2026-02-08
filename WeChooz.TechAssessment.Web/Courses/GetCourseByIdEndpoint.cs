using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Web.Courses.Responses;

namespace WeChooz.TechAssessment.Web.Courses;

[Route("_api/courses")]
public class GetCourseByIdEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<Guid>
    .WithActionResult<CourseResponse>
{
    private readonly ICourseRepository _courseRepository;

    public GetCourseByIdEndpoint(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    [HttpGet("{id:guid}")]
    public override async Task<ActionResult<CourseResponse>> HandleAsync(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken = default)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
            return NotFound();

        return Ok(CourseResponse.FromDomain(course));
    }
}