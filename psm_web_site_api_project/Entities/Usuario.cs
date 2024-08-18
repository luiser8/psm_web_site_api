using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Entities;
public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdUsuario { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? Correo { get; set; }
    public string? Contrasena { get; set; }
    public string? TokenAcceso { get; set; } = null;
    public string? TokenRefresco { get; set; } = null;
    public DateTime? TokenCreado { get; set; }
    public DateTime? TokenExpiracion { get; set; }
    public DateTime? FechaCreacion { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
    public virtual Rol Rol { get; set; } = new Rol();
    public virtual ICollection<Extension> Extension { get; set; } = new List<Extension>();
}
