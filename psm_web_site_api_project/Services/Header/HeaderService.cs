using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Auditorias;

namespace psm_web_site_api_project.Repository.Headers;
public class HeaderService(IHeaderRepository headerRepository, IAuditoriasRepository auditoriasRepository) : IHeaderService
{
    private readonly IHeaderRepository _headerRepository = headerRepository;
    private readonly IAuditoriasRepository _auditoriasRepository = auditoriasRepository;

    public async Task<Header> SelectHeaderPorIdService(string idHeader)
    {
        try
        {
            return await _headerRepository.SelectHeaderPorIdRepository(idHeader);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<Header> SelectHeaderPorIdExtensionService(string idExtension)
    {
        try
        {
            return await _headerRepository.SelectHeaderPorIdExtensionRepository(idExtension);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PostHeaderService(HeaderDto header)
    {
        try
        {
            var newHeader = new Header {
                IdExtension = header.IdExtension,
                Activo = header.Activo,
                HeaderCollections = header.HeaderCollections,
            };
            await _headerRepository.PostHeaderRepository(newHeader);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Header", Accion = "Creación de header", IdUsuario = header?.IdUsuarioIdentity.ToString() });
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PutHeaderService(string IdHeader, Header header)
    {
        try
        {
            await _headerRepository.PutHeaderRepository(IdHeader, header);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteHeaderService(string IdHeader)
    {
        try
        {
            await _headerRepository.DeleteHeaderRepository(IdHeader);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}