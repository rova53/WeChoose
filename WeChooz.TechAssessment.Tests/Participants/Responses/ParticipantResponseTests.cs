using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Web.Participants.Responses;

namespace WeChooz.TechAssessment.Tests.Participants.Responses;

public class ParticipantResponseTests
{
    [Fact]
    public void FromDomain_ShouldMapAllProperties()
    {
        // Arrange
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean.dupont@email.com",
            CompanyName = "Acme Corp"
        };

        // Act
        var response = ParticipantResponse.FromDomain(participant);

        // Assert
        Assert.Equal(participant.Id, response.Id);
        Assert.Equal(participant.SessionId, response.SessionId);
        Assert.Equal(participant.LastName, response.LastName);
        Assert.Equal(participant.FirstName, response.FirstName);
        Assert.Equal(participant.Email, response.Email);
        Assert.Equal(participant.CompanyName, response.CompanyName);
    }

    [Fact]
    public void FromDomain_WithEmptyStrings_ShouldMapCorrectly()
    {
        // Arrange
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = string.Empty,
            FirstName = string.Empty,
            Email = string.Empty,
            CompanyName = string.Empty
        };

        // Act
        var response = ParticipantResponse.FromDomain(participant);

        // Assert
        Assert.Equal(string.Empty, response.LastName);
        Assert.Equal(string.Empty, response.FirstName);
        Assert.Equal(string.Empty, response.Email);
        Assert.Equal(string.Empty, response.CompanyName);
    }

    [Fact]
    public void FromDomain_WithDefaultGuid_ShouldMapCorrectly()
    {
        // Arrange
        var participant = new Participant
        {
            Id = Guid.Empty,
            SessionId = Guid.Empty,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        // Act
        var response = ParticipantResponse.FromDomain(participant);

        // Assert
        Assert.Equal(Guid.Empty, response.Id);
        Assert.Equal(Guid.Empty, response.SessionId);
    }

    [Fact]
    public void FromDomain_ShouldReturnNewInstance()
    {
        // Arrange
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Martin",
            FirstName = "Marie",
            Email = "marie.martin@email.com",
            CompanyName = "Tech SA"
        };

        // Act
        var response1 = ParticipantResponse.FromDomain(participant);
        var response2 = ParticipantResponse.FromDomain(participant);

        // Assert
        Assert.NotSame(response1, response2);
        Assert.Equal(response1.Id, response2.Id);
        Assert.Equal(response1.SessionId, response2.SessionId);
        Assert.Equal(response1.LastName, response2.LastName);
        Assert.Equal(response1.FirstName, response2.FirstName);
        Assert.Equal(response1.Email, response2.Email);
        Assert.Equal(response1.CompanyName, response2.CompanyName);
    }

    [Fact]
    public void FromDomain_ShouldNotModifySourceParticipant()
    {
        // Arrange
        var originalId = Guid.NewGuid();
        var originalSessionId = Guid.NewGuid();
        var participant = new Participant
        {
            Id = originalId,
            SessionId = originalSessionId,
            LastName = "Leroy",
            FirstName = "Pierre",
            Email = "pierre.leroy@email.com",
            CompanyName = "Dev Inc"
        };

        // Act
        var response = ParticipantResponse.FromDomain(participant);

        // Assert
        Assert.Equal(originalId, participant.Id);
        Assert.Equal(originalSessionId, participant.SessionId);
        Assert.Equal("Leroy", participant.LastName);
        Assert.Equal("Pierre", participant.FirstName);
        Assert.Equal("pierre.leroy@email.com", participant.Email);
        Assert.Equal("Dev Inc", participant.CompanyName);
    }

    [Theory]
    [InlineData("nom@domaine.com")]
    [InlineData("prenom.nom@entreprise.fr")]
    [InlineData("user+tag@example.org")]
    public void FromDomain_WithVariousEmails_ShouldMapCorrectly(string email)
    {
        // Arrange
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = email,
            CompanyName = "Test"
        };

        // Act
        var response = ParticipantResponse.FromDomain(participant);

        // Assert
        Assert.Equal(email, response.Email);
    }
}