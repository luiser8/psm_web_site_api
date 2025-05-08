using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Auditorias;
using Xunit;

namespace psm_web_site_api_test.Repository;

public class AuditoriaRepositoryTest
{
    private readonly Mock<IMongoCollection<Auditoria>> _auditoriaCollectionMock;
    private readonly Mock<IOptions<ConfigDB>> _optionsMock;
    private readonly Mock<IMongoClient> _mongoClientMock;
    private readonly AuditoriasRepository _repository;

    public AuditoriaRepositoryTest()
    {
        // Mock de IOptions<ConfigDB>
        _optionsMock = new Mock<IOptions<ConfigDB>>();
        _optionsMock.Setup(o => o.Value).Returns(new ConfigDB
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "testdb"
        });

        // Mock de IMongoCollection<Auditoria>
        _auditoriaCollectionMock = new Mock<IMongoCollection<Auditoria>>();

        _mongoClientMock = new Mock<IMongoClient>();
        var mongoDatabaseMock = new Mock<IMongoDatabase>();
        mongoDatabaseMock.Setup(db => db.GetCollection<Auditoria>("auditorias", null)).Returns(_auditoriaCollectionMock.Object);
        _mongoClientMock.Setup(client => client.GetDatabase(It.IsAny<string>(), null)).Returns(mongoDatabaseMock.Object);

        // Crear instancia real del repositorio
        _repository = new AuditoriasRepository(_optionsMock.Object, _mongoClientMock.Object);

        // Inyectar manualmente el mock en el campo privado _auditoriaCollection usando Reflection
        var fieldInfo = typeof(AuditoriasRepository)
            .GetField("_auditoriaCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (fieldInfo != null)
        {
            fieldInfo.SetValue(_repository, _auditoriaCollectionMock.Object);
        }
        else
        {
            throw new InvalidOperationException("Field '_auditoriaCollection' not found in AuditoriasRepository.");
        }
    }

    private void SetupAuditoriaListRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Auditoria>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListAuditorias());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        asyncCursorMock
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Mockear el Find para devolver el cursor
        _auditoriaCollectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Auditoria>>(),
                It.IsAny<FindOptions<Auditoria, Auditoria>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asyncCursorMock.Object);
    }

    private static List<Auditoria> ListAuditorias()
    {
        return
        new List<Auditoria>
        {
            new Auditoria
            {
                IdAuditoria = "1",
                IdUsuario = "1",
                Accion = "Accion 1",
                Fecha = DateTime.Now,
            },
            new Auditoria
            {
                IdAuditoria = "1",
                IdUsuario = "1",
                Accion = "Accion 1",
                Fecha = DateTime.Now,
            }
        };
    }

    [Fact]
    public async Task SelectAuditoriaRepository_ReturnsList()
    {
        // Arrange
        const string usuarioId = "1";
        SetupAuditoriaListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectAuditoriasPorUsuarioIdRepository(usuarioId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(result.Count, ListAuditorias().Count);
    }

    [Fact]
    public async Task SelectRolesPorIdRepository_ReturnsRolesById()
    {
        // Arrange
        const string usuarioId = "1";

        SetupAuditoriaListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectAuditoriasPorUsuarioIdRepository(usuarioId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        var firstResult = result.FirstOrDefault();
        Assert.NotNull(firstResult);
        Assert.Equal(firstResult.IdUsuario, usuarioId);
    }

    [Fact]
    public async Task SelectRolesFilterRepository_ReturnsSelectRolesFilter()
    {
        // Arrange
        const string usuarioId = "1";

        SetupAuditoriaListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectAuditoriasPorUsuarioIdRepository(usuarioId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        var firstResult = result.FirstOrDefault();
        Assert.NotNull(firstResult);
        Assert.Equal(firstResult.IdUsuario, usuarioId);
    }

    [Fact]
    public async Task PostAuditoriaRepository_ReturnsAuditoriaCreado()
    {
        // Arrange
        var auditoriaPayloadMock = new Auditoria
        {
            IdAuditoria = "1",
            IdUsuario = "1",
            Accion = "Accion 1",
            Fecha = DateTime.Now,
            Activo = true,
        };

        SetupAuditoriaListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.PostAuditoriasRepository(auditoriaPayloadMock);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.True(result);
    }

        [Fact]
    public async Task DeleteAuditoriaRepository_ReturnsAuditoria()
    {
        // Arrange
        var idAuditoria = "1";

        SetupAuditoriaListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.DeleteAuditoriasRepository(idAuditoria);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.True(result);
    }
}
