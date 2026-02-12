using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Sessions;

namespace WeChooz.TechAssessment.Domain.Courses;

public record Course: Entity 
{
    public string Name { get;  set; }
    public string ShortDescription { get;  set; }
    public string LongDescription { get;  set; }
    public int DurationInDays { get;  set; }
    public TargetAudience TargetAudience { get;  set; }
    public int MaxCapacity { get;  set; }
    public string TrainerFirstName { get;  set; }
    public string TrainerLastName { get;  set; }
    
    public ICollection<Session> Sessions { get;  set; } = [];
}