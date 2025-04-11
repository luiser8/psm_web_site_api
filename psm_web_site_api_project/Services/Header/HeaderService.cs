using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Repository.Extensiones;
using psm_web_site_api_project.Repository.Headers;
using psm_web_site_api_project.Repository.ImageUpAndDown;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Responses;

namespace psm_web_site_api_project.Services.Headers;
public class HeaderService(IHeaderRepository headerRepository, IAuditoriasRepository auditoriasRepository, IExtensionesRepository extensionesRepository, IUsuariosRepository usuariosRepository, IImageUpAndDownService imageUpAndDownService) : IHeaderService
{
    public async Task<HeaderResponse> SelectHeaderPorIdExtensionService(string? idExtension)
    {
        try
        {
            var headers = await headerRepository.SelectHeaderPorIdExtensionRepository(idExtension);

            if (headers != null || !string.IsNullOrEmpty(headers?.IdHeader))
                headers?.HeaderCollections?.ForEach((hc) =>
                {
                    if (headers.Logo == null) throw new NotImplementedException("Imagen de logo no existe");
                    headers.HeaderExtensions ??= [];
                    headers.HeaderCollections ??= [];
                });
            if (headers != null || !string.IsNullOrEmpty(headers?.IdHeader))
                if (headers.EsNacional)
                {
                    var extensions = await extensionesRepository.SelectExtensionesRepository();
                    extensions.ForEach(ext =>
                    {
                        if (!ext.EsNacional)
                            headers?.HeaderExtensions?.AddRange([
                                new HeaderExtension
                                    {
                                        IdHeaderExtension = ext.IdExtension,
                                        Nombre = ext.Nombre,
                                        Link = ext.Nombre?.ToLower(),
                                        Target = true
                                    }
                            ]);
                    });
                }
            if (headers == null)
                throw new NotImplementedException("No existe un Header con este id de extension");

            var (content, contentType) = await imageUpAndDownService.SelectImageUpAndDownService(headers?.Logo, "Header", headers?.Nombre);

            return new HeaderResponse
            {
                IdHeader = headers?.IdHeader,
                IdExtension = headers?.IdExtension,
                Logo = $"data:{contentType};base64,{Convert.ToBase64String(content)}",
                Nombre = headers?.Nombre,
                EsNacional = headers?.EsNacional,
                HeaderCollections = headers?.HeaderCollections,
                HeaderExtensions = headers?.HeaderExtensions,
                FechaCreacion = headers?.FechaCreacion,
                Activo = headers?.Activo
            };
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PostHeaderService(HeaderPayload header)
    {
        try
        {
            if (header == null) throw new ArgumentNullException(nameof(header), "Header cannot be null");
            var existeExtension = await extensionesRepository.SelectExtensionesPorIdRepository(header.IdExtension ?? string.Empty);
            if (header.Logo == null) throw new NotImplementedException("Imagen de logo debe ser enviada");
            if (existeExtension == null)
                throw new NotImplementedException("Extension Id no existe");
            if (!existeExtension.Activo)
                throw new NotImplementedException("Extension desactivada no puede generarle logo");

            var headersExists = await headerRepository.SelectHeaderPorIdExtensionRepository(header.IdExtension ?? string.Empty);
            if (headersExists is { Activo: true })
            {
                throw new ArgumentNullException(nameof(header), "Existe una Extension");
            }

            var usuarioExtension = await usuariosRepository.SelectUsuariosPorIdRepository(header.IdUsuarioIdentity ?? string.Empty);
            var usuarioExtensionValid = usuarioExtension.Extension.Where(x => x.IdExtension == header.IdExtension);

            if (!usuarioExtensionValid.Any()) throw new NotImplementedException("Extension Id no pertenece al usuario");

            var saveLogoImage = await imageUpAndDownService.PostImageUpAndDownService(header.Logo, "Header", existeExtension.Nombre);
            if (string.IsNullOrEmpty(saveLogoImage)) throw new NotImplementedException("Ocurrió un error intentando guardar Logo");
            var newHeader = new Header
            {
                IdExtension = header.IdExtension,
                Logo = saveLogoImage,
                EsNacional = header.EsNacional,
                Nombre = existeExtension.Nombre,
                Activo = header.Activo,
                HeaderCollections = header.HeaderCollections
            };

            var response = await headerRepository.PostHeaderRepository(newHeader);
            await auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Header", Accion = "Creación de header", IdUsuario = header?.IdUsuarioIdentity?.ToString() ?? string.Empty });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PutHeaderService(string idHeader, HeaderPayload header)
    {
        try
        {
            if (idHeader == null || string.IsNullOrEmpty(idHeader)) throw new ArgumentNullException(nameof(idHeader), "Header Id cannot be null");
            if (header == null) throw new ArgumentNullException(nameof(header), "Header cannot be null");

            var headersExists = await headerRepository.SelectHeaderPorIdRepository(idHeader) ?? throw new NotImplementedException("No existe un Header con este id");
            var existeExtension = await extensionesRepository.SelectExtensionesPorIdRepository(header.IdExtension ?? string.Empty);

            var usuarioExtension = await usuariosRepository.SelectUsuariosPorIdRepository(header.IdUsuarioIdentity ?? string.Empty);
            var usuarioExtensionValid = usuarioExtension.Extension.Where(x => x.IdExtension == header.IdExtension);

            if (!usuarioExtensionValid.Any()) throw new NotImplementedException("Extension Id no pertenece al usuario");

            var saveLogoImage = await imageUpAndDownService.PutImageUpAndDownService(header.Logo, "Header", existeExtension.Nombre);
            if (string.IsNullOrEmpty(saveLogoImage)) throw new NotImplementedException("Ocurrió un error intentando guardar Logo");
            var newHeader = new Header
            {
                IdHeader = idHeader,
                IdExtension = header.IdExtension,
                Logo = saveLogoImage,
                EsNacional = header.EsNacional,
                Nombre = existeExtension.Nombre,
                Activo = header.Activo,
                HeaderCollections = header.HeaderCollections
            };

            var response = await headerRepository.PutHeaderRepository(idHeader, newHeader);
            await auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Header", Accion = "Actualizar header", IdUsuario = header?.IdUsuarioIdentity?.ToString() ?? string.Empty });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteHeaderService(HeaderPayload headerDto)
    {
        try
        {
            if (string.IsNullOrEmpty(headerDto.IdHeader))
                throw new ArgumentNullException(nameof(headerDto), "IdHeader cannot be null or empty");

            var headersExists = await headerRepository.SelectHeaderPorIdRepository(headerDto.IdHeader);
            if (headersExists == null)
            {
                throw new NotImplementedException("No existe un Header con este id");
            }

            var existeExtension = await extensionesRepository.SelectExtensionesPorIdRepository(headersExists.IdExtension ?? string.Empty);

            var deleteLogoImage = headersExists.Logo != null && await imageUpAndDownService.DeleteImageUpAndDownService(headersExists.Logo, "Header", existeExtension.Nombre);
            if (!deleteLogoImage) throw new NotImplementedException("Ocurrió un error intentando eliminar el Logo");

            var response = await headerRepository.DeleteHeaderRepository(headerDto.IdHeader);
            await auditoriasRepository.PostAuditoriasRepository(new Auditoria { Tabla = "Header", Accion = "Eliminación de header", IdUsuario = headerDto?.IdUsuarioIdentity?.ToString() ?? string.Empty });
            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> AddItemToHeader(string idExtension, HeaderCollection newItem)
    {
        if (string.IsNullOrEmpty(idExtension))
            throw new ArgumentNullException(nameof(idExtension), "IdExtension cannot be null or empty");
        if (newItem == null)
            throw new ArgumentNullException(nameof(newItem), "NewItem cannot be null");

        var headersExists = await headerRepository.SelectHeaderPorIdExtensionRepository(idExtension ?? string.Empty);

        if (headersExists is { Activo: false } && newItem == null)
            throw new ArgumentNullException(nameof(newItem), "Header disabled cannot add items");

        var newHeaderCollection =
            new HeaderCollection
            {
                Nombre = newItem.Nombre,
                Link = newItem.Link,
                Target = newItem.Target
            };

        return await headerRepository.AddItemToHeader(idExtension ?? throw new ArgumentNullException(nameof(newItem), "IdExtension cannot be null"), newHeaderCollection);
    }

    public async Task<bool> RemoveItemFromHeader(string idExtension, string itemNombreToRemove)
    {
        if (string.IsNullOrEmpty(idExtension))
            throw new ArgumentNullException(nameof(idExtension), "IdExtension cannot be null or empty");
        if (string.IsNullOrEmpty(itemNombreToRemove))
            throw new ArgumentNullException(nameof(itemNombreToRemove), "itemNombreToRemove cannot be null");

        return await headerRepository.RemoveItemFromHeader(idExtension, itemNombreToRemove);
    }
}