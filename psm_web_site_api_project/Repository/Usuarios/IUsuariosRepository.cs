using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Usuarios;
public interface IUsuariosRepository
{
    Task<List<Usuario>> SelectUsuariosRepository();
    Task<Usuario> SelectUsuariosPorIdRepository(string idUsuario);
    Task<Usuario> SelectUsuariosPorCorreoRepository(string correo);
    Task<Usuario> PostUsuariosRepository(Usuario usuario);
    Task<bool> PutUsuariosRepository(string IdUsuario, Usuario usuarios);
    Task<bool> DeleteUsuariosRepository(string IdUsuario);
    Task<bool> SetStatusUsuariosRepository(string IdUsuario, bool status);
}