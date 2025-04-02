using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Dto;
public class TokenDto
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdUsuario { get; set; }
    public string? Correo { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public virtual Rol? Rol { get; set; }
    public virtual ICollection<Extension>? Extension { get; set; }
}

public class TokenResponseDto
{
    public string? accessToken { get; set; }
    public string? refreshToken { get; set; }
}

public class UserTokenData
{
    public string? IdUser { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Rol { get; set; }
    public string? Extensions { get; set; }
    public long Exp { get; set; }
    [JsonIgnore]
    public bool IsExpired => DateTimeOffset.FromUnixTimeSeconds(Exp) <= DateTimeOffset.UtcNow;
}