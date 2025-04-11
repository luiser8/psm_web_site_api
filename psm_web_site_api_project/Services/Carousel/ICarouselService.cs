using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Responses;

namespace psm_web_site_api_project.Services.Carousel;
public interface ICarouselService
{
    Task<CarouselResponse> SelectCarouselPorIdExtensionService(string? idExtension);
    Task<bool> PostCarouselService(CarouselPayload carouselPayload);
    Task<bool> PutCarouselService(string idCarousel, CarouselPayload carouselPayload);
    Task<bool> DeleteCarouselService(CarouselPayload carouselPayload);
    Task<bool> AddItemToCarousel(string idExtension, CarouselCollectionPayload newItem);
    Task<bool> RemoveItemFromCarousel(string idExtension, string itemNombreToRemove);
}