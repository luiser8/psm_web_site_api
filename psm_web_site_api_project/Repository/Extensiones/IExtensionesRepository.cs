using MongoDB.Driver;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Extensiones;
    public interface IExtensionesRepository
    {
        Task<List<Extension>> SelectExtensionesRepository();
        Task<IAsyncCursor<Extension>> SelectExtensionesFilterRepository(FilterDefinition<Extension> filter);
        Task<Extension> SelectExtensionesPorIdRepository(string idExtension);
        Task<Extension> SelectExtensionesPorNombreRepository(string nombre);
        Task<bool> PostExtensionesRepository(Extension extension);
        Task<bool> PutExtensionesRepository(string idExtension, Extension extension);
        Task<bool> DeleteExtensionesRepository(string idExtension);
    }