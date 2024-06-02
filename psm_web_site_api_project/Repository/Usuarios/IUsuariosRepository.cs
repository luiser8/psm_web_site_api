using psm_web_site_api_project.Dto;

namespace psm_web_site_api_project.Repository.Usuarios;
    public interface IUsuariosRepository
    {
        Task<List<Entities.Usuarios>> SelectUsuariosRepository();
        Task<Entities.Usuarios> SelectUsuariosPorIdRepository(string idUsuario);
        Task<Entities.Usuarios> SelectUsuariosPorCorreoRepository(string correo);
        Task<Entities.Usuarios> LoginUsuarioRepository(LoginPayloadDto loginPayloadDto);
        Task<Entities.Usuarios> PostUsuariosRepository(Entities.Usuarios usuario);
        Task<bool> PutUsuariosRepository(string IdUsuario, Entities.Usuarios usuarios);
        Task<Entities.Usuarios> RefreshTokenRepository(string actualToken);
        Task<bool> DeleteUsuariosRepository(string IdUsuario);
    }