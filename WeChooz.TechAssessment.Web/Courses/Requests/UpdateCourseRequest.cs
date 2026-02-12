using System.Diagnostics.CodeAnalysis;
using WeChooz.TechAssessment.Domain.Courses;

namespace WeChooz.TechAssessment.Web.Courses.Requests;
[ExcludeFromCodeCoverage]
public class UpdateCourseRequest
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string ShortDescription { get; set; }
    public required string LongDescription { get; set; }
    public required int DurationInDays { get; set; }
    public required TargetAudience TargetAudience { get; set; }
    public required int MaxCapacity { get; set; }
    public required string TrainerFirstName { get; set; }
    public required string TrainerLastName { get; set; }
}