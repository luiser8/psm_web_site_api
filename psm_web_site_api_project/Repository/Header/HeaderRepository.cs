using Microsoft.Extensions.Options;
using MongoDB.Driver;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Headers;
public class HeaderRepository : IHeaderRepository
{
    private readonly IMongoCollection<Header> _headerCollection;

    public HeaderRepository(IOptions<ConfigDB> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
        _headerCollection = mongoDatabase.GetCollection<Header>("header");
    }

    public async Task<Header> SelectHeaderPorIdRepository(string idHeader)
    {
        try
        {
            return await _headerCollection.Find(driver => driver.IdHeader == idHeader).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<Header> SelectHeaderPorIdExtensionRepository(string? idExtension)
    {
        try
        {
            if(string.IsNullOrEmpty(idExtension))
                return await _headerCollection.Find(driver => driver.EsNacional).FirstOrDefaultAsync();
            return await _headerCollection.Find(driver => driver.IdExtension == idExtension).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PostHeaderRepository(Header header)
    {
        try
        {
            await _headerCollection.InsertOneAsync(header);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PutHeaderRepository(string idHeader, Header header)
    {
        try
        {
            var filter = Builders<Header>.Filter.Eq(x => x.IdHeader, idHeader);
            var response = await _headerCollection.ReplaceOneAsync(filter, header);
            return response.IsModifiedCountAvailable;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteHeaderRepository(string idHeader)
    {
        try
        {
            var filter = Builders<Header>.Filter.Eq(x => x.IdHeader, idHeader);
            var response = await _headerCollection.DeleteOneAsync(filter);
            return response.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> AddItemToHeader(string idExtension, HeaderCollection newItem)
    {
        try
        {
            var filter = Builders<Header>.Filter.Eq(doc => doc.IdExtension, idExtension);
            var update = Builders<Header>.Update.Push(doc => doc.HeaderCollections, newItem);
            var updateResult = await _headerCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
        catch (MongoException ex)
        {
            throw new ApplicationException("Ocurrió un error al actualizar el header.", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Ocurrió un error inesperado.", ex);
        }
    }

    public async Task<bool> RemoveItemFromHeader(string idExtension, string itemNombreToRemove)
    {
        if (string.IsNullOrEmpty(idExtension))
            throw new ArgumentNullException(nameof(idExtension));
        if (string.IsNullOrEmpty(itemNombreToRemove))
            throw new ArgumentNullException(nameof(itemNombreToRemove));

        try
        {
            var filter = Builders<Header>.Filter.Eq(h => h.IdExtension, idExtension);
            var update = Builders<Header>.Update.PullFilter(
                h => h.HeaderCollections,
                hc => hc.Nombre == itemNombreToRemove
            );

            var updateResult = await _headerCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
        catch (MongoException ex)
        {
            throw new ApplicationException("Error en base de datos al eliminar item del header.", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error inesperado al eliminar item del header.", ex);
        }
    }
}