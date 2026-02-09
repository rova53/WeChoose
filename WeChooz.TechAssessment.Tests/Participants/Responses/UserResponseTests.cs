using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Tests.Users.Responses;

public class UserResponseTests
{
    [Fact]
    public void FromDomain_ShouldMapAllProperties()
    {
        // Arrange
        var User = new User
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean.dupont@email.com",
            CompanyName = "Acme Corp"
        };

        // Act
        var response = UserResponse.FromDomain(User);

        // Assert
        Assert.Equal(User.Id, response.Id);
        Assert.Equal(User.SessionId, response.SessionId);
        Assert.Equal(User.LastName, response.LastName);
        Assert.Equal(User.FirstName, response.FirstName);
        Assert.Equal(User.Email, response.Email);
        Assert.Equal(User.CompanyName, response.CompanyName);
    }

    [Fact]
    public void FromDomain_WithEmptyStrings_ShouldMapCorrectly()
    {
        // Arrange
        var User = new User
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = string.Empty,
            FirstName = string.Empty,
            Email = string.Empty,
            CompanyName = string.Empty
        };

        // Act
        var response = UserResponse.FromDomain(User);

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
        var User = new User
        {
            Id = Guid.Empty,
            SessionId = Guid.Empty,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        // Act
        var response = UserResponse.FromDomain(User);

        // Assert
        Assert.Equal(Guid.Empty, response.Id);
        Assert.Equal(Guid.Empty, response.SessionId);
    }

    [Fact]
    public void FromDomain_ShouldReturnNewInstance()
    {
        // Arrange
        var User = new User
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Martin",
            FirstName = "Marie",
            Email = "marie.martin@email.com",
            CompanyName = "Tech SA"
        };

        // Act
        var response1 = UserResponse.FromDomain(User);
        var response2 = UserResponse.FromDomain(User);

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
    public void FromDomain_ShouldNotModifySourceUser()
    {
        // Arrange
        var originalId = Guid.NewGuid();
        var originalSessionId = Guid.NewGuid();
        var User = new User
        {
            Id = originalId,
            SessionId = originalSessionId,
            LastName = "Leroy",
            FirstName = "Pierre",
            Email = "pierre.leroy@email.com",
            CompanyName = "Dev Inc"
        };

        // Act
        var response = UserResponse.FromDomain(User);

        // Assert
        Assert.Equal(originalId, User.Id);
        Assert.Equal(originalSessionId, User.SessionId);
        Assert.Equal("Leroy", User.LastName);
        Assert.Equal("Pierre", User.FirstName);
        Assert.Equal("pierre.leroy@email.com", User.Email);
        Assert.Equal("Dev Inc", User.CompanyName);
    }

    [Theory]
    [InlineData("nom@domaine.com")]
    [InlineData("prenom.nom@entreprise.fr")]
    [InlineData("user+tag@example.org")]
    public void FromDomain_WithVariousEmails_ShouldMapCorrectly(string email)
    {
        // Arrange
        var User = new User
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = email,
            CompanyName = "Test"
        };

        // Act
        var response = UserResponse.FromDomain(User);

        // Assert
        Assert.Equal(email, response.Email);
    }
}