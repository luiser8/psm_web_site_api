using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Autenticacion;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Services.Autenticacion;
using Xunit;

namespace psm_web_site_api_test.Services;

public class AutenticacionServiceTest
{
    private readonly Mock<IAutenticacionRepository> _autenticacionRepositoryMock;
    private readonly Mock<IUsuariosRepository> _usuariosRepositoryMock;
    private readonly Mock<IAuditoriasRepository> _auditoriasRepositoryMock;
    private readonly AutenticacionService _autenticacionService;

    public AutenticacionServiceTest()
    {
        _autenticacionRepositoryMock = new Mock<IAutenticacionRepository>();
        _usuariosRepositoryMock = new Mock<IUsuariosRepository>();
        _auditoriasRepositoryMock = new Mock<IAuditoriasRepository>();
        _autenticacionService = new AutenticacionService(_autenticacionRepositoryMock.Object, _usuariosRepositoryMock.Object, _auditoriasRepositoryMock.Object);
    }

    private void SetupSessionRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Usuario>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListUsuarios());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

       _usuariosRepositoryMock
            .Setup(repo => repo.PutUsuariosRepository(It.IsAny<string>(), It.IsAny<Usuario>()))
            .ReturnsAsync(true);

        _auditoriasRepositoryMock
            .Setup(repo => repo.PostAuditoriasRepository(It.IsAny<Auditoria>()))
            .ReturnsAsync(true);

        _autenticacionRepositoryMock
            .Setup(repo => repo.SessionRepository(It.IsAny<LoginPayload>()))
            .ReturnsAsync(new Usuario { IdUsuario = "1", TokenAcceso = "fake_access_token", TokenRefresco = "fake_refresh_token" });

        _autenticacionRepositoryMock
            .Setup(repo => repo.RefrescoRepository(It.IsAny<string>()))
            .ReturnsAsync(new Usuario { IdUsuario = "1", TokenAcceso = "fake_access_token", TokenRefresco = "fake_refresh_token" });

        _autenticacionRepositoryMock
            .Setup(repo => repo.RemoverRepository(It.IsAny<string>()))
            .ReturnsAsync(true);

        _autenticacionRepositoryMock
            .Setup(repo => repo.ValidarRepository(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
    }

    private static List<Usuario> ListUsuarios()
    {
        return new List<Usuario>
        {
            new Usuario
            {
                IdUsuario = "1",
                Correo = "user@example.com",
                Nombres = "John",
                Apellidos = "Doe",
                Rol = new Rol { IdRol = "1", Nombre = "Admin" },
                Extension = new List<Extension>
                {
                    new Extension { IdExtension = "1", Nombre = "Ext1" },
                    new Extension { IdExtension = "2", Nombre = "Ext2" }
                }
            },
            new Usuario
            {
                IdUsuario = "2",
                Correo = "user2@example.com",
                Nombres = "Jane",
                Apellidos = "Smith",
                Rol = new Rol { IdRol = "2", Nombre = "User" },
                Extension = new List<Extension>()
                {
                    new Extension { IdExtension = "3", Nombre = "Ext3" },
                    new Extension { IdExtension = "4", Nombre = "Ext4" }
                }
            }
        };
    }

    [Fact]
    public async Task SessionService_ValidCredentials_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var loginPayload = new LoginPayload
        {
            Correo = "user@example.com",
            Contrasena = "password123"
        };

        SetupSessionRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _autenticacionService.SessionService(loginPayload);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.accessToken));
        Assert.False(string.IsNullOrEmpty(result.refreshToken));
    }

    [Fact]
    public async Task RefreshService_ValidCredentials_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var token = "fake_refresh_token";

        SetupSessionRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _autenticacionService.RefrescoService(token);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.accessToken));
        Assert.False(string.IsNullOrEmpty(result.refreshToken));
    }

    [Fact]
    public async Task RemoveService_ValidCredentials_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var usuarioId = "1";

        SetupSessionRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _autenticacionService.RemoverService(usuarioId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.True(result);
    }

    [Fact]
    public async Task ValidarService_ValidCredentials_ReturnsUsuarioWithTokens()
    {
        // Arrange: Datos de prueba
        var usuarioId = "1";
        var token = "fake_access_token";

        SetupSessionRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _autenticacionService.ValidarService(usuarioId, token);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.True(result);
    }
}