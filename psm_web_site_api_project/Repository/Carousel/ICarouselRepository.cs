using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.CarouselRepository;
public interface ICarouselRepository
{
    Task<Carousel> SelectCarouselPorIdRepository(string idCarousel);
    Task<Carousel> SelectCarouselPorIdExtensionRepository(string? idExtension);
    Task<bool> PostCarouselRepository(Carousel carousel);
    Task<bool> PutCarouselRepository(string idCarousel, Carousel carousel);
    Task<bool> DeleteCarouselRepository(string idCarousel);
    Task<bool> AddItemToCarousel(string idExtension, CarouselCollection newItem);
    Task<bool> RemoveItemFromCarousel(string idExtension, string itemNombreToRemove);
}