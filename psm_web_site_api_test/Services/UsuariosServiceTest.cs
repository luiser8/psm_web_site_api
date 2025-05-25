using AutoMapper;
using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Responses;
using psm_web_site_api_project.Services.Extensiones;
using psm_web_site_api_project.Services.Roles;
using psm_web_site_api_project.Services.Usuarios;
using Xunit;

namespace psm_web_site_api_test.Services;

public class UsuariosServiceTest
{
    private readonly Mock<IUsuariosRepository> _usuariosRepositoryMock;
    private readonly Mock<IExtensionesService> _extensionesServiceMock;
    private readonly Mock<IAuditoriasRepository> _auditoriaRepositoryMock;
    private readonly Mock<IRolesService> _rolesServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UsuariosService _usuariosService;

    public UsuariosServiceTest()
    {
        _usuariosRepositoryMock = new Mock<IUsuariosRepository>();
        _extensionesServiceMock = new Mock<IExtensionesService>();
        _rolesServiceMock = new Mock<IRolesService>();
        _auditoriaRepositoryMock = new Mock<IAuditoriasRepository>();
        _mapperMock = new Mock<IMapper>();
        _usuariosService = new UsuariosService(_usuariosRepositoryMock.Object, _rolesServiceMock.Object, _extensionesServiceMock.Object, _auditoriaRepositoryMock.Object, _mapperMock.Object);
    }

    private void SetupUsuariosRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Usuario>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListUsuarios());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        _rolesServiceMock
            .Setup(m => m.SelectRolPorIdService(It.IsAny<string>()))
            .ReturnsAsync(new Rol
            {
                IdRol = "1",
                Nombre = "Rol 1",
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
            });

        _auditoriaRepositoryMock
            .Setup(m => m.SelectAuditoriasPorUsuarioIdRepository(It.IsAny<string>()))
            .ReturnsAsync(
            [
                new Auditoria
                {
                    IdAuditoria = "1",
                    IdUsuario = "1",
                    Accion = "Accion 1",
                }
            ]);

        _extensionesServiceMock
            .Setup(m => m.GetCursorExtension(It.IsAny<List<string>>()))
            .ReturnsAsync(new List<Extension>
            {
                new Extension
                {
                    IdExtension = "1",
                    Nombre = "Extension 1",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                },
                new Extension
                {
                    IdExtension = "2",
                    Nombre = "Extension 2",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                }
            });

        _usuariosRepositoryMock
            .Setup(repo => repo.SelectUsuariosRepository())
            .ReturnsAsync(ListUsuarios());

        _usuariosRepositoryMock
            .Setup(repo => repo.SelectUsuariosPorIdRepository(It.IsAny<string>()))
            .ReturnsAsync(new Usuario
            {
                IdUsuario = "1",
                Nombres = "usuario",
                Apellidos = "apellido",
                Correo = "correo1@ejemplo.com",
                Contrasena = "contrasena",
                Activo = true,
                TokenAcceso = "token",
                TokenRefresco = "token_refresco",
                TokenExpiracion = DateTime.UtcNow.AddHours(1),
            });

        _usuariosRepositoryMock
            .Setup(repo => repo.PostUsuariosRepository(It.IsAny<Usuario>()))
            .ReturnsAsync(new Usuario
            {
                IdUsuario = "3",
                Nombres = "usuario",
                Apellidos = "apellido",
                Correo = "correo3@ejemplo.com",
                Contrasena = "contrasena",
                Activo = true,
                TokenAcceso = "token",
                TokenRefresco = "token_refresco",
                TokenExpiracion = DateTime.UtcNow.AddHours(1),
            });

        _usuariosRepositoryMock
            .Setup(repo => repo.PutUsuariosRepository(It.IsAny<string>(), It.IsAny<Usuario>()))
            .ReturnsAsync(It.IsAny<bool>());

        _usuariosRepositoryMock
            .Setup(repo => repo.DeleteUsuariosRepository(It.IsAny<string>()))
            .ReturnsAsync(It.IsAny<bool>());

        _usuariosRepositoryMock
            .Setup(repo => repo.SetStatusUsuariosRepository(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(It.IsAny<bool>());

        _mapperMock
            .Setup(m => m.Map<List<UsuariosResponse>>(It.IsAny<List<Usuario>>()))
            .Returns(ListUsuariosResponse());

        _mapperMock
            .Setup(m => m.Map<UsuariosResponse>(It.IsAny<Usuario>()))
            .Returns(ListUsuariosResponse().First());
    }

    private static List<Usuario> ListUsuarios()
    {
        return
        [
            new Usuario
            {
                IdUsuario = "4",
                Nombres = "usuario",
                Apellidos = "apellido",
                Correo = "correo4@ejemplo.com",
                Contrasena = "contrasena",
                Activo = true,
                TokenAcceso = "token",
                TokenRefresco = "token_refresco",
                TokenExpiracion = DateTime.UtcNow.AddHours(1),
            },
            new Usuario
            {
                IdUsuario = "5",
                Nombres = "usuario",
                Apellidos = "apellido",
                Correo = "correo5@ejemplo.com",
                Contrasena = "contrasena",
                Activo = true,
                TokenAcceso = "token",
                TokenRefresco = "token_refresco",
                TokenExpiracion = DateTime.UtcNow.AddHours(1),
            }
        ];
    }

    private static List<UsuariosResponse> ListUsuariosResponse()
    {
        return
        [
            new UsuariosResponse
            {
                IdUsuario = "6",
                Nombres = "usuario",
                Apellidos = "apellido",
                Correo = "correo6@ejemplo.com",
                Activo = true,
            },
            new UsuariosResponse
            {
                IdUsuario = "7",
                Nombres = "usuario",
                Apellidos = "apellido",
                Correo = "correo7@ejemplo.com",
                Activo = true,
            }
        ];
    }

    [Fact]
    public async Task SelectUsuariosService_ReturnsListOfUsuarios()
    {
        // Arrange
        SetupUsuariosRepositoryMocks();

        // Act
        var result = await _usuariosService.SelectUsuariosService();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<UsuariosResponse>>(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SelectUsuariosPorIdService_ReturnsUsuario()
    {
        // Arrange
        SetupUsuariosRepositoryMocks();

        // Act
        var result = await _usuariosService.SelectUsuariosPorIdService("6");

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UsuariosResponse>(result);
    }

    [Fact]
    public async Task SelectUsuariosPorAuditoriaService_ReturnsUsuario()
    {
        // Arrange
        SetupUsuariosRepositoryMocks();

        // Act
        var result = await _usuariosService.SelectUsuariosPorAuditoriaService("1");

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Auditoria>>(result);
    }

    [Fact]
    public async Task PostUsuariosService_ReturnsUsuario()
    {
        // Arrange
        var usuarioPayload = new UsuarioPayload
        {
            Nombres = "usuario",
            Apellidos = "apellido",
            Correo = "correo8@ejemplo.com",
            Contrasena = "contrasena",
            Extensiones = ["1", "2"],
            Rol = "rol",
        };
        SetupUsuariosRepositoryMocks();

        // Act
        var result = await _usuariosService.PostUsuariosService(usuarioPayload);

        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public async Task PutUsuariosService_ReturnsUsuario()
    {
        // Arrange
        var usuarioId = "1";
        var usuarioPayload = new UsuariosPayloadPut
        {
            IdUsuario = "1",
            IdUsuarioIdentity = "1",
            Nombres = "usuario",
            Apellidos = "apellido",
            Correo = "correo8@ejemplo.com",
            Contrasena = "contrasena",
            Extensiones = ["1", "2"],
            Rol = "rol",
        };
        SetupUsuariosRepositoryMocks();
        _usuariosRepositoryMock
            .Setup(repo => repo.SelectUsuariosPorCorreoRepository(It.IsAny<string>()))
            .ReturnsAsync(new Usuario
            {
                IdUsuario = "2",
                Nombres = "usuario",
                Apellidos = "apellido",
                Correo = "correo2@ejemplo.com",
                Contrasena = "contrasena",
                Activo = true,
                TokenAcceso = "token",
                TokenRefresco = "token_refresco",
                TokenExpiracion = DateTime.UtcNow.AddHours(1),
            });

        // Act
        var result = await _usuariosService.PutUsuariosService(usuarioId, usuarioPayload);

        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public async Task SetStatusUsuariosService_ReturnsUsuario()
    {
        // Arrange
         var usuarioPayload = new UsuariosPayloadDelete
        {
            IdUsuario = "1",
            IdUsuarioIdentity = "1",
        };
        SetupUsuariosRepositoryMocks();

        // Act
        var result = await _usuariosService.SetStatusUsuariosService(usuarioPayload, true);

        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public async Task DeleteUsuariosService_ReturnsUsuario()
    {
        // Arrange
         var usuarioPayload = new UsuariosPayloadDelete
        {
            IdUsuario = "1",
            IdUsuarioIdentity = "1",
        };
        SetupUsuariosRepositoryMocks();

        // Act
        var result = await _usuariosService.DeleteUsuariosService(usuarioPayload);

        // Assert
        Assert.IsType<bool>(result);
    }
}