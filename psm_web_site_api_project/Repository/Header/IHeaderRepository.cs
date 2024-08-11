using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Headers;
public interface IHeaderRepository
{
    Task<Header> SelectHeaderPorIdRepository(string idHeader);
    Task<Header> SelectHeaderPorIdExtensionRepository(string idExtension);
    Task<bool> PostHeaderRepository(Header header);
    Task<bool> PutHeaderRepository(string IdHeader, Header header);
    Task<bool> DeleteHeaderRepository(string IdHeader);
}