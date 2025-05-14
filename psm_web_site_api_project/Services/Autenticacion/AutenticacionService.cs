using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Autenticacion;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Responses;

namespace psm_web_site_api_project.Services.Autenticacion;
public class AutenticacionService : IAutenticacionService
{
    private readonly IAutenticacionRepository _autenticacionRepository;
    private readonly IUsuariosRepository _usuariosRepository;
    private readonly IAuditoriasRepository _auditoriasRepository;

    public AutenticacionService(IAutenticacionRepository autenticacionRepository, IUsuariosRepository usuariosRepository, IAuditoriasRepository auditoriasRepository)
    {
        _autenticacionRepository = autenticacionRepository;
        _usuariosRepository = usuariosRepository;
        _auditoriasRepository = auditoriasRepository;
    }

    public async Task<TokenResponse> SessionService(LoginPayload loginPayload)
    {
        try
        {
            var response = await _autenticacionRepository.SessionRepository(loginPayload);
            if (response.IdUsuario == null)
                return new TokenResponse
                {
                    accessToken = response?.TokenAcceso,
                    refreshToken = response?.TokenRefresco,
                };
            var request = await _usuariosRepository.PutUsuariosRepository(response.IdUsuario ?? string.Empty, response);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Inicio de sesión de usuario", IdUsuario = response.IdUsuario });
            return new TokenResponse
            {
                accessToken = response?.TokenAcceso,
                refreshToken = response?.TokenRefresco,
            };
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<TokenResponse> RefrescoService(string refreshToken)
    {
        try
        {
            var response = await _autenticacionRepository.RefrescoRepository(refreshToken);
            var request = await _usuariosRepository.PutUsuariosRepository(response.IdUsuario ?? string.Empty, response);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Usuarios", Accion = "Refresh de token", IdUsuario = response.IdUsuario });
            return new TokenResponse
            {
                accessToken = response?.TokenAcceso,
                refreshToken = response?.TokenRefresco,
            };
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> RemoverService(string usuarioId)
    {
        try
        {
            return await _autenticacionRepository.RemoverRepository(usuarioId);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> ValidarService(string usuarioId, string token)
    {
        try
        {
            return await _autenticacionRepository.ValidarRepository(usuarioId, token);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}