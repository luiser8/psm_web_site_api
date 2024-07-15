using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Entities;
    public class Footer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdFooter { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdExtension { get; set; }
        public List<FooterCollection>? FooterCollections { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
    }

    public class FooterCollection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdFooterCollection { get; set; }
        public string? Logo { get; set; }
        public string? Nombre { get; set; }
        public bool EsNacional { get; set; } = false;
        public List<FooterOptions>? FooterOptions { get; set; } = null;
    }

    public class FooterOptions
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdFooterOptions { get; set; }
        public string? Nombre { get; set; }
        public string? Link { get; set; }
        public bool Target { get; set; } = false;
    }