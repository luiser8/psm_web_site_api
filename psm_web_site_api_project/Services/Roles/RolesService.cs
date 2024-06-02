using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Roles;

namespace psm_web_site_api_project.Services.Roles;
public class RolesService : IRolesService
{
    private readonly IRolesRepository _rolesRepository;

    public RolesService(IRolesRepository rolesRepository)
    {
        _rolesRepository = rolesRepository;
    }

    public async Task<List<Rol>> SelectRolesService()
    {
        try
        {
            return await _rolesRepository.SelectRolesRepository();
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}