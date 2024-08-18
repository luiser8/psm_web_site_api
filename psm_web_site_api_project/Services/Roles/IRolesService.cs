using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Services.Roles;

public interface IRolesService
{
    Task<List<Rol>> SelectRolesService();
    Task<Rol> SelectRolPorIdService(string idRol);
    Task<List<Rol>> GetCursorRol(List<string> roles);
}