using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Entities;
    public class Rol
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdRol { get; set; }
        public string? Nombre { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
    }