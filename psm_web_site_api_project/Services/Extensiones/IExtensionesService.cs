using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
namespace psm_web_site_api_project.Services.Extensiones;

public interface IExtensionesService
{
    Task<List<Extension>> SelectExtensionesService();
    Task<Extension> SelectExtensionesPorIdService(string idExtension);
    Task<bool> PostExtensionesService(ExtensionPayload extension);
    Task<bool> PutExtensionesService(string idExtension, ExtensionPayload extension);
    Task<bool> DeleteExtensionesService(ExtensionPayload extension);
    Task<List<Extension>> GetCursorExtension(List<string> extensiones);
}