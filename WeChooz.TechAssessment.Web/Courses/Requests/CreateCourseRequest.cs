using System.Diagnostics.CodeAnalysis;
using WeChooz.TechAssessment.Domain.Courses;

namespace WeChooz.TechAssessment.Web.Courses.Requests;
[ExcludeFromCodeCoverage]
public class CreateCourseRequest
{
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public int DurationInDays { get; set; }
    public TargetAudience TargetAudience { get; set; }
    public int MaxCapacity { get; set; }
    public string TrainerFirstName { get; set; }
    public string TrainerLastName { get; set; }
}