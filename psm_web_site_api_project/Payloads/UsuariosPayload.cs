using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Payloads;
    public class UsuarioPayload
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

    public class UsuariosPayloadPut
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

    public class UsuariosPayloadDelete
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? IdUsuarioIdentity { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdUsuario { get; set; } = null!;
    }

