using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Responses;

namespace psm_web_site_api_project.Services.Headers;
public interface IHeaderService
{
    Task<HeaderResponse> SelectHeaderPorIdExtensionService(string? idExtension);
    Task<bool> PostHeaderService(HeaderPayload header);
    Task<bool> PutHeaderService(string idHeader, HeaderPayload headerDto);
    Task<bool> DeleteHeaderService(HeaderPayload headerDto);
    Task<bool> AddItemToHeader(string idExtension, HeaderCollection newItem);
    Task<bool> RemoveItemFromHeader(string idExtension, string itemNombreToRemove);
}