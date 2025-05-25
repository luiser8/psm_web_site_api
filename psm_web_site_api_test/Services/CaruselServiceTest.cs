using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.CarouselRepository;
using psm_web_site_api_project.Repository.Extensiones;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Services.Carousel;
using psm_web_site_api_project.Services.ImageUpAndDown;
using psm_web_site_api_test.mocks;
using Xunit;

namespace psm_web_site_api_test.Services;
public class CarouselServiceTest
{
    private readonly Mock<ICarouselRepository> _carouselRepositoryMock;
    private readonly Mock<IAuditoriasRepository> _auditoriasRepositoryMock;
    private readonly Mock<IExtensionesRepository> _extensionesRepositoryMock;
    private readonly Mock<IUsuariosRepository> _usuariosRepositoryMock;
    private readonly Mock<IImageUpAndDownService> _imageUpAndDownServiceMock;
    private readonly CarouselService _carouselService;

    public CarouselServiceTest()
    {
        _carouselRepositoryMock = new Mock<ICarouselRepository>();
        _auditoriasRepositoryMock = new Mock<IAuditoriasRepository>();
        _extensionesRepositoryMock = new Mock<IExtensionesRepository>();
        _usuariosRepositoryMock = new Mock<IUsuariosRepository>();
        _imageUpAndDownServiceMock = new Mock<IImageUpAndDownService>();

        _carouselService = new CarouselService(
            _carouselRepositoryMock.Object,
            _auditoriasRepositoryMock.Object,
            _extensionesRepositoryMock.Object,
            _usuariosRepositoryMock.Object,
            _imageUpAndDownServiceMock.Object
        );
    }

    private void SetupCarouselRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorCarouselMock = new Mock<IAsyncCursor<Carousel>>();
        asyncCursorCarouselMock.Setup(_ => _.Current).Returns(ListCarousels());
        asyncCursorCarouselMock
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

        _carouselRepositoryMock.Setup(r => r.PostCarouselRepository(It.IsAny<Carousel>()))
            .ReturnsAsync(true);

        _carouselRepositoryMock.Setup(r => r.SelectCarouselPorIdExtensionRepository(It.IsAny<string>()))
            .ReturnsAsync(new Carousel
            {
                IdCarousel = "carousel1",
                IdExtension = "ext1",
                EsNacional = true,
                CarouselCollections = [ new CarouselCollection
                {
                    IdCarouselCollection = "col1",
                    Nombre = "Test",
                    Imagen = "img1",
                    Title = "Title",
                    Iframe = "iframe",
                    Target = true
                }],
                FechaCreacion = DateTime.UtcNow,
                Activo = true
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
        _imageUpAndDownServiceMock.Setup(s => s.SelectImageUpAndDownService("image.png", "Carousel", "ExtensionName"))
            .ReturnsAsync((new byte[] { 1, 2, 3 }, "image/png"));
        _imageUpAndDownServiceMock.Setup(s => s.PostImageUpAndDownService(new FormFileMock("image content", "image.png", "image/png"), "Carousel", "ExtensionName"))
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

    private static List<Carousel> ListCarousels()
    {
        return
        [
            new Carousel
            {
                IdCarousel = "carousel1",
                IdExtension = "ext1",
                EsNacional = true,
                CarouselCollections = [ new CarouselCollection
                {
                    IdCarouselCollection = "col1",
                    Nombre = "Test",
                    Imagen = "img1",
                    Title = "Title",
                    Iframe = "iframe",
                    Target = true
                }],
                FechaCreacion = DateTime.UtcNow,
                Activo = true
            },
            new Carousel
            {
                IdCarousel = "carousel2",
                IdExtension = "ext2",
                EsNacional = false,
                CarouselCollections = [ new CarouselCollection
                {
                    IdCarouselCollection = "col2",
                    Nombre = "Test2",
                    Imagen = "img2",
                    Title = "Title2",
                    Iframe = "iframe",
                    Target = true
                }],
                FechaCreacion = DateTime.UtcNow,
                Activo = true
            },
        ];
    }

    [Fact]
    public async Task SelectCarouselPorIdExtensionService_ReturnsCarouselResponse_Success()
    {
        // Arrange
        var extensionId = "ext1";
        var carouselCollection = new CarouselCollection
        {
            IdCarouselCollection = "col1",
            Nombre = "Test",
            Imagen = "img1",
            Title = "Title",
            Iframe = "iframe",
            Target = true
        };
        var carousel = new Carousel
        {
            IdCarousel = "carousel1",
            IdExtension = extensionId,
            EsNacional = true,
            CarouselCollections = [carouselCollection],
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        SetupCarouselRepositoryMocks();

        // Act
        var result = await _carouselService.SelectCarouselPorIdExtensionService(extensionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(carousel.IdCarousel, result.IdCarousel);
        Assert.Equal(carousel.IdExtension, result.IdExtension);
        Assert.True(result.EsNacional);
        Assert.NotNull(result.CarouselCollections);
    }

    [Fact]
    public async Task SelectCarouselPorIdExtensionService_Throws_WhenCarouselNotFound()
    {
        // Arrange
        _carouselRepositoryMock.Setup(r => r.SelectCarouselPorIdExtensionRepository(It.IsAny<string>()))
            .ReturnsAsync((Carousel?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() =>
            _carouselService.SelectCarouselPorIdExtensionService("any"));
        Assert.Equal("No existe un Carousel con este id de extension", ex.Message);
    }

    [Fact]
    public async Task SelectCarouselPorIdExtensionService_Throws_WhenExtensionNotFound()
    {
        // Arrange
        var carousel = new Carousel
        {
            IdCarousel = "carousel1",
            IdExtension = "ext1",
            CarouselCollections = []
        };
        _carouselRepositoryMock.Setup(r => r.SelectCarouselPorIdExtensionRepository(It.IsAny<string>()))
            .ReturnsAsync((Carousel?)carousel);
        _extensionesRepositoryMock.Setup(r => r.SelectExtensionesPorIdRepository(It.IsAny<string>()))
            .ReturnsAsync((Extension)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() =>
            _carouselService.SelectCarouselPorIdExtensionService("ext1"));
        Assert.Equal("Extension Id no existe", ex.Message);
    }

    [Fact]
    public async Task SelectCarouselPorIdExtensionService_Throws_WhenExtensionInactive()
    {
        // Arrange
        var carousel = new Carousel
        {
            IdCarousel = "carousel1",
            IdExtension = "ext1",
            CarouselCollections = []
        };
        var extension = new Extension
        {
            IdExtension = "ext1",
            Nombre = "ExtensionName",
            Activo = false
        };
        _carouselRepositoryMock.Setup(r => r.SelectCarouselPorIdExtensionRepository(It.IsAny<string>()))
            .ReturnsAsync(carousel);
        _extensionesRepositoryMock.Setup(r => r.SelectExtensionesPorIdRepository(It.IsAny<string>()))
            .ReturnsAsync(extension);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotImplementedException>(() =>
            _carouselService.SelectCarouselPorIdExtensionService("ext1"));
        Assert.Equal("Extension desactivada", ex.Message);
    }

    [Fact]
    public async Task PostCarouselService_Create_WhenExtension()
    {
        // Arrange
        var carousel = new CarouselPayload
        {
            IdCarousel = "carousel3",
            IdExtension = "ext1",
            IdUsuarioIdentity = "user1",
            Iframe = "iframe3",
            Nombre = "Carousel 3",
            Imagen = new FormFileMock("image content", "image.png", "image/png"),
            Link = "http://example.com",
            Target = true,
        };

        SetupCarouselRepositoryMocks();
        _carouselRepositoryMock.Setup(r => r.SelectCarouselPorIdExtensionRepository(carousel.IdExtension))
            .ReturnsAsync((Carousel?)null);

        // Act
        var result = await _carouselService.PostCarouselService(carousel);

        //Assert
        Assert.True(result);
    }
}
