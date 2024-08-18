using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Dto;
    public class UsuariosResponseDto
    {
        public string? IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Correo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
        public virtual Rol Rol { get; set; } = new Rol();
        public virtual ICollection<Extension> Extension { get; set; } = new List<Extension>();
    }

    public class UsuariosPayloadDto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? IdUsuario { get; set; } = null!;
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? IdUsuarioIdentity { get; set; }
        public string? Nombres { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Rol { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string[]? Extensiones { get; set; }
        public string? Apellidos { get; set; }
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }
    }

    public class UsuariosPayloadPutDto
    {

        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? IdUsuarioIdentity { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? IdUsuario { get; set; } = null!;
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Rol { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string[]? Extensiones { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }
        public bool? Activo { get; set; }
    }

    public class UsuariosPayloadDeleteDto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? IdUsuarioIdentity { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdUsuario { get; set; } = null!;
    }

    public class LoginPayloadDto
    {
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }
        public bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
