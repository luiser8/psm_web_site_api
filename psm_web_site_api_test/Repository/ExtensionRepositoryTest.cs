using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Extensiones;
using Xunit;

namespace psm_web_site_api_test.Repository;

public class ExtensionesRepositoryTest
{
    private readonly Mock<IMongoCollection<Extension>> _extensionCollectionMock;
    private readonly Mock<IOptions<ConfigDB>> _optionsMock;
    private readonly Mock<IMongoClient> _mongoClientMock;
    private readonly ExtensionesRepository _repository;

    public ExtensionesRepositoryTest()
    {
        // Mock de IOptions<ConfigDB>
        _optionsMock = new Mock<IOptions<ConfigDB>>();
        _optionsMock.Setup(o => o.Value).Returns(new ConfigDB
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "testdb"
        });

        // Mock de IMongoCollection<Extension>
        _extensionCollectionMock = new Mock<IMongoCollection<Extension>>();

        //var tokenMock = JwtUtils.CreateToken(new TokenPayload { IdUsuario = "1", Correo = "user@example.com", Nombres = "John", Apellidos = "Doe", Rol = new Rol { IdRol = "1", Nombre = "Admin" }, Extension = null });

        _mongoClientMock = new Mock<IMongoClient>();
        var mongoDatabaseMock = new Mock<IMongoDatabase>();
        mongoDatabaseMock.Setup(db => db.GetCollection<Extension>("extensions", null)).Returns(_extensionCollectionMock.Object);
        _mongoClientMock.Setup(client => client.GetDatabase(It.IsAny<string>(), null)).Returns(mongoDatabaseMock.Object);

        // Crear instancia real del repositorio
        _repository = new ExtensionesRepository(_optionsMock.Object, _mongoClientMock.Object);

        // Inyectar manualmente el mock en el campo privado _extensionCollection usando Reflection
        var fieldInfo = typeof(ExtensionesRepository)
            .GetField("_extensionCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (fieldInfo != null)
        {
            fieldInfo.SetValue(_repository, _extensionCollectionMock.Object);
        }
        else
        {
            throw new InvalidOperationException("Field '_extensionCollection' not found in ExtensionesRepository.");
        }
    }

    private void SetupRolesListRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Extension>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListExtensions());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        asyncCursorMock
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Mockear el Find para devolver el cursor
        _extensionCollectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Extension>>(),
                It.IsAny<FindOptions<Extension, Extension>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asyncCursorMock.Object);

        var replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);
        _extensionCollectionMock
            .Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Extension>>(),
                It.IsAny<Extension>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(replaceResult);

        var deleteResult = new DeleteResult.Acknowledged(1);
        _extensionCollectionMock
            .Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Extension>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResult);
    }

    private static List<Extension> ListExtensions()
    {
        return new List<Extension>
        {
            new Extension
            {
                IdExtension = "1",
                Nombre = "Extension 1",
                Activo = true,
                FechaCreacion = DateTime.Now
            },
            new Extension
            {
                IdExtension = "2",
                Nombre = "Extension 2",
                Activo = true,
                FechaCreacion = DateTime.Now
            }
        };
    }

    [Fact]
    public async Task SelectExtensionesPorIdRepository_ReturnsAll()
    {
        // Arrange
        SetupRolesListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectExtensionesRepository();

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(result.Count, ListExtensions().Count);
    }

    [Fact]
    public async Task SelectExtensionesPorIdRepository_ReturnsById()
    {
        // Arrange
        const string extensionId = "1";

        SetupRolesListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectExtensionesPorIdRepository(extensionId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(extensionId, result.IdExtension);
    }

    [Fact]
    public async Task SelectExtensionFilterRepository_ReturnsSelectExtensionFilter()
    {
        // Arrange
        const string extensionId = "1";

        SetupRolesListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectExtensionesFilterRepository(extensionId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(extensionId, result.FirstOrDefault().IdExtension);
    }

    [Fact]
    public async Task PostExtensionesRepository_ReturnsTrue()
    {
        // Arrange
        var extension = new Extension
        {
            IdExtension = "1",
            Nombre = "Extension 1",
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        SetupRolesListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.PostExtensionesRepository(extension);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task PutExtensionesRepository_ReturnsTrue()
    {
        // Arrange
        var extension = new Extension
        {
            IdExtension = "1",
            Nombre = "Extension 1",
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        SetupRolesListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.PutExtensionesRepository("1", extension);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteExtensionesRepository_ReturnsTrue()
    {
        // Arrange
        const string extensionId = "1";

        SetupRolesListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.DeleteExtensionesRepository(extensionId);

        // Assert: Verificar el resultado
        Assert.True(result);
    }
}
