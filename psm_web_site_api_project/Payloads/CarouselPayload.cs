using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Payloads;

public class CarouselPayload
{
    [BsonId]
    [JsonIgnore]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdCarousel { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdExtension { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string? IdUsuarioIdentity { get; set; }
    public string? Nombre { get; set; }
    public IFormFile? Imagen { get; set; }
    public string? Link { get; set; }
    public string? Title { get; set; }
    public bool Target { get; set; } = false;
    public string? Iframe { get; set; }
    public bool Activo { get; set; } = true;
}

public class CarouselCollectionPayload
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdCarouselCollection { get; set; }
    public string? Nombre { get; set; }
    public IFormFile? Imagen { get; set; }
    public string? Link { get; set; }
    public string? Title { get; set; }
    public bool Target { get; set; } = false;
    public string? Iframe { get; set; }
}