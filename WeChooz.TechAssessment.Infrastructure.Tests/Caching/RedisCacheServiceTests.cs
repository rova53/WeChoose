using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using WeChooz.TechAssessment.Infrastructure.Caching;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Caching;

public class RedisCacheServiceTests
{
    private readonly IDistributedCache _distributedCache;
    private readonly RedisCacheService _sut;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public RedisCacheServiceTests()
    {
        _distributedCache = Substitute.For<IDistributedCache>();
        _sut = new RedisCacheService(_distributedCache);
    }

    private record TestEntity(Guid Id, string Name);

    [Fact]
    public async Task GetAsync_Should_Return_Default_When_Key_Not_Found()
    {
        // Arrange
        _distributedCache
            .GetAsync("missing-key", Arg.Any<CancellationToken>())
            .Returns((byte[]?)null);

        // Act
        var result = await _sut.GetAsync<TestEntity>("missing-key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_Should_Deserialize_Value_When_Key_Exists()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), "Test");
        var json = JsonSerializer.Serialize(entity, JsonOptions);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        _distributedCache
            .GetAsync("test-key", Arg.Any<CancellationToken>())
            .Returns(bytes);

        // Act
        var result = await _sut.GetAsync<TestEntity>("test-key");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(entity.Id);
        result.Name.Should().Be(entity.Name);
    }

    [Fact]
    public async Task SetAsync_Should_Serialize_And_Store_Value()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), "Test");

        // Act
        await _sut.SetAsync("test-key", entity, TimeSpan.FromMinutes(10));

        // Assert
        await _distributedCache
            .Received(1)
            .SetAsync(
                "test-key",
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o =>
                    o.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(10)),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAsync_Should_Use_Default_Expiration_When_Not_Specified()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid(), "Test");

        // Act
        await _sut.SetAsync("test-key", entity);

        // Assert
        await _distributedCache
            .Received(1)
            .SetAsync(
                "test-key",
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o =>
                    o.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(5)),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAsync_Should_Remove_Key()
    {
        // Act
        await _sut.RemoveAsync("test-key");

        // Assert
        await _distributedCache
            .Received(1)
            .RemoveAsync("test-key", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveByPrefixAsync_Should_Throw_NotImplementedException()
    {
        // Act
        var act = () => _sut.RemoveByPrefixAsync("prefix");

        // Assert
        await act.Should().ThrowAsync<NotImplementedException>();
    }
}
