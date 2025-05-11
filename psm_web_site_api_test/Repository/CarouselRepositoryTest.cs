using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.CarouselRepository;
using Xunit;

namespace psm_web_site_api_test.Repository;

public class CarouselRepositoryTest
{
    private readonly Mock<IMongoCollection<Carousel>> _carouselCollectionMock;
    private readonly Mock<IOptions<ConfigDB>> _optionsMock;
    private readonly Mock<IMongoClient> _mongoClientMock;
    private readonly CarouselRepository _repository;

    public CarouselRepositoryTest()
    {
        // Mock de IOptions<ConfigDB>
        _optionsMock = new Mock<IOptions<ConfigDB>>();
        _optionsMock.Setup(o => o.Value).Returns(new ConfigDB
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "testdb"
        });

        // Mock de IMongoCollection<Rol>
        _carouselCollectionMock = new Mock<IMongoCollection<Carousel>>();

        _mongoClientMock = new Mock<IMongoClient>();
        var mongoDatabaseMock = new Mock<IMongoDatabase>();
        mongoDatabaseMock.Setup(db => db.GetCollection<Carousel>("carousel", null)).Returns(_carouselCollectionMock.Object);
        _mongoClientMock.Setup(client => client.GetDatabase(It.IsAny<string>(), null)).Returns(mongoDatabaseMock.Object);

        // Crear instancia real del repositorio
        _repository = new CarouselRepository(_optionsMock.Object, _mongoClientMock.Object);

        // Inyectar manualmente el mock en el campo privado _usuariosCollection usando Reflection
        var fieldInfo = typeof(CarouselRepository)
            .GetField("_carouselCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (fieldInfo != null)
        {
            fieldInfo.SetValue(_repository, _carouselCollectionMock.Object);
        }
        else
        {
            throw new InvalidOperationException("Field '_carouselCollection' not found in CarouselRepository.");
        }
    }

    private void SetupCarouselListRepositoryMocks()
    {
        // Mock de IAsyncCursor para Find
        var asyncCursorMock = new Mock<IAsyncCursor<Carousel>>();
        asyncCursorMock.Setup(_ => _.Current).Returns(ListCarousel());
        asyncCursorMock
            .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        asyncCursorMock
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        // Mockear el Find para devolver el cursor
        _carouselCollectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Carousel>>(),
                It.IsAny<FindOptions<Carousel, Carousel>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(asyncCursorMock.Object);

        var replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);
        _carouselCollectionMock
            .Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Carousel>>(),
                It.IsAny<Carousel>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(replaceResult);

        var updateResult = new UpdateResult.Acknowledged(1, 1, null);
        _carouselCollectionMock
            .Setup(x => x.UpdateOneAsync(
                It.IsAny<FilterDefinition<Carousel>>(),
                It.IsAny<UpdateDefinition<Carousel>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateResult);

        var deleteResult = new DeleteResult.Acknowledged(1);
        _carouselCollectionMock
            .Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Carousel>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResult);
    }

    private static List<Carousel> ListCarousel()
    {
        return
        [
            new Carousel
            {
                IdCarousel = "1",
                EsNacional = true,
                IdExtension = "1",
                CarouselCollections = [
                    new() {
                        Activo = true,
                        IdCarouselCollection = "1",
                        Iframe = "",
                        Imagen = "",
                        Link = "/link",
                        Nombre = "test",
                        Target = true,
                        Title = ""
                    }
                ],
                Activo = true,
                FechaCreacion = DateTime.Now
            }
        ];
    }

    [Fact]
    public async Task SelectCarouselRepository_ReturnsListByIdCarousel()
    {
        // Arrange
        var idCarousel = "1";
        SetupCarouselListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectCarouselPorIdRepository(idCarousel);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(result.IdCarousel, ListCarousel()[0].IdCarousel);
    }

    [Fact]
    public async Task SelectCarouselRepository_ReturnsListByIdExtension()
    {
        // Arrange
        var idExtension = "1";
        SetupCarouselListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.SelectCarouselPorIdExtensionRepository(idExtension);

        // Assert: Verificar el resultado
        Assert.NotNull(result);
        Assert.Equal(result.IdExtension, ListCarousel()[0].IdExtension);
    }

    [Fact]
    public async Task PostCarouselRepository_ReturnsTrue()
    {
        // Arrange
        var carousel = new Carousel
        {
            IdCarousel = "1",
            EsNacional = true,
            IdExtension = "1",
            CarouselCollections = [
                    new() {
                        Activo = true,
                        IdCarouselCollection = "1",
                        Iframe = "",
                        Imagen = "",
                        Link = "/link",
                        Nombre = "test",
                        Target = true,
                        Title = ""
                    }
                ],
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        SetupCarouselListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.PostCarouselRepository(carousel);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task PutCarouselRepository_ReturnsTrue()
    {
        // Arrange
        var carousel = new Carousel
        {
            IdCarousel = "1",
            EsNacional = true,
            IdExtension = "1",
            CarouselCollections = [
                    new() {
                        Activo = true,
                        IdCarouselCollection = "1",
                        Iframe = "",
                        Imagen = "",
                        Link = "/link",
                        Nombre = "test",
                        Target = true,
                        Title = ""
                    }
                ],
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        SetupCarouselListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.PutCarouselRepository("1", carousel);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCarouselRepository_ReturnsTrue()
    {
        // Arrange
        const string carouselId = "1";

        SetupCarouselListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.DeleteCarouselRepository(carouselId);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task AddItemToCarouselRepository_ReturnsTrue()
    {
        // Arrange
        var idExtension = "1";
        var carousel = new CarouselCollection
        {

            Activo = true,
            IdCarouselCollection = "1",
            Iframe = "iframe",
            Imagen = "image",
            Link = "/link2",
            Nombre = "test",
            Target = true,
            Title = "test"
        };

        SetupCarouselListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.AddItemToCarousel(idExtension, carousel);

        // Assert: Verificar el resultado
        Assert.True(result);
    }

    [Fact]
    public async Task RemoveItemFromCarouselRepository_ReturnsTrue()
    {
        // Arrange
        var idExtension = "1";
        var itemNombreToRemove = "1";

        SetupCarouselListRepositoryMocks();

        // Act: Ejecutar el método
        var result = await _repository.RemoveItemFromCarousel(idExtension, itemNombreToRemove);

        // Assert: Verificar el resultado
        Assert.True(result);
    }
}