using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Autenticacion;
using Xunit;

namespace psm_web_site_api_test.Repository;

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

    private void SetupAutenticacionRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Usuario>>();
        asyncCursorMock.Setup(_ => _.Current).Returns([SetupUsuarioMock()]);
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
    }
    
    private static Usuario SetupUsuarioMock()
    {
        return new Usuario
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
    }
    
    [Fact]
    public async Task RefreshRepository_ValidCredentials_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var refreshPayload = "fake_refresh_token";

        SetupAutenticacionRepositoryMocks();
        
        // Act: Ejecutar el método
        var result = await _repository.RefrescoRepository(refreshPayload);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(SetupUsuarioMock().Correo, result.Correo);
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

        SetupAutenticacionRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SessionRepository(loginPayload);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(SetupUsuarioMock().Correo, result.Correo);
        Assert.False(string.IsNullOrEmpty(result.TokenAcceso));
        Assert.False(string.IsNullOrEmpty(result.TokenRefresco));
    }

    [Fact]
    public async Task RemoveRepository_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var userId = "1";
        
        SetupAutenticacionRepositoryMocks();

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
        var token = "fake_refresh_token";

        SetupAutenticacionRepositoryMocks();
        
        // Act: Ejecutar el método
        var result = await _repository.ValidarRepository(userId, token);

        // Assert: Verificar el resultado
        Assert.False(result);
    }
}
