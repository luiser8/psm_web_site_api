using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Responses;
public class HeaderResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdHeader { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdExtension { get; set; }
    public string? Logo { get; set; }
    public string? Nombre { get; set; }
    public bool? EsNacional { get; set; }
    public List<HeaderCollection>? HeaderCollections { get; set; }
    public List<HeaderExtension>? HeaderExtensions { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public bool? Activo { get; set; }
}