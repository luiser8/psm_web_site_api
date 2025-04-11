using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Responses;

namespace psm_web_site_api_project.Services.Usuarios;
    public interface IUsuariosService
    {
        Task<List<UsuariosResponse>> SelectUsuariosService();
        Task<UsuariosResponse> SelectUsuariosPorIdService(string IdUsuario);
        Task<List<Auditoria>> SelectUsuariosPorAuditoriaService(string IdUsuario);
        Task<bool> PostUsuariosService(UsuarioPayload nuevoUsuario);
        Task<bool> PutUsuariosService(string IdUsuario, UsuariosPayloadPut usuarios);
        Task<bool> DeleteUsuariosService(UsuariosPayloadDelete usuario);
        Task<bool> SetStatusUsuariosService(UsuariosPayloadDelete usuario, bool status);
    }
