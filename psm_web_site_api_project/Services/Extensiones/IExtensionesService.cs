using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Services.Extensiones;

public interface IExtensionesService
{
    Task<List<Extension>> SelectExtensionesService();
    Task<Extension> SelectExtensionesPorIdService(string idExtension);
    Task<bool> PostExtensionesService(ExtensionDto extension);
    Task<bool> PutExtensionesService(string idExtension, ExtensionDto extension);
    Task<bool> DeleteExtensionesService(ExtensionDto extension);
    Task<List<Extension>> GetCursorExtension(List<string> extensiones);
}