using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Web.Courses.Requests;
using WeChooz.TechAssessment.Web.Courses.Responses;

namespace WeChooz.TechAssessment.Web.Courses;
[Route("_api/course")]
public class UpdateCourseEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<UpdateCourseRequest>
    .WithActionResult<CourseResponse>
{
    private readonly ICourseRepository _courseRepository;

    public UpdateCourseEndpoint(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    [HttpPut("{id:guid}")]
    public override async Task<ActionResult<CourseResponse>> HandleAsync(
        [FromBody] UpdateCourseRequest request, 
        CancellationToken cancellationToken = default)
    {
        var existing = await _courseRepository
            .GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
            return NotFound();

        var updated = existing with
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

        var result = await _courseRepository.UpdateAsync(updated, cancellationToken);
        return Ok(CourseResponse.FromDomain(result));
    }
}