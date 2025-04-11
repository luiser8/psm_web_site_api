using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;

namespace psm_web_site_api_project.Repository.Autenticacion;

public interface IAutenticacionRepository
{
    Task<bool> ValidarRepository(string usuarioId, string token);
    Task<Usuario> SessionRepository(LoginPayload loginPayload);
    Task<Usuario> RefrescoRepository(string refreshToken);
    Task<bool> RemoverRepository(string usuarioId);
}