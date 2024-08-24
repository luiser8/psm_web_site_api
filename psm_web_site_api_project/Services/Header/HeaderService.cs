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
            var existeExtension = await _extensionesRepository.SelectExtensionesPorIdRepository(header.IdExtension ?? string.Empty);
            var newHeader = new Header
            {
                IdExtension = header.IdExtension,
                Logo = header.Logo,
                EsNacional = header.EsNacional,
                Nombre = existeExtension.Nombre,
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

    public async Task<bool> PutHeaderService(string idHeader, HeaderDto header)
    {
        try
        {
            var existeHeader = await _headerRepository.SelectHeaderPorIdRepository(idHeader) ?? throw new NotImplementedException("No existe header");

            if (header?.EsNacional != null)
                existeHeader.EsNacional = header.EsNacional;
            if (header?.Logo != null)
                existeHeader.Logo = header.Logo;
            if (header?.Activo != null)
                existeHeader.Activo = header.Activo;
            if (header?.IdExtension != null)
                existeHeader.IdExtension = header.IdExtension;
            if (header?.HeaderCollections != null || header?.HeaderCollections?.Count > 0)
                existeHeader.HeaderCollections = header.HeaderCollections;

            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Headers", Accion = "Actualización de header", IdUsuario = header?.IdUsuarioIdentity?.ToString() });
            return await _headerRepository.PutHeaderRepository(idHeader, existeHeader);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteHeaderService(HeaderDto headerDto)
    {
        try
        {
            var response = await _headerRepository.DeleteHeaderRepository(headerDto.IdHeader);
            await _auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Header", Accion = "Eliminación de header", IdUsuario = headerDto?.IdUsuarioIdentity?.ToString() ?? string.Empty });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}