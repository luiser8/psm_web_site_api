using Microsoft.Extensions.Options;
using MongoDB.Driver;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Auditorias;
    public class AuditoriasRepository : IAuditoriasRepository
    {
        private readonly IMongoCollection<Auditoria> _auditoriaCollection;

        public AuditoriasRepository(IOptions<ConfigDB> options)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
            _auditoriaCollection = mongoDatabase.GetCollection<Auditoria>("auditoria");
        }

        public async Task<List<Auditoria>> SelectAuditoriasPorUsuarioIdRepository(string idUsuario)
        {
            try
            {
                return await _auditoriaCollection.Find(driver => driver.IdUsuario == idUsuario).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<bool> PostAuditoriasRepository(Auditoria auditoria)
        {
            try
            {
                await _auditoriaCollection.InsertOneAsync(auditoria);
                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public async Task<bool> DeleteAuditoriasRepository(string idAuditoria)
        {
            try
            {
                await _auditoriaCollection.DeleteOneAsync(idAuditoria);
                return true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
    }