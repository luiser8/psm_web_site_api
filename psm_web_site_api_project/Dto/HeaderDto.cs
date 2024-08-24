using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace psm_web_site_api_project.Entities;
public class HeaderDto
{
    [BsonId]
    [JsonIgnore]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdHeader { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdExtension { get; set; }
    public string? Logo { get; set; }
    public bool EsNacional { get; set; } = false;

    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string? IdUsuarioIdentity { get; set; }
    public List<HeaderCollection>? HeaderCollections { get; set; }
    public bool Activo { get; set; } = true;
}
