using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Tests.Sessions.Responses;

public class SessionEnrollResponseTests
{
    [Fact]
    public void FromDomain_Should_Map_All_Properties()
    {
        // Arrange
        var enrollmentDate = DateTime.UtcNow;
        var enroll = new SessionEnroll
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            EnrollmentDate = enrollmentDate
        };

        // Act
        var response = SessionEnrollResponse.FromDomain(enroll);

        // Assert
        Assert.Equal(enroll.Id, response.Id);
        Assert.Equal(enroll.SessionId, response.SessionId);
        Assert.Equal(enroll.EnrollmentDate, response.EnrollmentDate);
    }
}
