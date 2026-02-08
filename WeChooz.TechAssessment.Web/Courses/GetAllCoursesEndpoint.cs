using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Web.Courses.Responses;

namespace WeChooz.TechAssessment.Web.Courses;
[Route("_api/courses")]
public class GetAllCoursesEndpoint: Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<IEnumerable<CourseResponse>>
{
    private readonly ICourseRepository _courseRepository;
    public GetAllCoursesEndpoint(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    [HttpGet]
    public override async Task<ActionResult<IEnumerable<CourseResponse>>> 
        HandleAsync(CancellationToken cancellationToken = default)
    {
        var courses = await _courseRepository.GetAllAsync(cancellationToken);
        var response = courses.Select(CourseResponse.FromDomain);
        return Ok(response);
    }
}