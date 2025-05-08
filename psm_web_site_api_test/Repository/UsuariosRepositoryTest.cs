using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Usuarios;
using Xunit;

namespace psm_web_site_api_test.Repository;

public class UsuariosRepositoryTest
{
    private readonly Mock<IMongoCollection<Usuario>> _usuariosCollectionMock;
    private readonly Mock<IOptions<ConfigDB>> _optionsMock;
    private readonly Mock<IMongoClient> _mongoClientMock;
    private readonly UsuariosRepository _repository;

    public UsuariosRepositoryTest()
    {
        // Mock de IOptions<ConfigDB>
        _optionsMock = new Mock<IOptions<ConfigDB>>();
        _optionsMock.Setup(o => o.Value).Returns(new ConfigDB
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "testdb"
        });

        // Mock de IMongoCollection<Usuario>
        _usuariosCollectionMock = new Mock<IMongoCollection<Usuario>>();

        //var tokenMock = JwtUtils.CreateToken(new TokenPayload { IdUsuario = "1", Correo = "user@example.com", Nombres = "John", Apellidos = "Doe", Rol = new Rol { IdRol = "1", Nombre = "Admin" }, Extension = null });

        _mongoClientMock = new Mock<IMongoClient>();
        var mongoDatabaseMock = new Mock<IMongoDatabase>();
        mongoDatabaseMock.Setup(db => db.GetCollection<Usuario>("usuarios", null)).Returns(_usuariosCollectionMock.Object);
        _mongoClientMock.Setup(client => client.GetDatabase(It.IsAny<string>(), null)).Returns(mongoDatabaseMock.Object);

        // Crear instancia real del repositorio
        _repository = new UsuariosRepository(_optionsMock.Object, _mongoClientMock.Object);

        // Inyectar manualmente el mock en el campo privado _usuariosCollection usando Reflection
        var fieldInfo = typeof(UsuariosRepository)
            .GetField("_usuariosCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (fieldInfo != null)
        {
            fieldInfo.SetValue(_repository, _usuariosCollectionMock.Object);
        }
        else
        {
            throw new InvalidOperationException("Field '_usuariosCollection' not found in UsuariosRepository.");
        }
    }

    private static List<Usuario> ListUsers()
    {
        return
        [
            new Usuario
            {
                IdUsuario = "1",
                Correo = "user@example.com",
                Contrasena = psm_web_site_api_project.Utils.Md5utils.Md5utilsClass.GetMd5("password123"),
                Nombres = "John",
                Apellidos = "Doe",
                Activo = true,
                Rol = new Rol { IdRol = "1", Nombre = "Admin" },
                Extension =
                [
                    new Extension { IdExtension = "1", Nombre = "Ext1" },
                    new Extension { IdExtension = "2", Nombre = "Ext2" }
                ],
            }
        ];
    }

    private void SetupUsuariosListRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Usuario>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListUsers());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        asyncCursorMock
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Mockear el Find para devolver el cursor
        _usuariosCollectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Usuario>>(),
                It.IsAny<FindOptions<Usuario, Usuario>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asyncCursorMock.Object);

        var replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);
        _usuariosCollectionMock
            .Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Usuario>>(),
                It.IsAny<Usuario>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(replaceResult);

        var updateOneResult = new UpdateResult.Acknowledged(1, 1, null);
        _usuariosCollectionMock
            .Setup(x => x.UpdateOneAsync(
                It.IsAny<FilterDefinition<Usuario>>(),
                It.IsAny<UpdateDefinition<Usuario>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateOneResult);
    }

    [Fact]
    public async Task SelectUsuariosRepository_ReturnsList()
    {
        // Arrange
        var usuarioMock = ListUsers();

        SetupUsuariosListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectUsuariosRepository();

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(result.Count, usuarioMock.Count);
    }

    [Fact]
    public async Task SelectUsuariosPorIdRepository_ReturnsUsuarioById()
    {
        // Arrange
        const string usuarioId = "1";

        SetupUsuariosListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectUsuariosPorIdRepository(usuarioId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(usuarioId, result.IdUsuario);
    }

    [Fact]
    public async Task SelectUsuariosPorCorreoRepository_ReturnsUsuarioByCorreo()
    {
        // Arrange
        const string usuarioCorreo = "user@example.com";

        SetupUsuariosListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectUsuariosPorCorreoRepository(usuarioCorreo);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(usuarioCorreo, result.Correo);
    }

    [Fact]
    public async Task PostUsuariosRepository_ReturnsUsuarioCreado()
    {
        // Arrange
        var usuarioPayloadMock = new Usuario
        {
            IdUsuario = "1",
            Correo = "user@example.com",
            Contrasena = psm_web_site_api_project.Utils.Md5utils.Md5utilsClass.GetMd5("password123"),
            Nombres = "John",
            Apellidos = "Doe",
            Activo = true,
            Rol = new Rol { IdRol = "1", Nombre = "Admin" },
            Extension =
            [
                new Extension { IdExtension = "1", Nombre = "Ext1" },
                new Extension { IdExtension = "2", Nombre = "Ext2" }
            ],
        };

        SetupUsuariosListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.PostUsuariosRepository(usuarioPayloadMock);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(usuarioPayloadMock.Correo, result.Correo);
    }

    [Fact]
    public async Task PutUsuariosRepository_ReturnsUsuarioActualizado()
    {
        // Arrange
        var usuarioId = "1";
        var usuarioPayloadMock = new Usuario
        {
            IdUsuario = "1",
            Correo = "user@example.com",
            Contrasena = psm_web_site_api_project.Utils.Md5utils.Md5utilsClass.GetMd5("password123"),
            Nombres = "John",
            Apellidos = "Doe",
            Activo = true,
            Rol = new Rol { IdRol = "1", Nombre = "Admin" },
            Extension =
            [
                new Extension { IdExtension = "1", Nombre = "Ext1" },
                new Extension { IdExtension = "2", Nombre = "Ext2" }
            ],
        };

        SetupUsuariosListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.PutUsuariosRepository(usuarioId, usuarioPayloadMock);

        // Assert: Verificar el resultado
        Assert.Equal(usuarioId, usuarioPayloadMock.IdUsuario);
    }

    [Fact]
    public async Task SetStatusUsuariosRepository_ReturnsUsuarioActualizadoEstado()
    {
        // Arrange
        var usuarioId = "1";
        var status = false;

        SetupUsuariosListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SetStatusUsuariosRepository(usuarioId, status);

        // Assert: Verificar el resultado
        Assert.Equal(!status, result);
        Assert.True(result);
    }
}