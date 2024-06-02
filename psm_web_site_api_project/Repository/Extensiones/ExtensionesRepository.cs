using Microsoft.Extensions.Options;
using MongoDB.Driver;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Extensiones;
    public class ExtensionesRepository : IExtensionesRepository
    {
        private readonly IMongoCollection<Extension> _extensionCollection;

        public ExtensionesRepository(IOptions<ConfigDB> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
            _extensionCollection = mongoDatabase.GetCollection<Extension>("extensiones");
        }

        public async Task<List<Extension>> SelectExtensionesRepository()
        {
            try
            {
                var filter = Builders<Extension>.Filter.Empty;
                return await _extensionCollection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<IAsyncCursor<Extension>> SelectExtensionesFilterRepository(FilterDefinition<Extension> filter)
        {
            try
            {
                return await _extensionCollection.FindAsync(filter);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<Extension> SelectExtensionesPorIdRepository(string idExtension)
        {
            try
            {
                return await _extensionCollection.Find(driver => driver.IdExtension == idExtension).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<Extension> SelectExtensionesPorNombreRepository(string nombre)
        {
            try
            {
                return await _extensionCollection.Find(driver => driver.Nombre == nombre).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<bool> PostExtensionesRepository(Extension extension)
        {
            try
            {
                await _extensionCollection.InsertOneAsync(extension);
                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<bool> PutExtensionesRepository(string idExtension, Extension extension)
        {
            try
            {
                var filter = Builders<Extension>.Filter.Eq(x => x.IdExtension, idExtension);
                await _extensionCollection.ReplaceOneAsync(filter, extension);
                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<bool> DeleteExtensionesRepository(string idExtension)
        {
            try
            {
                var filter = Builders<Extension>.Filter.Eq(x => x.IdExtension, idExtension);
                await _extensionCollection.DeleteOneAsync(filter);
                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
    }