using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Web.Courses.Requests;
using WeChooz.TechAssessment.Web.Courses.Responses;

namespace WeChooz.TechAssessment.Web.Courses;

[Route("_api/course")]
public class CreateCourseEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<CreateCourseRequest>
    .WithActionResult<CourseResponse>
{
    private readonly ICourseRepository _courseRepository;
    public CreateCourseEndpoint(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    [HttpPost]
    public override async Task<ActionResult<CourseResponse>> 
        HandleAsync([FromBody] CreateCourseRequest request, 
            CancellationToken cancellationToken = default)
    {
        var course = new Course
        {
            Name = request.Name,
            ShortDescription = request.ShortDescription,
            LongDescription = request.LongDescription,
            DurationInDays = request.DurationInDays,
            TargetAudience = request.TargetAudience,
            MaxCapacity = request.MaxCapacity,
            TrainerFirstName = request.TrainerFirstName,
            TrainerLastName = request.TrainerLastName
        };

        var created = await _courseRepository.AddAsync(course, cancellationToken);
        return Created($"_api/course/{created.Id}", CourseResponse.FromDomain(created));
    }
}