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

    public async Task<Header> SelectHeaderPorIdExtensionRepository(string idExtension)
    {
        try
        {
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

    public async Task<bool> PutHeaderRepository(string IdHeader, Header header)
    {
        try
        {
            var filter = Builders<Header>.Filter.Eq(x => x.IdHeader, IdHeader);
            await _headerCollection.ReplaceOneAsync(filter, header);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteHeaderRepository(string IdHeader)
    {
        try
        {
            var filter = Builders<Header>.Filter.Eq(x => x.IdHeader, IdHeader);
            await _headerCollection.DeleteOneAsync(filter);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}