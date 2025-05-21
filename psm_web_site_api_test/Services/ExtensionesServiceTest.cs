using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Extensiones;
using psm_web_site_api_project.Services.Extensiones;
using Xunit;

namespace psm_web_site_api_test.Services;

public class ExtensionesServiceTest
{
    private readonly Mock<IExtensionesRepository> _extensionesRepositoryMock;
    private readonly Mock<IAuditoriasRepository> _auditoriaRepositoryMock;
    private readonly ExtensionesService _extensionesService;

    public ExtensionesServiceTest()
    {
        _extensionesRepositoryMock = new Mock<IExtensionesRepository>();
        _auditoriaRepositoryMock = new Mock<IAuditoriasRepository>();
        _extensionesService = new ExtensionesService(_extensionesRepositoryMock.Object, _auditoriaRepositoryMock.Object);
    }

    private void SetupExtensionesRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Extension>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListExtensiones());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        _extensionesRepositoryMock
            .Setup(repo => repo.SelectExtensionesRepository())
            .ReturnsAsync(ListExtensiones());

        // _extensionesRepositoryMock
        //     .Setup(repo => repo.SelectExtensionesPorNombreRepository(It.IsAny<string>()))
        //     .ReturnsAsync(new Extension
        //     {
        //         IdExtension = "1",
        //         Nombre = "Nacional"
        //     });

        _extensionesRepositoryMock
            .Setup(repo => repo.SelectExtensionesPorIdRepository(It.IsAny<string>()))
            .ReturnsAsync(new Extension
            {
                IdExtension = "1",
                Nombre = "Nacional"
            });

        _extensionesRepositoryMock
            .Setup(repo => repo.SelectExtensionesPorIdRepository(It.IsAny<string>()))
            .ReturnsAsync(new Extension
            {
                IdExtension = "1",
                Nombre = "Nacional"
            });

        _extensionesRepositoryMock
            .Setup(repo => repo.SelectExtensionesFilterRepository(
                It.IsAny<FilterDefinition<Extension>>()))
            .ReturnsAsync(asyncCursorMock.Object);

        _extensionesRepositoryMock
            .Setup(repo => repo.PostExtensionesRepository(It.IsAny<Extension>()))
            .ReturnsAsync(true);

        _extensionesRepositoryMock
            .Setup(repo => repo.PutExtensionesRepository(It.IsAny<string>(), It.IsAny<Extension>()))
            .ReturnsAsync(true);

        _extensionesRepositoryMock
            .Setup(repo => repo.DeleteExtensionesRepository(It.IsAny<string>()))
            .ReturnsAsync(true);
    }

    private static List<Extension> ListExtensiones()
    {
        return new List<Extension>
        {
            new Extension
            {
                IdExtension = "1",
                Nombre = "Nacional"
            },
            new Extension
            {
                IdExtension = "2",
                Nombre = "Barcelona"
            }
        };
    }

    [Fact]
    public async Task SelectExtensionPorIdService_Valid_ReturnsExtension()
    {
        // Arrange: Datos de prueba
        var extensionId = "1";

        SetupExtensionesRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _extensionesService.SelectExtensionesPorIdService(extensionId);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.IdExtension));
        Assert.False(string.IsNullOrEmpty(result.Nombre));
    }

    [Fact]
    public async Task SelectExtensionesService_Valid_ReturnsExtension()
    {
        // Arrange: Datos de prueba

        SetupExtensionesRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _extensionesService.SelectExtensionesService();

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.FirstOrDefault()?.IdExtension));
        Assert.False(string.IsNullOrEmpty(result.FirstOrDefault()?.Nombre));
    }

    [Fact]
    public async Task PostExtensionesService_Valid_ReturnsExtension()
    {
        // Arrange: Datos de prueba
        var extension = new ExtensionPayload
        {
            Nombre = "Barcelona",
            Descripcion = "Extension Barcelona",
            IdUsuarioIdentity = "1",
            Activo = true,
        };

        SetupExtensionesRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _extensionesService.PostExtensionesService(extension);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.True(result);
    }

    [Fact]
    public async Task PutExtensionesService_Valid_ReturnsExtension()
    {
        // Arrange: Datos de prueba
        var idExtension = "1";
        var extension = new ExtensionPayload
        {
            Nombre = "Barcelona",
            Descripcion = "Extension Barcelona",
            IdUsuarioIdentity = "1",
            Activo = true,
        };

        SetupExtensionesRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _extensionesService.PutExtensionesService(idExtension, extension);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteExtensionesService_Valid_ReturnsExtension()
    {
        // Arrange: Datos de prueba
        var extension = new ExtensionPayload
        {
            IdExtension = "1",
            Nombre = "Barcelona",
            Descripcion = "Extension Barcelona",
            IdUsuarioIdentity = "1",
            Activo = true,
        };

        SetupExtensionesRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _extensionesService.DeleteExtensionesService(extension);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.True(result);
    }
}
