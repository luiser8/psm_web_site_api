using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Responses;
public class CarouselResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdCarousel { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdExtension { get; set; }
    public bool EsNacional { get; set; }
    public List<CarouselCollection>? CarouselCollections { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
}