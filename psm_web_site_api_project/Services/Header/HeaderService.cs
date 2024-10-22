using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Extensiones;
using psm_web_site_api_project.Repository.ImageUpAndDown;
using psm_web_site_api_project.Repository.Usuarios;

namespace psm_web_site_api_project.Repository.Headers;
public class HeaderService(IHeaderRepository headerRepository, IAuditoriasRepository auditoriasRepository, IExtensionesRepository extensionesRepository, IImageUpAndDownService imageUpAndDownService, IUsuariosRepository usuariosRepository) : IHeaderService
{
    private readonly IHeaderRepository _headerRepository = headerRepository;
    private readonly IAuditoriasRepository _auditoriasRepository = auditoriasRepository;
    private readonly IExtensionesRepository _extensionesRepository = extensionesRepository;
    private readonly IImageUpAndDownService _imageUpAndDownService = imageUpAndDownService;
    private readonly IUsuariosRepository _usuariosRepository = usuariosRepository;

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
            if (header.Logo == null) throw new NotImplementedException("Imagen de logo debe ser enviada");
            if (existeExtension == null)
                throw new NotImplementedException("Extension Id no existe");
            if (!existeExtension.Activo)
                throw new NotImplementedException("Extension desactivada no puede generarle logo");

            var usuarioExtension = await _usuariosRepository.SelectUsuariosPorIdRepository(header.IdUsuarioIdentity ?? string.Empty);
            var usuarioExtensionValid = usuarioExtension.Extension.Where(x => x.IdExtension == header.IdExtension);

            if(!usuarioExtensionValid.Any()) throw new NotImplementedException("Extension Id no pertenece al usuario");

            var saveLogoImage = await _imageUpAndDownService.PostImageUpAndDownService(header.Logo);
            if(saveLogoImage.Length < 0) throw new NotImplementedException("Ocurrió un error intentando guardar Logo");
            var newHeader = new Header
            {
                IdExtension = header.IdExtension,
                Logo = saveLogoImage,
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
                //existeHeader.Logo = header.Logo;
            if (header?.Activo != null)
                existeHeader.Activo = header.Activo;
            if (header?.IdExtension != null)
                existeHeader.IdExtension = header.IdExtension;
            if (header?.HeaderCollections != null || header?.HeaderCollections?.Count > 0)
                existeHeader.HeaderCollections = header.HeaderCollections;

            var existeExtension = await _extensionesRepository.SelectExtensionesPorIdRepository(header.IdExtension ?? string.Empty);
            if (header.Logo == null) throw new NotImplementedException("Imagen de logo debe ser enviada");
            if (existeExtension == null)
                throw new NotImplementedException("Extension Id no existe");
            if (!existeExtension.Activo)
                throw new NotImplementedException("Extension desactivada no puede generarle logo");

            var usuarioExtension = await _usuariosRepository.SelectUsuariosPorIdRepository(header.IdUsuarioIdentity ?? string.Empty);
            var usuarioExtensionValid = usuarioExtension.Extension.Where(x => x.IdExtension == header.IdExtension);

            if(!usuarioExtensionValid.Any()) throw new NotImplementedException("Extension Id no pertenece al usuario");

            var saveLogoImage = await _imageUpAndDownService.PostImageUpAndDownService(header.Logo);
            if(saveLogoImage.Length < 0) throw new NotImplementedException("Ocurrió un error intentando guardar Logo");
            var newHeader = new Header
            {
                IdExtension = header.IdExtension,
                Logo = saveLogoImage,
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