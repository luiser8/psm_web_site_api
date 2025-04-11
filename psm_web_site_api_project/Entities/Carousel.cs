using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Entities;

    public class Carousel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdCarousel { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdExtension { get; set; }
        public bool EsNacional { get; set; }
        public List<CarouselCollection>? CarouselCollections { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
    }

    public class CarouselCollection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdCarouselCollection { get; set; }
        public string? Nombre { get; set; }
        public string? Imagen { get; set; }
        public string? Link { get; set; }
        public string? Title { get; set; }
        public bool Target { get; set; } = false;
        public string? Iframe { get; set; }
        public bool Activo { get; set; } = true;
    }