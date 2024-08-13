using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Entities;
    public class Header
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdHeader { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdExtension { get; set; }
        public string? Logo { get; set; }
        public List<HeaderCollection>? HeaderCollections { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
    }

    public class HeaderCollection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdHeaderCollection { get; set; }
        public string? Nombre { get; set; }
        public string? Link { get; set; }
        public bool Target { get; set; } = false;
        public bool EsNacional { get; set; } = false;
        public List<HeaderExtension>? HeaderExtensions { get; set; } = null;
    }

    public class HeaderExtension
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdHeaderExtension { get; set; }
        public string? Nombre { get; set; }
        public string? Link { get; set; }
        public bool Target { get; set; } = false;
    }