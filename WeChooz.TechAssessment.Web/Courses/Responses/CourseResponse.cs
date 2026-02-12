using WeChooz.TechAssessment.Domain.Courses;

namespace WeChooz.TechAssessment.Web.Courses.Responses;

public class CourseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public TargetAudience TargetAudience { get; set; }
    public int MaxCapacity { get; set; }
    public string TrainerFirstName { get; set; } = string.Empty;
    public string TrainerLastName { get; set; } = string.Empty;
    public int SessionCount { get; set; }

    public static CourseResponse FromDomain(Course course) => new()
    {
        Id = course.Id,
        Name = course.Name,
        ShortDescription = course.ShortDescription,
        LongDescription = course.LongDescription,
        DurationInDays = course.DurationInDays,
        TargetAudience = course.TargetAudience,
        MaxCapacity = course.MaxCapacity,
        TrainerFirstName = course.TrainerFirstName,
        TrainerLastName = course.TrainerLastName,
        SessionCount = course.Sessions?.Count ?? 0
    };
}