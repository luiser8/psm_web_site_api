using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Roles;
using Xunit;

namespace psm_web_site_api_test.Repository;

public class RolesRepositoryTest
{
    private readonly Mock<IMongoCollection<Rol>> _rolesCollectionMock;
    private readonly Mock<IOptions<ConfigDB>> _optionsMock;
    private readonly Mock<IMongoClient> _mongoClientMock;
    private readonly RolesRepository _repository;
    
    public RolesRepositoryTest()
    {
        // Mock de IOptions<ConfigDB>
        _optionsMock = new Mock<IOptions<ConfigDB>>();
        _optionsMock.Setup(o => o.Value).Returns(new ConfigDB
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "testdb"
        });

        // Mock de IMongoCollection<Rol>
        _rolesCollectionMock = new Mock<IMongoCollection<Rol>>();

        //var tokenMock = JwtUtils.CreateToken(new TokenPayload { IdUsuario = "1", Correo = "user@example.com", Nombres = "John", Apellidos = "Doe", Rol = new Rol { IdRol = "1", Nombre = "Admin" }, Extension = null });

        _mongoClientMock = new Mock<IMongoClient>();
        var mongoDatabaseMock = new Mock<IMongoDatabase>();
        mongoDatabaseMock.Setup(db => db.GetCollection<Rol>("roles", null)).Returns(_rolesCollectionMock.Object);
        _mongoClientMock.Setup(client => client.GetDatabase(It.IsAny<string>(), null)).Returns(mongoDatabaseMock.Object);

        // Crear instancia real del repositorio
        _repository = new RolesRepository(_optionsMock.Object, _mongoClientMock.Object);

        // Inyectar manualmente el mock en el campo privado _usuariosCollection usando Reflection
        var fieldInfo = typeof(RolesRepository)
            .GetField("_rolCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (fieldInfo != null)
        {
            fieldInfo.SetValue(_repository, _rolesCollectionMock.Object);
        }
        else
        {
            throw new InvalidOperationException("Field '_rolCollection' not found in RolesRepository.");
        }
    }
    
    private void SetupRolesListRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Rol>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListRoles());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        asyncCursorMock
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Mockear el Find para devolver el cursor
        _rolesCollectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Rol>>(),
                It.IsAny<FindOptions<Rol, Rol>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asyncCursorMock.Object);
    }
    
    private static List<Rol> ListRoles()
    {
        return
        [
            new Rol
            {
                IdRol = "1",
                Nombre = "Rol 1",
                Activo = true,
                FechaCreacion = DateTime.Now
            }
        ];
    }
    
    [Fact]
    public async Task SelectRolesRepository_ReturnsList()
    {
        // Arrange
        SetupRolesListRepositoryMocks();
        
        // Act: Ejecutar el método
        var result = await _repository.SelectRolesRepository();

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(result.Count, ListRoles().Count);
    }
    
    [Fact]
    public async Task SelectRolesPorIdRepository_ReturnsRolesById()
    {
        // Arrange
        const string rolId = "1";

        SetupRolesListRepositoryMocks();
        
        // Act: Ejecutar el método
        var result = await _repository.SelectRolRepository(rolId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(rolId, result.IdRol);
    }
    
    [Fact]
    public async Task SelectRolesFilterRepository_ReturnsSelectRolesFilter()
    {
        // Arrange
        const string rolId = "1";

        SetupRolesListRepositoryMocks();
        
        // Act: Ejecutar el método
        var result = await _repository.SelectRolesFilterRepository(rolId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(rolId, result.FirstOrDefault().IdRol);
    }
}
