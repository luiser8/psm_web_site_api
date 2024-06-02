using MongoDB.Driver;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Roles;
    public interface IRolesRepository
    {
        Task<List<Rol>> SelectRolesRepository();
        Task<IAsyncCursor<Rol>> SelectRolesFilterRepository(FilterDefinition<Rol> filter);
        Task<Rol> SelectRolRepository(string idRol);
    }