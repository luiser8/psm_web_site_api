using Microsoft.Extensions.Options;
using MongoDB.Driver;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.CarouselRepository;
public class CarouselRepository : ICarouselRepository
{
    private readonly IMongoCollection<Carousel> _carouselCollection;

    public CarouselRepository(IOptions<ConfigDB> options, IMongoClient mongoClient)
    {
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
        _carouselCollection = mongoDatabase.GetCollection<Carousel>("carousel");
    }

    public async Task<Carousel> SelectCarouselPorIdRepository(string idCarousel)
    {
        try
        {
            return await _carouselCollection.Find(driver => driver.IdCarousel == idCarousel).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<Carousel> SelectCarouselPorIdExtensionRepository(string? idExtension)
    {
        try
        {
            if(string.IsNullOrEmpty(idExtension))
                return await _carouselCollection.Find(driver => driver.EsNacional).FirstOrDefaultAsync();
            return await _carouselCollection.Find(driver => driver.IdExtension == idExtension).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PostCarouselRepository(Carousel carousel)
    {
        try
        {
            await _carouselCollection.InsertOneAsync(carousel);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PutCarouselRepository(string idCarousel, Carousel carousel)
    {
        try
        {
            var filter = Builders<Carousel>.Filter.Eq(x => x.IdCarousel, idCarousel);
            var response = await _carouselCollection.ReplaceOneAsync(filter, carousel);
            return response.IsModifiedCountAvailable;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteCarouselRepository(string idCarousel)
    {
        try
        {
            var filter = Builders<Carousel>.Filter.Eq(x => x.IdCarousel, idCarousel);
            var response = await _carouselCollection.DeleteOneAsync(filter);
            return response.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> AddItemToCarousel(string idExtension, CarouselCollection newItem)
    {
        try
        {
            var filter = Builders<Carousel>.Filter.Eq(doc => doc.IdExtension, idExtension);
            var update = Builders<Carousel>.Update.Push(doc => doc.CarouselCollections, newItem);
            var updateResult = await _carouselCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
        catch (MongoException ex)
        {
            throw new ApplicationException("Ocurrió un error al actualizar el carrusel.", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ocurrió un error inesperado.", ex);
        }
    }

    public async Task<bool> RemoveItemFromCarousel(string idExtension, string itemNombreToRemove)
    {
        if (string.IsNullOrEmpty(idExtension))
            throw new ArgumentNullException(nameof(idExtension));
        if (string.IsNullOrEmpty(itemNombreToRemove))
            throw new ArgumentNullException(nameof(itemNombreToRemove));

        try
        {
            var filter = Builders<Carousel>.Filter.Eq(h => h.IdExtension, idExtension);
            var update = Builders<Carousel>.Update.PullFilter(
                h => h.CarouselCollections,
                hc => hc.Nombre == itemNombreToRemove
            );

            var updateResult = await _carouselCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
        catch (MongoException ex)
        {
            throw new ApplicationException("Error en base de datos al eliminar item del carousel.", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error inesperado al eliminar item del carousel.", ex);
        }
    }
}