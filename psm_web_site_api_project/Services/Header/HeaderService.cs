using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Extensiones;

namespace psm_web_site_api_project.Repository.Headers;
public class HeaderService(IHeaderRepository headerRepository, IAuditoriasRepository auditoriasRepository, IExtensionesRepository extensionesRepository) : IHeaderService
{
    private readonly IHeaderRepository _headerRepository = headerRepository;
    private readonly IAuditoriasRepository _auditoriasRepository = auditoriasRepository;
    private readonly IExtensionesRepository _extensionesRepository = extensionesRepository;

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
            var headers = await _headerRepository.SelectHeaderPorIdExtensionRepository(idExtension);

            if (headers != null)
                headers?.HeaderCollections?.ForEach(async hc =>
                {
                    if (headers.EsNacional)
                    {
                        var extensions = await _extensionesRepository.SelectExtensionesRepository();
                        headers.HeaderExtensions ??= [];
                        extensions.ForEach(ext =>
                        {
                            headers.HeaderExtensions.Add(new HeaderExtension
                            {
                                IdHeaderExtension = ext.IdExtension,
                                Nombre = ext.Nombre,
                                Link = ext.Nombre?.ToLower(),
                                Target = hc.Target
                            });
                        });
                    }
                });

            return headers ?? new Header { };
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
            var newHeader = new Header
            {
                IdExtension = header.IdExtension,
                Logo = header.Logo,
                EsNacional = header.EsNacional,
                Activo = header.Activo,
                HeaderCollections = header.HeaderCollections
            };
            var response = await _headerRepository.PostHeaderRepository(newHeader);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Header", Accion = "Creación de header", IdUsuario = header?.IdUsuarioIdentity?.ToString() ?? string.Empty });
            return response;
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
            var response = await _headerRepository.PutHeaderRepository(IdHeader, header);
            return response;
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
            var response = await _headerRepository.DeleteHeaderRepository(IdHeader);
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}