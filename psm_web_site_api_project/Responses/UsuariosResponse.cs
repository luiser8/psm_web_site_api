using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Responses;
public class UsuariosResponse
{
    public string? IdUsuario { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? Correo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
    public virtual Rol Rol { get; set; } = new Rol();
    public virtual ICollection<Extension> Extension { get; set; } = [];
}