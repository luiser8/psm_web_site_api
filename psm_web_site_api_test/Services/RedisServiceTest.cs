using Moq;
using psm_web_site_api_project.Services.Redis;
using StackExchange.Redis;
using Xunit;

namespace psm_web_site_api_test.Services;

public class RedisServiceTest
{
    private readonly IRedisService _redisService;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IDatabase> _databaseMock;
    private readonly Mock<IConnectionMultiplexer> _connectionMultiplexerMock;

    public RedisServiceTest()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(x => x.GetSection("Clients:Redis:host").Value).Returns("localhost");
        _configurationMock.Setup(x => x.GetSection("Clients:Redis:absoluteExpiration").Value).Returns("60");
        _configurationMock.Setup(x => x.GetSection("Clients:Redis:shortExpiration").Value).Returns("5");;

        _connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
        _databaseMock = new Mock<IDatabase>();

        _databaseMock.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue("testValue"));

        _databaseMock.Setup(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);
        _databaseMock.Setup(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);
        _databaseMock.Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        _connectionMultiplexerMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns(_databaseMock.Object);

        _redisService = new RedisService(_configurationMock.Object);
    }

    [Fact]
    public async Task GetData_ReturnsExpectedResult()
    {
        // Arrange

        // Act
        var result = await _redisService.GetData<string>("testKey");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetDataSingle_ReturnsExpectedResult()
    {
        // Arrange

        // Act
        var result = await _redisService.GetDataSingle<string>("testKey");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task SetData_ReturnsTrue()
    {
        // Arrange
        var data = new List<string> { "value1", "value2" };

        // Act
        var result = await _redisService.SetData("testKey", data);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SetDataSingle_ReturnsTrue()
    {
        // Arrange
        var data = "singleValue";

        // Act
        var result = await _redisService.SetDataSingle("testKey", data);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RemoveData_KeyExists_RemovesKey()
    {
        // Arrange
        var key = "testKey";

        // Act
        var result = await _redisService.RemoveData(key);

        // Assert
        Assert.True(result);
    }
}