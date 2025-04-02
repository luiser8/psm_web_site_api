using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Services.Usuarios;
    public interface IUsuariosService
    {
        Task<List<UsuariosResponseDto>> SelectUsuariosService();
        Task<UsuariosResponseDto> SelectUsuariosPorIdService(string IdUsuario);
        Task<List<Auditoria>> SelectUsuariosPorAuditoriaService(string IdUsuario);
        Task<bool> PostUsuariosService(UsuariosPayloadDto nuevoUsuario);
        Task<bool> PutUsuariosService(string IdUsuario, UsuariosPayloadPutDto usuarios);
        Task<bool> DeleteUsuariosService(UsuariosPayloadDeleteDto usuario);
        Task<bool> SetStatusUsuariosService(UsuariosPayloadDeleteDto usuario, bool status);
    }
