using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Extensiones;
using psm_web_site_api_project.Repository.Headers;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Services.Headers;
using psm_web_site_api_project.Services.ImageUpAndDown;
using psm_web_site_api_test.mocks;
using Xunit;

namespace psm_web_site_api_test.Services;
public class HeaderServiceTest
{
    private readonly Mock<IHeaderRepository> _headerRepositoryMock;
    private readonly Mock<IAuditoriasRepository> _auditoriasRepositoryMock;
    private readonly Mock<IExtensionesRepository> _extensionesRepositoryMock;
    private readonly Mock<IUsuariosRepository> _usuariosRepositoryMock;
    private readonly Mock<IImageUpAndDownService> _imageUpAndDownServiceMock;
    private readonly HeaderService _headerService;

    public HeaderServiceTest()
    {
        _headerRepositoryMock = new Mock<IHeaderRepository>();
        _auditoriasRepositoryMock = new Mock<IAuditoriasRepository>();
        _extensionesRepositoryMock = new Mock<IExtensionesRepository>();
        _usuariosRepositoryMock = new Mock<IUsuariosRepository>();
        _imageUpAndDownServiceMock = new Mock<IImageUpAndDownService>();

        _headerService = new HeaderService(
            _headerRepositoryMock.Object,
            _auditoriasRepositoryMock.Object,
            _extensionesRepositoryMock.Object,
            _usuariosRepositoryMock.Object,
            _imageUpAndDownServiceMock.Object
        );
    }

    private void SetupHeaderRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorHeaderMock = new Mock<IAsyncCursor<Header>>();
        asyncCursorHeaderMock.Setup(_ => _.Current).Returns(ListHeaders());
        asyncCursorHeaderMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        var asyncCursorExtensionMock = new Mock<IAsyncCursor<Extension>>();
        asyncCursorExtensionMock.Setup(_ => _.Current).Returns(
        [
            new Extension
            {
                IdExtension = "ext1",
                Nombre = "ExtensionName",
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                EsNacional = true,
            }
        ]);
        asyncCursorExtensionMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        _headerRepositoryMock.Setup(r => r.PostHeaderRepository(It.IsAny<Header>()))
            .ReturnsAsync(true);

        _headerRepositoryMock.Setup(r => r.SelectHeaderPorIdExtensionRepository(It.IsAny<string>()))
            .ReturnsAsync(new Header
            {
                IdHeader = "header1",
                IdExtension = "ext1",
                EsNacional = true,
                HeaderCollections = [ new HeaderCollection
                {
                    IdHeaderCollection = "col1",
                    Nombre = "Test",
                    Target = true
                }],
                FechaCreacion = DateTime.UtcNow,
                Activo = true,
                Logo = "img1"
            });
        _extensionesRepositoryMock.Setup(r => r.SelectExtensionesPorIdRepository(It.IsAny<string>()))
            .ReturnsAsync(new Extension
            {
                IdExtension = "ext1",
                Nombre = "ExtensionName",
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                EsNacional = true,
            });
        _extensionesRepositoryMock.Setup(r => r.SelectExtensionesRepository())
            .ReturnsAsync(new List<Extension>
            {
                new Extension
                {
                    IdExtension = "ext1",
                    Nombre = "ExtensionName",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    EsNacional = true,
                }
            });
        _imageUpAndDownServiceMock.Setup(s => s.SelectImageUpAndDownService("image.png", "Header", "ExtensionName"))
            .ReturnsAsync((new byte[] { 1, 2, 3 }, "image/png"));
        _imageUpAndDownServiceMock.Setup(s => s.PostImageUpAndDownService(new FormFileMock("image content", "image.png", "image/png"), "Header", "ExtensionName"))
            .ReturnsAsync("img1");

        _usuariosRepositoryMock.Setup(r => r.SelectUsuariosPorIdRepository(It.IsAny<string>()))
            .ReturnsAsync(new Usuario
            {
                IdUsuario = "user1",
                Nombres = "Test User",
                Apellidos = "Test User",
                Extension =
                [
                    new Extension
                    {
                        IdExtension = "ext1",
                        Nombre = "ExtensionName",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        EsNacional = true,
                    }
                ],
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            });
    }

    private static List<Header> ListHeaders()
    {
        return
        [
            new Header
            {
                IdHeader = "header1",
                IdExtension = "ext1",
                EsNacional = true,
                HeaderCollections = [ new HeaderCollection
                {
                    IdHeaderCollection = "col1",
                    Nombre = "Test",
                    Target = true
                }],
                FechaCreacion = DateTime.UtcNow,
                Activo = true,
                Logo = "img1"
            },
            new Header
            {
                IdHeader = "header2",
                IdExtension = "ext2",
                EsNacional = false,
                HeaderCollections = [ new HeaderCollection
                {
                    IdHeaderCollection = "col2",
                    Nombre = "Test2",
                    Target = true
                }],
                FechaCreacion = DateTime.UtcNow,
                Activo = true,
                Logo = "img1"
            },
        ];
    }

    [Fact]
    public async Task SelectHeaderPorIdExtensionService_ReturnsHeaderResponse_Success()
    {
        // Arrange
        var extensionId = "ext1";
        var headerCollection = new HeaderCollection
        {
            IdHeaderCollection = "col1",
            Nombre = "Test",
            Target = true
        };
        var header = new Header
        {
            IdHeader = "header1",
            IdExtension = extensionId,
            EsNacional = true,
            HeaderCollections = [headerCollection],
            FechaCreacion = DateTime.UtcNow,
            Activo = true,
            Logo = "img1"
        };

        SetupHeaderRepositoryMocks();

        // Act
        var result = await _headerService.SelectHeaderPorIdExtensionService(extensionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(header.IdHeader, result.IdHeader);
        Assert.Equal(header.IdExtension, result.IdExtension);
        Assert.True(result.EsNacional);
        Assert.NotNull(result.HeaderCollections);
    }

    [Fact]
    public async Task SelectHeaderPorIdExtensionService_Throws_WhenHeaderNotFound()
    {
        // Arrange
        _headerRepositoryMock.Setup(r => r.SelectHeaderPorIdExtensionRepository(It.IsAny<string>()))
            .ReturnsAsync((Header?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() =>
            _headerService.SelectHeaderPorIdExtensionService("any"));
        Assert.Equal("No existe un Header con este id de extension", ex.Message);
    }

    [Fact]
    public async Task SelectHeaderPorIdExtensionService_Throws_WhenExtensionNotFound()
    {
        // Arrange
        var header = new Header
        {
            IdHeader = "header1",
            IdExtension = "ext1",
            EsNacional = true,
            HeaderCollections = [],
            FechaCreacion = DateTime.UtcNow,
            Activo = true,
            Logo = "img1"
        };
        _headerRepositoryMock.Setup(r => r.SelectHeaderPorIdExtensionRepository(It.IsAny<string>()))
            .ReturnsAsync((Header?)header);
        _extensionesRepositoryMock.Setup(r => r.SelectExtensionesPorIdRepository(It.IsAny<string>()))
            .ReturnsAsync((Extension)null);
        _extensionesRepositoryMock.Setup(r => r.SelectExtensionesRepository())
            .ReturnsAsync(new List<Extension>());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() =>
            _headerService.SelectHeaderPorIdExtensionService("ext1"));
        Assert.Equal("No existen extensiones", ex.Message);
    }

    [Fact]
    public async Task SelectHeaderPorIdExtensionService_Throws_WhenExtensionInactive()
    {
        // Arrange
        var header = new Header
        {
            IdHeader = "header1",
            IdExtension = "ext1",
            EsNacional = true,
            HeaderCollections = [],
            FechaCreacion = DateTime.UtcNow,
            Activo = true,
            Logo = "img1"
        };
        var extension = new Extension
        {
            IdExtension = "ext1",
            Nombre = "ExtensionName",
            Activo = false
        };
        _headerRepositoryMock.Setup(r => r.SelectHeaderPorIdExtensionRepository(It.IsAny<string>()))
            .ReturnsAsync(header);
        _extensionesRepositoryMock.Setup(r => r.SelectExtensionesPorIdRepository(It.IsAny<string>()))
            .ReturnsAsync(extension);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() =>
            _headerService.SelectHeaderPorIdExtensionService("ext1"));
        Assert.Equal("No existen extensiones", ex.Message);
    }

    [Fact]
    public async Task PostHeaderService_Create_WhenExtension()
    {
        // Arrange
        var header = new HeaderPayload
        {
            IdExtension = "ext1",
            EsNacional = true,
            HeaderCollections = [ new HeaderCollection
            {
                Nombre = "Test",
                Target = true,
            }],
            Logo = new FormFileMock("image content", "image.png", "image/png"),
            IdUsuarioIdentity = "user1"
        };

        SetupHeaderRepositoryMocks();
        _headerRepositoryMock.Setup(r => r.SelectHeaderPorIdExtensionRepository(header.IdExtension))
            .ReturnsAsync((Header?)null);

        _imageUpAndDownServiceMock.Setup(s => s.PostImageUpAndDownService(header.Logo, "Header", "ExtensionName"))
            .ReturnsAsync("img1");

        // Act
        var result = await _headerService.PostHeaderService(header);

        //Assert
        Assert.True(result);
    }
}
