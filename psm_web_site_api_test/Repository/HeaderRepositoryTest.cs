using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Headers;
using Xunit;

namespace psm_web_site_api_test.Repository;

public class HeaderRepositoryTest
{
    private readonly Mock<IMongoCollection<Header>> _headerCollectionMock;
    private readonly Mock<IOptions<ConfigDB>> _optionsMock;
    private readonly Mock<IMongoClient> _mongoClientMock;
    private readonly HeaderRepository _repository;

    public HeaderRepositoryTest()
    {
        // Mock de IOptions<ConfigDB>
        _optionsMock = new Mock<IOptions<ConfigDB>>();
        _optionsMock.Setup(o => o.Value).Returns(new ConfigDB
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "testdb"
        });

        // Mock de IMongoCollection<Rol>
        _headerCollectionMock = new Mock<IMongoCollection<Header>>();

        _mongoClientMock = new Mock<IMongoClient>();
        var mongoDatabaseMock = new Mock<IMongoDatabase>();
        mongoDatabaseMock.Setup(db => db.GetCollection<Header>("header", null)).Returns(_headerCollectionMock.Object);
        _mongoClientMock.Setup(client => client.GetDatabase(It.IsAny<string>(), null)).Returns(mongoDatabaseMock.Object);

        // Crear instancia real del repositorio
        _repository = new HeaderRepository(_optionsMock.Object, _mongoClientMock.Object);

        // Inyectar manualmente el mock en el campo privado _headerCollection usando Reflection
        var fieldInfo = typeof(HeaderRepository)
            .GetField("_headerCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (fieldInfo != null)
        {
            fieldInfo.SetValue(_repository, _headerCollectionMock.Object);
        }
        else
        {
            throw new InvalidOperationException("Field '_headerCollection' not found in HeaderRepository.");
        }
    }

    private void SetupHeaderListRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Header>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListHeader());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        asyncCursorMock
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Mockear el Find para devolver el cursor
        _headerCollectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Header>>(),
                It.IsAny<FindOptions<Header, Header>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asyncCursorMock.Object);

        var replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);
        _headerCollectionMock
            .Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Header>>(),
                It.IsAny<Header>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(replaceResult);

        var updateResult = new UpdateResult.Acknowledged(1, 1, null);
        _headerCollectionMock
            .Setup(x => x.UpdateOneAsync(
                It.IsAny<FilterDefinition<Header>>(),
                It.IsAny<UpdateDefinition<Header>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateResult);

        var deleteResult = new DeleteResult.Acknowledged(1);
        _headerCollectionMock
            .Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Header>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResult);
    }

    private static List<Header> ListHeader()
    {
        return
        [
            new Header
            {
                IdHeader = "1",
                EsNacional = true,
                IdExtension = "1",
                HeaderCollections = [
                    new() {
                        Activo = true,
                        IdHeaderCollection = "1",
                        Link = "/link",
                        Nombre = "test",
                        Target = true,
                    }
                ],
                Activo = true,
                FechaCreacion = DateTime.Now
            }
        ];
    }

    [Fact]
    public async Task SelectHeaderRepository_ReturnsListByIdHeader()
    {
        // Arrange
        var idHeader = "1";
        SetupHeaderListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectHeaderPorIdRepository(idHeader);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(result.IdHeader, ListHeader()[0].IdHeader);
    }

    [Fact]
    public async Task SelectHeaderRepository_ReturnsListByIdExtension()
    {
        // Arrange
        var idExtension = "1";
        SetupHeaderListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectHeaderPorIdExtensionRepository(idExtension);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(result.IdExtension, ListHeader()[0].IdExtension);
    }

    [Fact]
    public async Task PostHeaderRepository_ReturnsTrue()
    {
        // Arrange
        var header = new Header
        {
            IdHeader = "1",
            EsNacional = true,
            IdExtension = "1",
            HeaderCollections = [
                    new() {
                        Activo = true,
                        IdHeaderCollection = "1",
                        Link = "/link",
                        Nombre = "test",
                        Target = true,
                    }
                ],
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        SetupHeaderListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.PostHeaderRepository(header);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task PutHeaderRepository_ReturnsTrue()
    {
        // Arrange
        var header = new Header
        {
            IdHeader = "1",
            EsNacional = true,
            IdExtension = "1",
            HeaderCollections = [
                    new() {
                        Activo = true,
                        IdHeaderCollection = "1",
                        Link = "/link",
                        Nombre = "test",
                        Target = true,
                    }
                ],
            Activo = true,
            FechaCreacion = DateTime.Now
        };
        SetupHeaderListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.PutHeaderRepository("1", header);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteHeaderRepository_ReturnsTrue()
    {
        // Arrange
        const string headerId = "1";

        SetupHeaderListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.DeleteHeaderRepository(headerId);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task AddItemToHeaderRepository_ReturnsTrue()
    {
        // Arrange
        var idExtension = "1";
        var header = new HeaderCollection
        {
            Activo = true,
            Nombre = "test",
            Target = true,
            Link = "/link",
            IdHeaderCollection = "1"
        };

        SetupHeaderListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.AddItemToHeader(idExtension, header);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task RemoveItemFromHeaderRepository_ReturnsTrue()
    {
        // Arrange
        var idExtension = "1";
        var itemNombreToRemove = "1";

        SetupHeaderListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.RemoveItemFromHeader(idExtension, itemNombreToRemove);

        // Assert: Verificar el resultado
        Assert.True(result);
    }
}