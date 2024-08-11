using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Headers;
public interface IHeaderService
{
    Task<Header> SelectHeaderPorIdService(string idHeader);
    Task<Header> SelectHeaderPorIdExtensionService(string idExtension);
    Task<bool> PostHeaderService(HeaderDto header);
    Task<bool> PutHeaderService(string IdHeader, Header header);
    Task<bool> DeleteHeaderService(string IdHeader);
}