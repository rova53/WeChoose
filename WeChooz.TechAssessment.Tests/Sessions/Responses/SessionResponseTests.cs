using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Tests.Sessions.Responses;

public class SessionResponseTests
{
    [Fact]
    public void FromDomain_ShouldMapAllProperties()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var session = new Session
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            StarDate = new DateOnly(2026, 6, 15),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = courseId,
                Name = "C# Avancé",
                ShortDescription = "C#",
                LongDescription = "C# complet",
                DurationInDays = 5,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 20,
                TrainerFirstName = "Jean",
                TrainerLastName = "Dupont"
            },
            Users = new List<User>
            {
                new() { Id = Guid.NewGuid(), SessionId = Guid.NewGuid(), LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" },
                new() { Id = Guid.NewGuid(), SessionId = Guid.NewGuid(), LastName = "P2", FirstName = "F2", Email = "p2@test.com", CompanyName = "C2" }
            }
        };

        // Act
        var response = SessionResponse.FromDomain(session);

        // Assert
        Assert.Equal(session.Id, response.Id);
        Assert.Equal(session.CourseId, response.CourseId);
        Assert.Equal("C# Avancé", response.CourseName);
        Assert.Equal(session.StarDate, response.StartDate);
        Assert.Equal(session.DeliveryMode, response.DeliveryMode);
        Assert.Equal(2, response.UserCount);
    }

    [Fact]
    public void FromDomain_WithNullCourse_ShouldReturnEmptyCourseName()
    {
        // Arrange
        var session = new Session
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.InPerson,
            Course = null!,
            Users = []
        };

        // Act
        var response = SessionResponse.FromDomain(session);

        // Assert
        Assert.Equal(string.Empty, response.CourseName);
    }

    [Fact]
    public void FromDomain_WithNullUsers_ShouldReturnZeroUserCount()
    {
        // Arrange
        var session = new Session
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            StarDate = new DateOnly(2026, 4, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                ShortDescription = "Test",
                LongDescription = "Test",
                DurationInDays = 1,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 10,
                TrainerFirstName = "A",
                TrainerLastName = "B"
            },
            Users = null!
        };

        // Act
        var response = SessionResponse.FromDomain(session);

        // Assert
        Assert.Equal(0, response.UserCount);
    }

    [Fact]
    public void FromDomain_WithEmptyUsers_ShouldReturnZeroUserCount()
    {
        // Arrange
        var session = new Session
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            StarDate = new DateOnly(2026, 5, 1),
            DeliveryMode = DeliveryMode.InPerson,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Formation",
                ShortDescription = "Test",
                LongDescription = "Test",
                DurationInDays = 2,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 15,
                TrainerFirstName = "A",
                TrainerLastName = "B"
            },
            Users = []
        };

        // Act
        var response = SessionResponse.FromDomain(session);

        // Assert
        Assert.Equal(0, response.UserCount);
    }

    [Fact]
    public void FromDomain_WithDefaultGuid_ShouldMapCorrectly()
    {
        // Arrange
        var session = new Session
        {
            Id = Guid.Empty,
            CourseId = Guid.Empty,
            StarDate = default,
            DeliveryMode = DeliveryMode.Remote,
            Course = null!,
            Users = []
        };

        // Act
        var response = SessionResponse.FromDomain(session);

        // Assert
        Assert.Equal(Guid.Empty, response.Id);
        Assert.Equal(Guid.Empty, response.CourseId);
        Assert.Equal(default, response.StartDate);
    }

    [Fact]
    public void FromDomain_ShouldReturnNewInstance()
    {
        // Arrange
        var session = new Session
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            StarDate = new DateOnly(2026, 7, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                ShortDescription = "Test",
                LongDescription = "Test",
                DurationInDays = 1,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 10,
                TrainerFirstName = "A",
                TrainerLastName = "B"
            },
            Users = []
        };

        // Act
        var response1 = SessionResponse.FromDomain(session);
        var response2 = SessionResponse.FromDomain(session);

        // Assert
        Assert.NotSame(response1, response2);
        Assert.Equal(response1.Id, response2.Id);
        Assert.Equal(response1.CourseId, response2.CourseId);
        Assert.Equal(response1.CourseName, response2.CourseName);
        Assert.Equal(response1.StartDate, response2.StartDate);
        Assert.Equal(response1.DeliveryMode, response2.DeliveryMode);
        Assert.Equal(response1.UserCount, response2.UserCount);
    }

    [Theory]
    [InlineData(DeliveryMode.Remote)]
    [InlineData(DeliveryMode.InPerson)]
    public void FromDomain_ShouldMapAllDeliveryModeValues(DeliveryMode deliveryMode)
    {
        // Arrange
        var session = new Session
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            StarDate = new DateOnly(2026, 8, 1),
            DeliveryMode = deliveryMode,
            Course = null!,
            Users = []
        };

        // Act
        var response = SessionResponse.FromDomain(session);

        // Assert
        Assert.Equal(deliveryMode, response.DeliveryMode);
    }
}