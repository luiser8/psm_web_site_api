using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Autenticacion;

public interface IAutenticacionRepository
{
    Task<bool> ValidarRepository(string usuarioId, string token);
    Task<Usuario> SessionRepository(LoginPayloadDto loginPayloadDto);
    Task<Usuario> RefrescoRepository(string refreshToken);
    Task<bool> RemoverRepository(string usuarioId);
}