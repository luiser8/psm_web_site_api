using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using psm_web_site_api_project.Utils.JsonArrayModelBinder;

namespace psm_web_site_api_project.Entities;
public class HeaderDto
{
    [BsonId]
    [JsonIgnore]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdHeader { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdExtension { get; set; }
    public IFormFile? Logo { get; set; }
    public bool EsNacional { get; set; } = false;

    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string? IdUsuarioIdentity { get; set; }
    
    [FromJson]
    public List<HeaderCollection>? HeaderCollections { get; set; }
    public bool Activo { get; set; } = true;
}
