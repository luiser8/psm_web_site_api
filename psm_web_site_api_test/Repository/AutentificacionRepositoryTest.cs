using Xunit;
using Moq;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using psm_web_site_api_project.Repository.Autenticacion;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Config;

public class AutenticacionRepositoryTests
{
    private readonly Mock<IMongoCollection<Usuario>> _usuariosCollectionMock;
    private readonly Mock<IOptions<ConfigDB>> _optionsMock;
    private readonly Mock<IMongoClient> _mongoClientMock;
    private readonly AutenticacionRepository _repository;

    public AutenticacionRepositoryTests()
    {
        // 1. Mock de IConfiguration
        var configurationMock = new Mock<IConfiguration>();

        // 2. Configurar lo que devuelve cuando busca claves
        configurationMock.Setup(c => c["Security:Jwt:Token"]).Returns("clave-secreta-fake-muylarga-para-test");
        configurationMock.Setup(c => c["Security:Jwt:ExpirationToken"]).Returns("7");
        configurationMock.Setup(c => c["Security:Jwt:ExpirationRefreshToken"]).Returns("14");

        // 3. Inicializar JwtUtils
        var jwtUtilsMock = new Mock<IJwtUtils>();
        jwtUtilsMock.Setup(j => j.CreateToken(It.IsAny<TokenPayload>())).Returns("fake_access_token");
        jwtUtilsMock.Setup(j => j.RefreshToken(It.IsAny<TokenPayload>())).Returns("fake_refresh_token");

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
        _repository = new AutenticacionRepository(_optionsMock.Object, _mongoClientMock.Object, jwtUtilsMock.Object);

        // Inyectar manualmente el mock en el campo privado _usuariosCollection usando Reflection
        var fieldInfo = typeof(AutenticacionRepository)
            .GetField("_usuariosCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (fieldInfo != null)
        {
            fieldInfo.SetValue(_repository, _usuariosCollectionMock.Object);
        }
        else
        {
            throw new InvalidOperationException("Field '_usuariosCollection' not found in AutenticacionRepository.");
        }
    }

    [Fact]
    public async Task RefreshRepository_ValidCredentials_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var refreshPayload = "fake_refresh_token";

        var usuarioMock = new Usuario
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

        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Usuario>>();
        asyncCursorMock.Setup(_ => _.Current).Returns([usuarioMock]);
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

        // Act: Ejecutar el método
        var result = await _repository.RefrescoRepository(refreshPayload);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(usuarioMock.Correo, result.Correo);
        Assert.False(string.IsNullOrEmpty(result.TokenAcceso));
        Assert.False(string.IsNullOrEmpty(result.TokenRefresco));
    }

    [Fact]
    public async Task SessionRepository_ValidCredentials_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var loginPayload = new LoginPayload
        {
            Correo = "user@example.com",
            Contrasena = "password123"
        };

        var usuarioMock = new Usuario
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

        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Usuario>>();
        asyncCursorMock.Setup(_ => _.Current).Returns([usuarioMock]);
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

        // Act: Ejecutar el método
        var result = await _repository.SessionRepository(loginPayload);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(usuarioMock.Correo, result.Correo);
        Assert.False(string.IsNullOrEmpty(result.TokenAcceso));
        Assert.False(string.IsNullOrEmpty(result.TokenRefresco));
    }

    [Fact]
    public async Task RemoveRepository_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var userId = "1";

        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Usuario>>();
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

        // Act: Ejecutar el método
        var result = await _repository.RemoverRepository(userId);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateTokenRepository_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var userId = "1";
        var token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJpZHVzZXIiOiI2NjBmNjY4Y2YyMTY5Mjk5MTk0ZTVhYzAiLCJmaXJzdG5hbWUiOiJMdWlzIEVkdWFyZG8iLCJsYXN0bmFtZSI6IlJvbmRvbiIsImVtYWlsIjoibGVkdWFyZG8ucm9uZG9uQGdtYWlsLmNvbSIsInJvbCI6IjY2MGYyYTM2ZjRiYjE3YjU1NDU5MDRhYi1BZG1pbmlzdHJhZG9yIiwiZXh0ZW5zaW9ucyI6WyI2NjBmMmFjM2Y0YmIxN2I1NTQ1OTA0YjAtTmFjaW9uYWwiLCI2NjBmMmFjM2Y0YmIxN2I1NTQ1OTA0YjItQ2FyYWNhcyIsIjY2MGYyYWMzZjRiYjE3YjU1NDU5MDRiMS1CYXJjZWxvbmEiXSwiZXhwIjoxNzQ1OTU4MzI1fQ.LMZc_23cZXPw-wbac3n1E8zFKLcAYypGkVy5fo2DojJmQmOXj6PfrQpCbobrN7gVGQ5ckvbx5nuwoDuaUEyzJA";

        var usuarioMock = new Usuario
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
            TokenAcceso = token,
            TokenRefresco = token,
            TokenCreado = DateTime.Now,
            TokenExpiracion = DateTime.Now.AddDays(7)
        };

        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Usuario>>();
        asyncCursorMock.Setup(_ => _.Current).Returns([usuarioMock]);
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

        // Act: Ejecutar el método
        var result = await _repository.ValidarRepository(userId, token);

        // Assert: Verificar el resultado
        Assert.True(result);
    }
}
