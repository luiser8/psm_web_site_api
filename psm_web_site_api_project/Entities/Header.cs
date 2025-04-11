using System.Text.Json.Serialization;
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
    public string? Nombre { get; set; }
    public bool EsNacional { get; set; }
    public List<HeaderCollection>? HeaderCollections { get; set; }
    public List<HeaderExtension>? HeaderExtensions { get; set; } = null;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
}

public class HeaderCollection
{
    [BsonId]
    [JsonIgnore]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdHeaderCollection { get; set; } = ObjectId.GenerateNewId().ToString();
    public string? Nombre { get; set; }
    public string? Link { get; set; }
    public bool Target { get; set; } = false;
    public bool Activo { get; set; } = true;
}

public class HeaderExtension
{
    [BsonId]
    [JsonIgnore]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? IdHeaderExtension { get; set; } = ObjectId.GenerateNewId().ToString();
    public string? Nombre { get; set; }
    public string? Link { get; set; }
    public bool Target { get; set; } = false;
}