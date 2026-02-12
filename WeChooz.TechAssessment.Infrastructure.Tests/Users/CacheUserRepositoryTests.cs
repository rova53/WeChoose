using FluentAssertions;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Infrastructure.Users;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Users;

public class CacheUserRepositoryTests
{
    private readonly IUserRepository _innerRepository;
    private readonly ICacheService _cacheService;
    private readonly CacheUserRepository _sut;

    public CacheUserRepositoryTests()
    {
        _innerRepository = Substitute.For<IUserRepository>();
        _cacheService = Substitute.For<ICacheService>();
        _sut = new CacheUserRepository(_innerRepository, _cacheService);
    }

    private static User CreateUser(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        FirstName = "Pierre",
        LastName = "Martin",
        Email = "pierre@test.com",
        CompanyName = "ACME",
        Password = "hashed",
        Role = PolicyRoles.Formation
    };

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cachedUser = CreateUser(userId);

        _cacheService
            .GetAsync<User>($"Users:{userId}", Arg.Any<CancellationToken>())
            .Returns(cachedUser);

        // Act
        var result = await _sut.GetByIdAsync(userId);

        // Assert
        result.Should().Be(cachedUser);
        await _innerRepository
            .DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);

        _cacheService
            .GetAsync<User>($"Users:{userId}", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        _innerRepository
            .GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.GetByIdAsync(userId);

        // Assert
        result.Should().Be(user);
        await _cacheService
            .Received(1)
            .SetAsync($"Users:{userId}", user, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _cacheService
            .GetAsync<User>($"Users:{userId}", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        _innerRepository
            .GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _sut.GetByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var users = new List<User> { CreateUser(), CreateUser() };

        _cacheService
            .GetAsync<IEnumerable<User>>("Users:all", Arg.Any<CancellationToken>())
            .Returns(users);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(users);
        await _innerRepository
            .DidNotReceive()
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var users = new List<User> { CreateUser(), CreateUser() };

        _cacheService
            .GetAsync<IEnumerable<User>>("Users:all", Arg.Any<CancellationToken>())
            .Returns((IEnumerable<User>?)null);

        _innerRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(users);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(users);
        await _cacheService
            .Received(1)
            .SetAsync("Users:all", users, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_Should_Call_Inner_And_Invalidate_Prefix_Cache()
    {
        // Arrange
        var user = CreateUser();

        _innerRepository
            .AddAsync(user, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.AddAsync(user);

        // Assert
        result.Should().Be(user);
        await _cacheService
            .Received(1)
            .RemoveByPrefixAsync("Users", Arg.Any<CancellationToken>());
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_Should_Call_Inner_And_Invalidate_Cache()
    {
        // Arrange
        var user = CreateUser();

        _innerRepository
            .UpdateAsync(user, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.UpdateAsync(user);

        // Assert
        result.Should().Be(user);
        await _cacheService
            .Received(1)
            .RemoveAsync($"Users:{user.Id}", Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync("Users:all", Arg.Any<CancellationToken>());
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_Should_Call_Inner_And_Invalidate_Cache()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _sut.DeleteAsync(userId);

        // Assert
        await _innerRepository
            .Received(1)
            .DeleteAsync(userId, Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync($"Users:{userId}", Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync("Users:all", Arg.Any<CancellationToken>());
    }

    #endregion

    #region SaveChangesAsync

    [Fact]
    public async Task SaveChangesAsync_Should_Delegate_To_Inner()
    {
        // Arrange
        _innerRepository.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
    }

    #endregion

    #region GetBySessionIdAsync

    [Fact]
    public async Task GetBySessionIdAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var users = new List<User> { CreateUser() }.AsReadOnly();

        _cacheService
            .GetAsync<IEnumerable<User>>($"Users:GetBySessionId{sessionId}", Arg.Any<CancellationToken>())
            .Returns(users);

        // Act
        var result = await _sut.GetBySessionIdAsync(sessionId);

        // Assert
        result.Should().HaveCount(1);
        await _innerRepository
            .DidNotReceive()
            .GetBySessionIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBySessionIdAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var users = new List<User> { CreateUser() }.AsReadOnly();

        _cacheService
            .GetAsync<IEnumerable<User>>($"Users:GetBySessionId{sessionId}", Arg.Any<CancellationToken>())
            .Returns((IEnumerable<User>?)null);

        _innerRepository
            .GetBySessionIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(users);

        // Act
        var result = await _sut.GetBySessionIdAsync(sessionId);

        // Assert
        result.Should().HaveCount(1);
        await _cacheService
            .Received(1)
            .SetAsync($"Users:GetBySessionId{sessionId}", users, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    #endregion

    #region IsUserRegisteredAsync

    [Fact]
    public async Task IsUserRegisteredAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var email = "pierre@test.com";

        _cacheService
            .GetAsync<bool?>($"Users:IsUserRegistered{sessionId}{email}", Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _sut.IsUserRegisteredAsync(sessionId, email);

        // Assert
        result.Should().BeTrue();
        await _innerRepository
            .DidNotReceive()
            .IsUserRegisteredAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task IsUserRegisteredAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var email = "pierre@test.com";

        _cacheService
            .GetAsync<bool?>($"Users:IsUserRegistered{sessionId}{email}", Arg.Any<CancellationToken>())
            .Returns((bool?)null);

        _innerRepository
            .IsUserRegisteredAsync(sessionId, email, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _sut.IsUserRegisteredAsync(sessionId, email);

        // Assert
        result.Should().BeFalse();
        await _cacheService
            .Received(1)
            .SetAsync($"Users:IsUserRegistered{sessionId}{email}", false, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    #endregion

    #region FindByEmail

    [Fact]
    public async Task FindByEmail_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var email = "pierre@test.com";
        var user = CreateUser();

        _cacheService
            .GetAsync<User?>($"Users:FindByEmail{email}", Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.FindByEmail(email);

        // Assert
        result.Should().Be(user);
        await _innerRepository
            .DidNotReceive()
            .FindByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FindByEmail_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var email = "pierre@test.com";
        var user = CreateUser();

        _cacheService
            .GetAsync<User?>($"Users:FindByEmail{email}", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        _innerRepository
            .FindByEmail(email, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.FindByEmail(email);

        // Assert
        result.Should().Be(user);
        await _cacheService
            .Received(1)
            .SetAsync($"Users:FindByEmail{email}", user, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    #endregion
}
