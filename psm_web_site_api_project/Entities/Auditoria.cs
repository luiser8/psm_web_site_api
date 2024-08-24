using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Entities;
    public class Auditoria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdAuditoria { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdUsuario { get; set; }
        public string? Accion { get; set; }
        public string? Tabla { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
