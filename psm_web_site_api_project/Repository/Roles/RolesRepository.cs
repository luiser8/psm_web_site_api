using Microsoft.Extensions.Options;
using MongoDB.Driver;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Roles;

    public class RolesRepository : IRolesRepository
    {
        private readonly IMongoCollection<Rol> _rolCollection;

        public RolesRepository(IOptions<ConfigDB> options, IMongoClient mongoClient)
        {
            var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
            _rolCollection = mongoDatabase.GetCollection<Rol>("roles");
        }

        public async Task<List<Rol>> SelectRolesRepository()
        {
            try
            {
                var filter = Builders<Rol>.Filter.Empty;
                return await _rolCollection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<IAsyncCursor<Rol>> SelectRolesFilterRepository(FilterDefinition<Rol> filter)
        {
            try
            {
                return await _rolCollection.FindAsync(filter);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<Rol> SelectRolRepository(string idRol)
        {
            try
            {
                return await _rolCollection.Find(driver => driver.IdRol == idRol).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
    }