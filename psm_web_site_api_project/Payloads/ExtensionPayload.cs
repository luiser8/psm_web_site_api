using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Payloads;
    public class ExtensionPayload
    {
        [BsonId]
        [JsonIgnore]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdExtension { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string? IdUsuarioIdentity { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool? Activo { get; set; } = true;
    }