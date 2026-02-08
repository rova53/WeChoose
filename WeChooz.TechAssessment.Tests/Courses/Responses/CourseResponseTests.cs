using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Web.Courses.Responses;

namespace WeChooz.TechAssessment.Tests.Courses.Responses;

public class CourseResponseTests
{
    [Fact]
    public void FromDomain_ShouldMapAllProperties()
    {
        // Arrange
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "C# Avancé",
            ShortDescription = "Formation C# avancée",
            LongDescription = "Une formation complète sur les concepts avancés de C#",
            DurationInDays = 5,
            TargetAudience = TargetAudience.CsePresident,
            MaxCapacity = 20,
            TrainerFirstName = "Jean",
            TrainerLastName = "Dupont",
            Sessions = []
        };

        // Act
        var response = CourseResponse.FromDomain(course);

        // Assert
        Assert.Equal(course.Id, response.Id);
        Assert.Equal(course.Name, response.Name);
        Assert.Equal(course.ShortDescription, response.ShortDescription);
        Assert.Equal(course.LongDescription, response.LongDescription);
        Assert.Equal(course.DurationInDays, response.DurationInDays);
        Assert.Equal(course.TargetAudience, response.TargetAudience);
        Assert.Equal(course.MaxCapacity, response.MaxCapacity);
        Assert.Equal(course.TrainerFirstName, response.TrainerFirstName);
        Assert.Equal(course.TrainerLastName, response.TrainerLastName);
    }
    [Fact]
    public void FromDomain_WithNullSessions_ShouldReturnZeroSessionCount()
    {
        // Arrange
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Formation Azure",
            ShortDescription = "Intro Azure",
            LongDescription = "Formation Azure complète",
            DurationInDays = 2,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 25,
            TrainerFirstName = "Sophie",
            TrainerLastName = "Leroy",
            Sessions = null!
        };

        // Act
        var response = CourseResponse.FromDomain(course);

        // Assert
        Assert.Equal(0, response.SessionCount);
    }
}